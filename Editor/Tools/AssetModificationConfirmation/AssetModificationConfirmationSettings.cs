using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Tools
{
	public class AssetModificationConfirmationSettings
	{
		private struct DefaultSettings : IDefaultEditorSettings<AssetModificationConfirmationSettings>
		{
			public AssetModificationConfirmationSettings Create()
			{
				return new();
			}
		}

		private const string DISABLE_SESSION_CONFIRMATION = "AMCS_DisableSessionConfirmation";

		public static AssetModificationConfirmationSettings Instance => EditorSettings.GetSettings(new DefaultSettings());

		public bool enabled = true;

		[Space]

		[PropertyOrder(1)]
		public bool folderConfirmation = true;
		[PropertyOrder(1)]
		public bool fileConfirmation = true;
		[PropertyOrder(1)]
		public bool renameConfirmation = true;

		[Space]

		[PropertyOrder(2)]
		public int confirmationCancelInterval = 1;
		[PropertyOrder(2)]
		public int confirmationAcceptInterval = 1;

		[ShowInInspector, JsonIgnore]
		[PropertyOrder(1)]
		public bool DisableSessionConfirmation
		{
			get => SessionState.GetBool(DISABLE_SESSION_CONFIRMATION, false);
			set => SessionState.SetBool(DISABLE_SESSION_CONFIRMATION, value);
		}

		[SettingsProvider]
		private static SettingsProvider GetSettings()
		{
			return EditorSettingsProvider<AssetModificationConfirmationSettings>.Create(new DefaultSettings());
		}
	}
}