using Godot;
using SpartansLib.Attributes;

namespace MSG.Script.UI.Game
{
	[Global]
	public class InterfaceCore : Control
	{
		[Export] public float GameSpeedSteps = 0.25f;

		public override void _GuiInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton mb && mb.IsPressed())
				GetFocusOwner()?.ReleaseFocus();
		}
	}
}
