using Godot;
using MSG.Script.UI.Base;
using MSG.Script.UI.Settings.Tab;
using SpartansLib.Attributes;

namespace MSG.Script.UI.Settings
{
    public class SettingsMenu : BaseMenuPanel
    {
        [Node("VBoxContainer/SettingsTabs/Game")]
        public HBoxContainer GameTab;

        [Node("VBoxContainer/SettingsTabs/Video")]
        public HBoxContainer VideoTab;

        [Node("VBoxContainer/SettingsTabs/Audio")]
        public HBoxContainer AudioTab;

        [Node("VBoxContainer/SettingsTabs/Controls")]
        public Controls ControlsTab;

        [Node("VBoxContainer/TopWindowDecoration")]
        public TopWindowDecoration TopWindowDecoration;

        [Node("RebindCenter/RebindDialog")] public RebindPopupDialog RebindDialog;

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
