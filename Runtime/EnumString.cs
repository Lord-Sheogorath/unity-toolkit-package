using System;

namespace LordSheo
{
	public struct EnumString
	{
		public string value;

		public static implicit operator EnumString(string val)
		{
			return new()
			{
				value = val,
			};
		}

		public static implicit operator EnumString(Enum val)
		{
			return new()
			{
				value = val.ToString()
			};
		}

		public static implicit operator string(EnumString val)
		{
			return val.value;
		}
	}
}