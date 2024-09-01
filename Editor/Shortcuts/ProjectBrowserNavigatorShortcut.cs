using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using System;
using System.Linq;

namespace Editor.Shortcuts
{
	public static class ProjectBrowserNavigatorShortcut
	{
		public class FolderStack
		{
			public List<string> folders = new();
			public int offset;
		}
		public class FolderStacksCollection
		{
			public Dictionary<int, FolderStack> stacks = new();
		}

		public const string NAMESPACE = "Custom/Project Browser";

		public const string SESSION_PREFS_KEY = nameof(ProjectBrowserNavigatorShortcut);

		public static FolderStacksCollection folderStacks = new();

		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			var folderStacksJson = UnityEditor.SessionState.GetString(SESSION_PREFS_KEY, null);
			folderStacks = JsonConvert.DeserializeObject<FolderStacksCollection>(folderStacksJson);

			EditorApplication.update -= OnUpdate_Safe;
			EditorApplication.update += OnUpdate_Safe;
		}

		public static void OnUpdate_Safe()
		{
			try
			{
				OnUpdate();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void OnUpdate()
		{
			var lastInteractedProjectBrowser = ProjectBrowserProxy.GetLastBrowser();

			if (lastInteractedProjectBrowser == null)
			{
				return;
			}

			var lastInteractedProjectBrowserInstanceId = lastInteractedProjectBrowser.GetInstanceID();

			var currentFolderStack = GetCurrentFolderStack(lastInteractedProjectBrowserInstanceId);
			var currentSelectedFolder = ProjectBrowserProxy.GetLastFolders(lastInteractedProjectBrowser).LastOrDefault();

			if (string.IsNullOrEmpty(currentSelectedFolder))
			{
				return;
			}

			if (currentFolderStack.offset != 0)
			{
				if (GetCurrentFolder(currentFolderStack) != currentSelectedFolder)
				{
					// Reset stack to here
					currentFolderStack.folders = currentFolderStack.folders.Skip(currentFolderStack.offset).ToList();

					currentFolderStack.folders.Insert(0, currentSelectedFolder);
					currentFolderStack.offset = 0;

					Save();
				}
			}
			else if (currentFolderStack.folders.FirstOrDefault() != currentSelectedFolder)
			{
				// Record as newest folder in stack
				currentFolderStack.folders.Insert(0, currentSelectedFolder);
				currentFolderStack.offset = 0;

				Save();
			}
		}

		public static FolderStack GetCurrentFolderStack(int instanceId)
		{
			if (folderStacks.stacks.TryGetValue(instanceId, out var stack) == false)
			{
				folderStacks.stacks[instanceId] = stack = new();
			}

			return stack;
		}

		public static void UpdateCurrentFolderStack(int amount)
		{
			var browser = ProjectBrowserProxy.GetLastBrowser();

			if (browser == null)
			{
				return;
			}

			var stack = GetCurrentFolderStack(browser.GetInstanceID());

			stack.offset += amount;

			SetCurrentFolder(browser, stack);

			Save();
		}

		public static void Save()
		{
			UnityEditor.SessionState.SetString(SESSION_PREFS_KEY, JsonConvert.SerializeObject(folderStacks));
		}

		public static string GetCurrentFolder(FolderStack stack)
		{
			if (stack.folders.Count == 0)
			{
				return null;
			}

			return stack.folders[stack.offset];
		}
		public static void SetCurrentFolder(EditorWindow browser, FolderStack stack)
		{
			if (browser.hasFocus == false)
			{
				browser.Focus();
			}

			stack.offset = Mathf.Clamp(stack.offset, 0, stack.folders.Count - 1);

			if (stack.folders.Count == 0)
			{
				return;
			}

			var targetFolder = GetCurrentFolder(stack);

			var lastFolderInstanceId = ProjectBrowserProxy.GetFolderInstanceID(targetFolder);

			ProjectBrowserProxy.SetFolderSelection(browser, new int[] { lastFolderInstanceId });
		}

		[Shortcut(NAMESPACE + "Backward", KeyCode.Mouse3)]
		public static void OnShortcut_Backward()
		{
			UpdateCurrentFolderStack(1);
		}

		[Shortcut(NAMESPACE + "Forward", KeyCode.Mouse4)]
		public static void OnShortcut_Forward()
		{
			UpdateCurrentFolderStack(-1);
		}
	}
}