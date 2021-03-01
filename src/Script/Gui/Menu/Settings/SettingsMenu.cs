using Godot;
using MSG.Gui.Menu;
using MSG.Script.Gui.Window;
using SpartansLib.Attributes;
using static MSG.Script.Gui.Window.BaseWindow;

namespace MSG.Script.Gui.Menu.Settings
{
    public class SettingsMenu : ChildPanel
    {
        [Node("SettingsTabs/Game")]
        public HBoxContainer GameTab;

        [Node("SettingsTabs/Video")]
        public HBoxContainer VideoTab;

        [Node("SettingsTabs/Audio")]
        public HBoxContainer AudioTab;

        [Node("SettingsTabs/Controls")]
        public ControlsTab ControlsTab;

        [Node("RebindCenter/RebindDialog")]
        public RebindDialog RebindDialog;

        private BaseWindow _panelControl;
        public override Control PanelControl => _panelControl;

        public override void _Ready()
        {
            _panelControl = GetParent<BaseWindow>();
            _panelControl.OnButtonPressed += (window, button) =>
            {
                if (button.GetIndex() == (int)WindowButton.Close) OnExitButtonPressed();
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
