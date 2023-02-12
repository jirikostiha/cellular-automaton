using Godot;
using System;

namespace CellularAutomaton.UI.Godot
{
	public partial class ControlPanel : HBoxContainer
	{

		private void _on_save_pressed()
		{
			var saveFileDialog = GetNode<FileDialog>("Save");
			saveFileDialog.Popup();
		}


		private void _on_load_pressed()
		{
			// Replace with function body.
		}
	}
}
