#if LORD_SHEO_ODIN_ENABLED
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.UI
{
	public static class OdinUtil
	{
		public static Color DefaultBackgroundColor => SirenixGUIStyles.DarkEditorBackground;
		public static Color MouseHoverOverlayColor => SirenixGUIStyles.MouseOverBgOverlayColor;
		
		public static Color GetSelectedColor(this OdinMenuStyle style, bool active)
		{
			return active ? style.SelectedColor : style.SelectedInactiveColor;
		}

		public static Color GetSelectedColor(this OdinMenuItem item)
		{
			var active = item.MenuTree == OdinMenuTree.ActiveMenuTree;
			return item.Style.GetSelectedColor(active);
		}

		public static Color GetCurrentBackgroundColor(this OdinMenuItem item)
		{
			if (item.IsSelected)
			{
				return item.GetSelectedColor();
			}

			return item.IsMouseHovering() ? MouseHoverOverlayColor : DefaultBackgroundColor;
		}
		public static GUIStyle GetCurrentLabelStyle(this OdinMenuItem item)
		{
			return item.IsSelected ? item.Style.SelectedLabelStyle : item.Style.DefaultLabelStyle;
		}

		public static bool IsMouseHovering(this OdinMenuItem item)
		{
			if (item.IsEnabled == false)
			{
				return false;
			}
			
			if (item.IsSelectable == false)
			{
				return false;
			}

			if (item.IsSelected)
			{
				return false;
			}

			return item.Rect.Contains(Event.current.mousePosition);
		}

		public static void DrawOverContent(OdinMenuItem item, GUIContent content)
		{
			var color = item.GetCurrentBackgroundColor();
			DrawOverContent(item, content, color);
		}
		public static void DrawOverContent(OdinMenuItem item, GUIContent content, Color background)
		{
			var style = item.GetCurrentLabelStyle();

			var rect = item.LabelRect;

			// NOTE: Hover color has low alpha so we need to draw
			// the default color first so that it doesn't show the
			// original text underneath.
			EditorGUI.DrawRect(rect, OdinUtil.DefaultBackgroundColor);
			EditorGUI.DrawRect(rect, background);
			EditorGUI.LabelField(rect, content, style);
		}
	}
}
#endif