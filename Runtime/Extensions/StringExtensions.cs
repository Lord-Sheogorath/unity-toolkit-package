using System.Collections;

namespace LordSheo
{
	public static class StringExtensions
	{
		public static string ToTitleCase(this string source, bool isUpperCase = false)
		{
			if (isUpperCase)
			{
				source = source.ToLower();
			}
			
			var culture = System.Globalization.CultureInfo.CurrentCulture;
			return culture.TextInfo.ToTitleCase(source);
		}

		public static bool IsNullOrEmpty(this string source)
		{
			return string.IsNullOrEmpty(source);
		}
	}
}