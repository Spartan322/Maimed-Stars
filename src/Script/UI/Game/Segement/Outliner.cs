using Godot;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.UI.Game.Segement
{
	[Tool]
	public class Outliner : HBoxContainer
	{
		[Node] public PanelContainer OutlinerPanel;

		[Node("VBoxContainer/OutlinerToggle")] public Button OutlinerToggleButton;

		private bool toggle = true;

		[Export]
		public bool Toggle
		{
			get => toggle;
			set
			{
				toggle = value;
				if (OutlinerPanel != null)
				{
					OutlinerPanel.Visible = OutlinerToggleButton.SetToggleTextFor(!toggle);
					OutlinerPanel.Update();
					Update();
				}
			}
		}

		public override void _Ready()
		{
			Toggle = Toggle;
		}

		public void OnOutlinerToggleButtonPressed()
		{
			Toggle = !Toggle;
		}
	}
}
