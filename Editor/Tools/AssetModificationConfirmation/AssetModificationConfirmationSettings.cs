﻿#if LORD_SHEO_ODIN_ENABLED
using Sirenix.OdinInspector;
#endif

using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Tools
{
	public class AssetModificationConfirmationSettings : EditorSettingsAsset
	{
		private const string DISABLE_SESSION_CONFIRMATION = "AMCS_DisableSessionConfirmation";

		public static AssetModificationConfirmationSettings Instance => EditorSettings.GetSettings<AssetModificationConfirmationSettings>();

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
	}
}