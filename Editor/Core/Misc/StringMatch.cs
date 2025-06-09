using System;
using System.IO;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;

namespace LordSheo.Editor
{
	[System.Serializable]
	public class StringMatch
	{
		public enum Mode
		{
			Contains,
			StartsWith,
			EndsWith,
			Regex,
		}

		[FoldoutGroup("@" + nameof(ToDisplayString) + "()")]
		public string pattern;
		[FoldoutGroup("@" + nameof(ToDisplayString) + "()")]
		public Mode mode;
		[FoldoutGroup("@" + nameof(ToDisplayString) + "()")]
		public bool invert;
		[FoldoutGroup("@" + nameof(ToDisplayString) + "()")]
		public bool ignoreCase = true;

		public bool IsMatch(string text)
		{
			return IsMatch(this, text);
		}

		public string ToDisplayString()
		{
			var text = mode.ToString().ToUpper();
			
			if (invert)
			{
				text += " != ";
			}
			else
			{
				text += " == ";
			}
			
			text += $"'{pattern}'";
			
			return text;
		}
		
		public static bool IsMatch(StringMatch match, string text)
		{
			var stringComparison = match.ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			var regexOptions = match.ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

			var isMatch = match.mode switch
			{
				Mode.Contains => text.Contains(match.pattern, stringComparison),
				Mode.StartsWith => text.StartsWith(match.pattern, stringComparison),
				Mode.EndsWith => text.EndsWith(match.pattern, stringComparison),
				Mode.Regex => Regex.IsMatch(text, match.pattern, regexOptions),

				_ => throw new ArgumentOutOfRangeException()
			};

			if (match.invert)
			{
				isMatch = isMatch == false;
			}

			return isMatch;
		}
	}
}