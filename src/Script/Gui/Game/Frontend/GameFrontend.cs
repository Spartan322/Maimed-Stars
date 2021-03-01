using Godot;
using MSG.Script.Gui.Window;
using SpartansLib.Attributes;

namespace MSG.Script.Gui.Game.Frontend
{
    [Global]
    public class GameFrontend : Control
    {
        [Node]
        private Control GameMenuList;

        [Export] public float GameSpeedSteps = 0.25f;

        public override void _Ready()
        {
            if (!GetTree().HasMeta("ActivateTutorial")) return;
            var popup = GameMenuList.GetNode<BaseWindow>("TutorialPopup");
            // popup.TextLabel.Text = "Tutorial";
            // popup.Popup_();
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mb && mb.IsPressed())
                GetFocusOwner()?.ReleaseFocus();
        }
    }
}
