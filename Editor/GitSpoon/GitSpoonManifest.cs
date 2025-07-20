using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace LordSheo.Editor.GitSpoon
{
	[System.Serializable]
	public class GitSpoonManifest
	{
		[System.Serializable]
		public class Dependency
		{
			public enum Source
			{
				Unity,
				Internal,
				Git,
			}
			
			public string id;
			public Source source;
			public SemanticVersion minVersion;
		}

		public string name;
		public string displayName;
		public SemanticVersion version;
		public string description;
		public string author;
			
		public string url;

		[ListDrawerSettings(ListElementLabelName = nameof(Dependency.id))]
		public List<Dependency> dependencies;
	}
}