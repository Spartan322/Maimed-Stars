using Godot;
using SpartansLib.Attributes;

namespace MSG.Script.Gui.Game.Frontend
{
    [Tool]
    public class MusicPlayer : PanelContainer
    {
        [Node("MusicArrowHBox/ArrowButton")] public Button ArrowButton;

        [Node("MusicArrowHBox/MusicHContainer")]
        public HBoxContainer MusicHContainer;

        public override void _Ready()
        {
            MusicHContainer.Visible = false;
            ArrowButton.Text = "<";
        }

        public void OnArrowButtonToggled(bool pressed)
        {
            ArrowButton.Text = pressed ? "<" : ">";
            MusicHContainer.Visible = !pressed;
        }
    }
}
