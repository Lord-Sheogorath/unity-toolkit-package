#if LORD_SHEO_ODIN_ENABLED
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	[TypeGuid("E00B51D6-14E7-4ABB-A1E6-3DC509A3EB6D")]
	public class TSPInvalidValue : ITreeStyleValue
	{
		[JsonIgnore]
		public string Name => "INVALID: " + (json != null ? json["T"].ToString() : "MISSING");

		[JsonIgnore]
		public Texture Icon => EditorGUIUtility.IconContent("Folder Icon").image;

		public event Action ModifiedCallback;
		
		private readonly JObject json;
		
		public TSPInvalidValue(JObject json)
		{
			this.json = json;
		}
		
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

		public void OnGUI(Rect rect)
		{
		}

		public IEnumerable<GenericSelectorItem<System.Action>> GetContextActions()
		{
			yield break;
		}
	}
}
#endif