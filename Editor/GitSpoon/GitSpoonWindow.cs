using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using LordSheo;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UPMClient = UnityEditor.PackageManager.Client;

namespace LordSheo.Editor.GitSpoon
{
	public class GitSpoonWindow : OdinMenuEditorWindow
	{
		public class GitSpoonManifest
		{
			public string name;
			public string displayName;
			public string version;
			public string description;
			public string author;
			
			public string url;
		}

		private static List<GitSpoonManifest> manifests = new()
		{
			new GitSpoonManifest()
			{
				name = "com.lord-sheo.toolkit",
				displayName = "Unity Toolkit Package",
				author = "Lord Sheo",
				version = "1.0.0",
				url = "https://github.com/Lord-Sheogorath/unity-toolkit-package"
			},
		};

		private ListRequest _listPackagesRequest;
		
		[MenuItem("Window/LordSheo/GitSpoon")]
		public static void Open()
		{
			CreateWindow<GitSpoonWindow>("Git Spoon");
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
				
			EditorApplicationUtility.DisplayConfirmAction("GitSpoon - Open Window", "Would you like to open a new GitSpoon window?", () =>
			{
				Open();
			});
		}

		protected override void OnImGUI()
		{
			base.OnImGUI();

			if (_listPackagesRequest != null)
			{
				if (_listPackagesRequest.IsCompleted)
				{
					foreach (var manifest in manifests)
					{
						var path = $"{manifest.author}/{manifest.displayName}";
						var item = MenuTree.GetMenuItem(path);

						item.Icon = _listPackagesRequest.Result.Any(p => p.packageId == manifest.name) 
							? EditorIcons.Checkmark.Active 
							: EditorIcons.Download.Active;
					}
				}
				else if (_listPackagesRequest.Error != null)
				{
					Debug.LogError(_listPackagesRequest.Error.message);
				}
			}
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			_listPackagesRequest = UPMClient.List(true);

			var tree = new OdinMenuTree();

			foreach (var manifest in manifests)
			{
				var path = $"{manifest.author}/{manifest.displayName}";
				tree.Add(path, manifest);
			}

			return tree;
		}
	}
}