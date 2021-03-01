using Godot;
using MSG.Gui.Menu;
using MSG.Script.UI.Base;
using SpartansLib.Attributes;

namespace MSG.Script.Gui.Menu.Settings
{
    public class SettingsMenu : ChildPanel
    {
        [Node("VBoxContainer/SettingsTabs/Game")]
        public HBoxContainer GameTab;

        [Node("VBoxContainer/SettingsTabs/Video")]
        public HBoxContainer VideoTab;

        [Node("VBoxContainer/SettingsTabs/Audio")]
        public HBoxContainer AudioTab;

        [Node("VBoxContainer/SettingsTabs/Controls")]
        public ControlsTab ControlsTab;

        [Node("VBoxContainer/TopWindowDecoration")]
        public TopWindowDecoration TopWindowDecoration;

        [Node("RebindCenter/RebindDialog")] public RebindDialog RebindDialog;

        public override void _Ready()
        {
            TopWindowDecoration.OnButtonPressed += (sender, args) =>
            {
                if (args.ButtonType.IsExit()) OnExitButtonPressed();
            };
            ControlsTab.RebindDialog = RebindDialog;
        }

        public void GenerateGameSettingsList()
        {
            var gameVBox = GameTab.GetNode<VBoxContainer>("ScrollContainer/VBox");
        }

        public void OnExitButtonPressed()
        {
            MenuParent.HandleCancelPress();
        }

        public override string ToString()
            => "SettingsMenu" + GetInstanceId();
    }
}
