using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace LordSheo.Editor.Core
{
    public class EditorSettingsPreferencesProvider
    {
        [SettingsProviderGroup]
        public static SettingsProvider[] GetSettings()
        {
            var settings = Resources.LoadAll<EditorSettingsAsset>("LordSheo/Settings");
            var providers = new SettingsProvider[settings.Length];

            for (int i = 0; i < settings.Length; i++)
            {
                var asset = settings[i];
                var provider = providers[i] = new($"LordSheo/Settings/{asset.name}", SettingsScope.User);
                PropertyTree tree = null;

                provider.activateHandler = (_, _) =>
                {
                    tree = PropertyTree.Create(asset);
                };
                provider.guiHandler = _ =>
                {
                    tree.Draw();
                };
                provider.deactivateHandler = () =>
                {
                    tree?.Dispose();
                };
            }
            
            return providers;
        }
    }
}
