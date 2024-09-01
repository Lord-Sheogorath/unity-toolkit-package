using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Tools
{
	public class AssetModificationConfirmation : UnityEditor.AssetModificationProcessor
	{
		public enum Type
		{
			Move,
			Rename,
		}

		private static AssetModificationConfirmationSettings Settings => AssetModificationConfirmationSettings.Instance;

		private static DateTime previousConfirmationTimestamp = DateTime.MinValue;
		private static DateTime previousCancelTimestamp = DateTime.MinValue;

		public static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			if (Settings == null || Settings.enabled == false)
			{
				return AssetMoveResult.DidNotMove;
			}

			if (Settings.DisableSessionConfirmation)
			{
				return AssetMoveResult.DidNotMove;
			}

			try
			{
				var type = GetModificationType(sourcePath, destinationPath);

				if (type == Type.Rename && Settings.renameConfirmation == false)
				{
					return AssetMoveResult.DidNotMove;
				}

				var showConfirmation = false;

				if (Settings.folderConfirmation)
				{
					showConfirmation = AssetDatabase.IsValidFolder(sourcePath);
				}
				if (Settings.fileConfirmation)
				{
					showConfirmation |= AssetDatabase.GUIDFromAssetPath(sourcePath) != default;
				}

				var result = AssetMoveResult.DidNotMove;

				var window = EditorWindow.focusedWindow;

				if (showConfirmation)
				{
					result = ShowConfirmation(sourcePath, destinationPath, type);
				}

				window.Focus();

				return result;
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}

			return AssetMoveResult.DidNotMove;
		}

		private static Type GetModificationType(string source, string destination)
		{
			var sourceSlashIndex = source.LastIndexOf('/');
			var sourcePath = source.Substring(0, sourceSlashIndex);

			var destinationSlashIndex = destination.LastIndexOf('/');
			var destinationPath = destination.Substring(0, destinationSlashIndex);

			var isRename = sourcePath == destinationPath;

			return isRename ? Type.Rename : Type.Move;
		}
		private static AssetMoveResult ShowConfirmation(string source, string destination, Type type)
		{
			var now = DateTime.UtcNow;

			var timeSincePreviousCancel = now - previousCancelTimestamp;

			if (timeSincePreviousCancel.TotalSeconds < Settings.confirmationCancelInterval)
			{
				return AssetMoveResult.FailedMove;
			}

			var timeSincePreviousConfirmation = now - previousConfirmationTimestamp;

			if (timeSincePreviousConfirmation.TotalSeconds <= Settings.confirmationAcceptInterval)
			{
				return AssetMoveResult.DidNotMove;
			}

			var selection = Selection.assetGUIDs
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.OrderBy(path => path)
				.ToArray();

			if (selection.Length == 0)
			{
				selection = new string[] { source };
			}

			var prompt = string.Join("\n", selection);

			bool isConfirmed = EditorUtility.DisplayDialog($"Confirm {type}", prompt + "\n\nAre you sure?", "Yes", "No");

			// We don't know how long the popup was open for
			now = DateTime.UtcNow;

			if (isConfirmed == false)
			{
				previousCancelTimestamp = now;
				return AssetMoveResult.FailedMove;
			}

			previousConfirmationTimestamp = now;

			return AssetMoveResult.DidNotMove;
		}
	}
}