using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public class TSPJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (typeof(IValue)).IsAssignableFrom(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var jo = new JObject();

			var val = value as IValue;
			var type = value.GetType();

			var guid = TypeGuidUtil.GetGuid(type).guid;
			
			jo.Add("T", guid);
			jo.Add("V", JObject.FromObject(value));

			jo.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return new InvalidValue(null);
			}

			var jo = JObject.Load(reader);

			var guid = jo["T"].ToString();
			var json = jo["V"].ToString();

			try
			{
				var type = TypeGuidUtil.GetType(guid);

				var asset = JsonConvert.DeserializeObject(json, type);

				return asset as IValue;
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);

				return new InvalidValue(jo);
			}
		}
	}
}