using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LordSheo.Editor
{
	public class EditorSettingsProvider<T> : SettingsProvider, IDisposable
	{
		public PropertyTree<T> tree;
		public bool isDirty = false;

		public readonly IDefaultEditorSettings<T> defaults;

		public T Settings => EditorSettings.GetSettings(defaults);

		private EditorSettingsProvider(string name, IDefaultEditorSettings<T> defaults, IEnumerable<string> keywords)
			: base(name, SettingsScope.User, keywords)
		{
			this.defaults = defaults;
		}

		public static EditorSettingsProvider<T> Create(IDefaultEditorSettings<T> defaults, IEnumerable<string> keywords = null)
		{
			var name = ConstValues.NAMESPACE_PATH + typeof(T).Name.Replace("Settings", "");

			return new(name, defaults, keywords);
		}

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			base.OnActivate(searchContext, rootElement);

			Refresh();
		}

		protected virtual void Refresh()
		{
			Dispose();

			tree = new(new T[] { Settings });
			tree.OnPropertyValueChanged += OnPropertyValueChanged;
		}

		protected virtual void OnPropertyValueChanged(InspectorProperty prop, int index)
		{
			isDirty = true;
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();

			Dispose();
		}

		public override void OnTitleBarGUI()
		{
			base.OnTitleBarGUI();
			DrawToolbar();
		}

		public override void OnGUI(string searchContext)
		{
			base.OnGUI(searchContext);

			tree.Draw(false);

			if (isDirty)
			{
				EditorSettings.SetSettings(Settings);
				isDirty = false;
			}
		}

		private void DrawToolbar()
		{
			if (SirenixEditorGUI.ToolbarButton("Import"))
			{
				var confirm = EditorUtility.DisplayDialog("Import Settings?", "", "Confirm", "Cancel");

				if (confirm)
				{
					EditorSettings.SetSerialisedSettings<T>(GUIUtility.systemCopyBuffer);
					Refresh();
				}
			}
			if (SirenixEditorGUI.ToolbarButton("Export"))
			{
				GUIUtility.systemCopyBuffer = EditorSettings.GetSerialisedSettings(defaults);
			}

			if (SirenixEditorGUI.ToolbarButton("Reset"))
			{
				var confirm = EditorUtility.DisplayDialog("Reset Settings?", "", "Confirm", "Cancel");

				if (confirm)
				{
					EditorSettings.SetSettings(defaults.Create());
					Refresh();
				}
			}
		}

		public void Dispose()
		{
			if (tree == null)
			{
				return;
			}

			tree.OnPropertyValueChanged -= OnPropertyValueChanged;
			tree.Dispose();
		}
	}
}