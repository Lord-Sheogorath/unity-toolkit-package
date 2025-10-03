#if LORD_SHEO_ODIN_ENABLED
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	[TypeGuid("25D85997-0CAB-42B4-A720-7D44918C1BD9")]
	public class TSPCustomValue : ITreeStyleValue
	{
		public string name = System.Guid.NewGuid().ToString();

		[JsonIgnore]
		public string Name => name;

		[JsonIgnore]
		public Texture Icon => EditorGUIUtility.FindTexture("Folder Icon");

		public event Action ModifiedCallback;

		public bool IsValid()
		{
			return true;
		}

		public void Refresh()
		{
		}

		public void Select()
		{
		}

		public void Deselect()
		{
		}

		private bool isRenameActive;
		private string nameBeforeRename;

		private void CompleteRename(bool revert)
		{
			if (isRenameActive == false)
			{
				return;
			}

			isRenameActive = false;

			if (revert)
			{
				name = nameBeforeRename;
				return;
			}

			ModifiedCallback?.Invoke();
		}

		public void OnGUI(Rect rect)
		{
			HandleRenameGUI(rect);
			HandleRenameInput(rect);
		}

		private void HandleRenameGUI(Rect rect)
		{
			if (isRenameActive == false)
			{
				return;
			}

			EditorGUI.DrawRect(rect, Color.black);

			GUI.SetNextControlName("tsp_custom_value_rename_field");
			name = EditorGUI.TextField(rect
				.AddXMin(16)
				.AddXMax(-100), name, EditorStyles.label);
			EditorGUI.FocusTextInControl("tsp_custom_value_rename_field");

			if (GUI.Button(rect
					    .AlignRight(24)
					    .AddX(-4)
					    .SetHeight(24)
					    .SetCenterY(rect.center.y)
				    , EditorIcons.Checkmark.Active))
			{
				CompleteRename(false);
			}

			if (GUI.Button(rect
					    .AlignRight(24)
					    .AddX(-32)
					    .SetHeight(24)
					    .SetCenterY(rect.center.y)
				    , EditorIcons.X.Active))
			{
				CompleteRename(true);
			}
		}

		private void HandleRenameInput(Rect rect)
		{
			if (isRenameActive)
			{
				if (Event.current.IsKeyUp(KeyCode.KeypadEnter))
				{
					CompleteRename(false);
					Event.current.Use();
				}
				else if (Event.current.IsKeyDown(KeyCode.Escape))
				{
					CompleteRename(true);
					Event.current.Use();
				}
				else if (rect.Contains(Event.current.mousePosition) == false && Event.current.clickCount > 0)
				{
					CompleteRename(true);
				}

				if (Event.current.IsKeyboardEvent() || Event.current.IsMouseEvent())
				{
					Event.current.Use();
				}

				return;
			}

			// Make sure they're left clicking
			if (Event.current.button != 0)
			{
				return;
			}

			if (rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			if (Event.current.clickCount == 1 && Event.current.alt)
			{
				isRenameActive = true;
				nameBeforeRename = name;

				Event.current.Use();
			}
		}

		public IEnumerable<GenericSelectorItem<System.Action>> GetContextActions()
		{
			yield break;
		}
	}
}
#endif