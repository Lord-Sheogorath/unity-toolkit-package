using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace LordSheo
{
	public class SemanticVersionConverter : JsonConverter<SemanticVersion>
	{
		public override void WriteJson(JsonWriter writer, SemanticVersion value, JsonSerializer serializer)
		{
			// Serialize as "major.minor.patch"
			writer.WriteValue(value.Version);
		}

		public override SemanticVersion ReadJson(JsonReader reader, Type objectType, SemanticVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			// Expect a string like "1.2.3"
			var versionStr = reader.Value?.ToString();
			if (string.IsNullOrWhiteSpace(versionStr))
			{
				throw new JsonSerializationException("Expected a version string like '1.2.3'");
			}

			return new SemanticVersion(versionStr);
		}
	}
	
	[JsonConverter(typeof(SemanticVersionConverter))]
	[System.Serializable]
	public class SemanticVersion
	{
		public int major;
		public int minor;
		public int patch;

		[ShowInInspector]
		public string Version => $"{major}.{minor}.{patch}";
		
		public SemanticVersion(string value)
		{
			var split = value.Split('.');

			if (split.Length != 3)
			{
				throw new System.ArgumentException("Input version does not contain 3 segments");
			}
			
			major = int.Parse(split[0]);
			minor = int.Parse(split[1]);
			patch = int.Parse(split[2]);
		}
		public SemanticVersion(int major, int minor, int patch)
		{
			this.major = major;
			this.minor = minor;
			this.patch = patch;
		}

		public void AddMajor(int amount)
		{
			major += amount;
			minor = 0;
			patch = 0;
		}
		public void AddMinor(int amount)
		{
			minor += amount;
			patch = 0;
		}
		public void AddPatch(int amount)
		{
			patch += amount;
		}

		public static SemanticVersion Max(SemanticVersion input, SemanticVersion other)
		{
			if (input.major > other.major)
			{
				return input;
			}
			else if (input.major < other.major)
			{
				return other;
			}
			
			if (input.minor > other.minor)
			{
				return input;
			}
			else if (input.minor < other.minor)
			{
				return other;
			}
			
			if (input.patch > other.patch)
			{
				return input;
			}
			else if (input.patch < other.patch)
			{
				return other;
			}

			// Both values the same, return first given value.
			return input;
		}
		public static SemanticVersion Min(SemanticVersion input, SemanticVersion other)
		{
			if (input.major > other.major)
			{
				return other;
			}
			else if (input.major < other.major)
			{
				return input;
			}
			
			if (input.minor > other.minor)
			{
				return other;
			}
			else if (input.minor < other.minor)
			{
				return input;
			}
			
			if (input.patch > other.patch)
			{
				return other;
			}
			else if (input.patch < other.patch)
			{
				return input;
			}

			// Both values the same, return first given value.
			return input;
		}

		public static bool Equal(SemanticVersion input, SemanticVersion other)
		{
			return input.major == other.major
				&& input.minor == other.minor
				&& input.patch == other.patch;
		}
	}
}