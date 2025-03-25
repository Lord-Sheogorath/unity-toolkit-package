using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Text.RegularExpressions;
using Codice.Client.BaseCommands;
using Sirenix.OdinInspector;

namespace LordSheo.Editor.UI
{
	public interface ISearch
	{
		IEnumerable<string> GetSearchStrings();
	}
	public struct StringSearch : ISearch
	{
		public string value;

		public StringSearch(string value)
		{
			this.value = value;
		}

		public IEnumerable<string> GetSearchStrings()
		{
			yield return value;
		}

		public static implicit operator StringSearch(string value)
		{
			return new(value);
		}
	}
	public struct PathSearch : ISearch
	{
		private static char[] separators = new char[] { '/', '\\' };
		public string path;

		public string name;
		public string[] split;

		public PathSearch(string value)
		{
			this.path = value;

			split = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			name = split.LastOrDefault();
		}

		public IEnumerable<string> GetSearchStrings()
		{
			yield return path;

			foreach (var item in split)
			{
				yield return item;
			}
		}

		public static implicit operator PathSearch(string value)
		{
			return new(value);
		}
	}

	public class BetterGenericSelector<T> : GenericSelector<T>
		where T : ISearch
	{
		public class SearchInfo
		{
			public string highestScoreKey = string.Empty;

			public readonly Dictionary<string, SearchScore> scores = new();

			public void SetScore(string key, SearchScore score)
			{
				scores[key] = score;

				if (score.contains == false)
				{
					return;
				}

				if (string.IsNullOrEmpty(highestScoreKey))
				{
					highestScoreKey = key;
					return;
				}

				var highestScore = GetHighestScore();

				if (highestScore == null || highestScore.score < score.score)
				{
					highestScoreKey = key;
				}
			}

			public SearchScore GetHighestScore()
			{
				return scores.GetValueOrDefault(highestScoreKey);
			}
		}
		public class SearchScore
		{
			public bool contains;
			public int score;
		}

		public enum SearchMode
		{
			Fuzzy,
			Regex,
		}

		[NonSerialized]
		public readonly Dictionary<OdinMenuItem, SearchInfo> cachedSearchInfo = new();

		[NonSerialized]
		public string cachedSearchTerm;

		[NonSerialized]
		public OdinEditorWindow window;

		[NonSerialized, ShowInInspector, PropertyOrder(-1)]
		public SearchMode searchMode = SearchMode.Fuzzy;

		[NonSerialized]
		public bool isValidSearchTerm = true;

		[NonSerialized, ShowInInspector, PropertyOrder(-1)]
		public bool showItemPathsAsNames = false;

		public string SearchTerm => SelectionTree.Config.SearchTerm;

		public BetterGenericSelector(string title, bool multiSelect, IEnumerable<GenericSelectorItem<T>> collection)
			: base(title, multiSelect, collection)
		{
			SelectionTree.Config.SearchFunction = DefaultSearch;

			foreach (var item in SelectionTree.EnumerateTree())
			{
				item.OnDrawItem += OnDrawItem;
			}
		}

		public void SetWindow(OdinEditorWindow window)
		{
			if (this.window != null)
			{
				this.window.OnBeginGUI -= OnBeginGUI;
				this.window.OnEndGUI -= OnEndGUI;
			}
			
			this.window = window;

			window.OnBeginGUI += OnBeginGUI;
			window.OnEndGUI += OnEndGUI;
		}

		public string GetHighestScore(OdinMenuItem item, out SearchScore score)
		{
			if (cachedSearchInfo.TryGetValue(item, out var info) == false)
			{
				score = null;
				return null;
			}

			score = info.GetHighestScore();
			return info.highestScoreKey;
		}

		protected void OnDrawItem(OdinMenuItem item)
		{
			if (item == null)
			{
				return;
			}

			if (item.IsSelected || showItemPathsAsNames)
			{
				OnDrawSelectedItem(item);
			}
		}

		private void OnDrawSelectedItem(OdinMenuItem item)
		{
			var path = item.GetFullPath();
			
			OdinUtil.DrawOverContent(item, path);
		}

		protected void OnBeginGUI()
		{
			if (string.IsNullOrEmpty(SearchTerm))
			{
				cachedSearchTerm = string.Empty;
			}

			if (cachedSearchTerm == SearchTerm)
			{
				FlushSearch();
			}
		}
		protected void OnEndGUI()
		{
			if (cachedSearchTerm == SearchTerm)
			{
				return;
			}

			cachedSearchTerm = SearchTerm;

			var items = SelectionTree.EnumerateTree()
				.Where(i => i != null)
				.Where(i => cachedSearchInfo.ContainsKey(i))
				.Select(i =>
				{
					var info = cachedSearchInfo[i];
					var score = info.GetHighestScore();

					return new
					{
						item = i,
						info = info,
						score = score,
					};
				})
				.Where(i => i.score != null)
				.OrderByDescending(i => i.score.score)
				.ThenBy(i => i.item.Name.Length)
				.ThenBy(i => i.item.Name)
				.Select(i => i.item);

			SelectionTree.FlatMenuTree.Clear();
			SelectionTree.FlatMenuTree.AddRange(items);

			SelectionTree.UpdateMenuTree();
		}

		public void FlushSearch()
		{
			cachedSearchInfo.Clear();
			isValidSearchTerm = true;
		}

		public bool DefaultSearch(OdinMenuItem item)
		{
			if (item == null)
			{
				return false;
			}

			var primaryContains = MatchesSearch(item, item.Name);

			var value = item.Value as ISearch;

			if (value == null)
			{
				return primaryContains;
			}

			var secondaryContains = value.GetSearchStrings()
				.Any(s => MatchesSearch(item, s));

			return primaryContains || secondaryContains;
		}

		protected bool MatchesSearch(OdinMenuItem item, string input)
		{
			if (item == null)
			{
				return false;
			}

			if (cachedSearchInfo.TryGetValue(item, out var info) == false)
			{
				cachedSearchInfo[item] = info = new();
			}

			if (info.scores.TryGetValue(input, out var cachedScore))
			{
				return cachedScore.contains;
			}

			var score = 0;

			var contains = searchMode switch
			{
				SearchMode.Fuzzy => MatchesFuzzySearch(item, input, out score),
				SearchMode.Regex => MatchesRegexSearch(item, input, out score),

				_ => throw new NotImplementedException(),
			};

			info.SetScore(input, new()
			{
				contains = contains,
				score = score,
			});

			return contains;
		}

		protected bool MatchesFuzzySearch(OdinMenuItem item, string input, out int score)
		{
			if (input.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
			{
				score = 1;
				return true;
			}

			return FuzzySearch.Contains(SearchTerm, input, out score);
		}
		protected bool MatchesRegexSearch(OdinMenuItem item, string input, out int score)
		{
			if (isValidSearchTerm == false)
			{
				score = 0;
				return false;
			}

			try
			{
				score = 0;
				return Regex.IsMatch(input, SearchTerm, RegexOptions.IgnoreCase);
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);

				score = 0;
				isValidSearchTerm = false;
				return false;
			}
		}
	}
}
