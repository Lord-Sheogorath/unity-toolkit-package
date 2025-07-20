using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using LordSheo;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine.Serialization;
using UPMClient = UnityEditor.PackageManager.Client;

namespace LordSheo.Editor.GitSpoon
{
	public class GitSpoonWindow : OdinMenuEditorWindow
	{
		private static List<GitSpoonManifest> manifests = new();

		private ListRequest _listPackagesRequest;

		[MenuItem("Window/LordSheo/GitSpoon")]
		public static void Open()
		{
			CreateWindow<GitSpoonWindow>("Git Spoon")
				.Refresh();
		}

		[InitializeOnLoadMethod]
		private static void OnEditorInitialise()
		{
			PromptToOpenNewWindow();
		}

		private static void PromptToOpenNewWindow(bool forceIfNoneOpen = false)
		{
			if (EditorApplicationUtility.IsWindowOpen<GitSpoonWindow>())
			{
				return;
			}

			var now = DateTime.UtcNow;

			if (forceIfNoneOpen == false)
			{
				var lastPromptTimeStr = EditorProject.projectOnlyPrefs.GetString("git_spoon_prompt", DateTime.MinValue.ToString());
				var lastPromptTime = DateTime.Parse(lastPromptTimeStr);

				var lastPromptDelta = now - lastPromptTime;

				if (lastPromptDelta.TotalDays < 1)
				{
					return;
				}
			}

			EditorProject.projectOnlyPrefs.SetString("git_spoon_prompt", now.ToString());

			EditorApplicationUtility.DisplayConfirmAction("GitSpoon - Open Window", "Would you like to open a new GitSpoon window?", () => { Open(); });
		}

		protected override void OnImGUI()
		{
			base.OnImGUI();

			if (Event.current.IsKeyUp(KeyCode.F5))
			{
				Refresh();
				return;
			}

			// foreach (var manifest in manifests)
			// {
			// 	var path = $"{manifest.author}/{manifest.displayName}";
			// 	var item = MenuTree.GetMenuItem(path);
			// }

			if (_listPackagesRequest != null && _listPackagesRequest.Error != null)
			{
				Debug.LogError(_listPackagesRequest.Error.message);
				_listPackagesRequest = null;
			}
		}
		
		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();

			tree.Add("Settings/Sources", GitSpoonManifestSourceSettings.Instance);

			manifests.Clear();

			foreach (var source in GitSpoonManifestSourceSettings.Instance.sources)
			{
				if (source.IsAsset)
				{
					var json = source.asset.text;
					var localManifests = JsonConvert.DeserializeObject<List<GitSpoonManifest>>(json);

					manifests.AddRange(localManifests);
				}

				// TO-DO: Handle git URL
			}

			foreach (var manifest in manifests)
			{
				var path = $"{manifest.author}/{manifest.displayName}";
				tree.Add(path, manifest);

				var item = tree.GetMenuItem(path);
				SetIconGetter(manifest, path, item);
			}

			return tree;
		}
		
		private void SetIconGetter(GitSpoonManifest manifest, string path, OdinMenuItem item)
		{
			item.IconGetter = () =>
			{
				if (_listPackagesRequest == null || _listPackagesRequest.IsCompleted == false)
				{
					if (_listPackagesRequest.Error != null)
					{
						return EditorIcons.UnityErrorIcon;
					}

					return EditorIcons.LoadingBar.Active;
				}

				return _listPackagesRequest.Result.Any(p => p.packageId == manifest.name)
					? EditorIcons.Checkmark.Active
					: EditorIcons.Download.Active;
			};
		}

		public void Refresh()
		{
			// TO-DO: Move this somewhere better
			_listPackagesRequest = UPMClient.List(true);
			ForceMenuTreeRebuild();
		}
	}
}