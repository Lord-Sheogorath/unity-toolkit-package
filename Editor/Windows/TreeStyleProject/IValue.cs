using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public interface IValue
	{
		string Name { get; }
		Texture Icon { get; }

		bool IsValid();
		void Refresh();
		void Select();
		void OnGUI(Rect rect);
	}
}