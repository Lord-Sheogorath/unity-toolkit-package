using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector.Editor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	[TypeGuid("E00B51D6-14E7-4ABB-A1E6-3DC509A3EB6D")]
	public class InvalidValue : ITreeStyleValue
	{
		[JsonIgnore]
		public string Name => "INVALID: " + (json != null ? json["T"].ToString() : "MISSING");
		
		[JsonIgnore]
		public Texture Icon { get; }

		private readonly JObject json;
		
		public InvalidValue(JObject json)
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

		public void OnGUI(Rect rect)
		{
		}

		public IEnumerable<GenericSelectorItem<System.Action>> GetContextActions()
		{
			yield break;
		}
	}
}