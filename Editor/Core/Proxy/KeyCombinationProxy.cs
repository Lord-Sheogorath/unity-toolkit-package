using System.Reflection;
using UnityEditor.ShortcutManagement;

namespace LordSheo.Editor
{
	public static class KeyCombinationProxy
	{
		public static MethodInfo tryParseMenuItemBindingStringMethod;

		static KeyCombinationProxy()
		{
			var type = typeof(KeyCombination);

			tryParseMenuItemBindingStringMethod = type
				.FindMethodInfo_Static("TryParseMenuItemBindingString");
		}

		public static bool TryParseMenuItemBindingString(string menuItemBindingString, out KeyCombination keyCombination)
		{
			var @params = new object[] { menuItemBindingString, default(KeyCombination) };
			var valid = (bool)tryParseMenuItemBindingStringMethod.Invoke(null, @params);

			keyCombination = (KeyCombination)@params[1];

			return valid;
		}
	}
}
