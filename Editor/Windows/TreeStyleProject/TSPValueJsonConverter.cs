#if LORD_SHEO_ODIN_ENABLED
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public class TSPValueJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (typeof(ITreeStyleValue)).IsAssignableFrom(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var jObject = new JObject();

			var type = value.GetType();
			var guid = TypeGuidUtil.GetGuid(type).guid;
			
			jObject.Add("T", guid);
			jObject.Add("V", JObject.FromObject(value));

			jObject.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return new TSPInvalidValue(null);
			}

			var jObject = JObject.Load(reader);

			var guid = jObject["T"].ToString();
			var json = jObject["V"].ToString();

			try
			{
				var type = TypeGuidUtil.GetType(guid);
				var value = JsonConvert.DeserializeObject(json, type);

				return value as ITreeStyleValue;
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);

				return new TSPInvalidValue(jObject);
			}
		}
	}
}
#endif