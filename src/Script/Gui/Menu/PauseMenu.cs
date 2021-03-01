using Godot;
using MSG.Global;
using MSG.Gui.Menu;
using MSG.Script.Game;
using MSG.Script.Gui.Game.Select;
using MSG.Script.Gui.Menu.Settings;
using MSG.Script.UI.Base;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Structure;

namespace MSG.Script.Gui.Menu
{
    public class PauseMenu : ParentPanel
    {
        public static bool FocusOwnerExist { get; private set; } = false;

        [Export]                                   //  rr gg bb aa
        public Color Background = ColorExt.FromRGBA8(0x00_00_00_3c);

        [Node("PauseContainer/PausePanel")]
        protected override PanelContainer Panel { get; set; }

        [Node("PauseContainer/SettingsMenu")]
        protected override SettingsMenu SettingsMenu { get; set; }

        public bool GamePaused
        {
            get => Visible;
            set
            {
                if (GamePaused == value) return;
                Visible = value;
                GetTree().Paused = value;
                if (!value) return;
                RtsCamera.CurrentCamera.Reset();
                NodeRegistry.Get<SelectBox>().Reset();
                IsPanelActive = true;
            }
        }

        public override bool HandleCancelPress()
        {
            if (!GamePaused) GamePaused = true;
#pragma warning disable 642
            else if (base.HandleCancelPress()) ;
#pragma warning restore 642
            else OnCancel();
            return true;
        }

        public void ExitToMenu()
        {
            GetTree().ChangeScene("res://asset/godot-scene/Gui/Menu/MainMenu.tscn");
            GetTree().Paused = false;
        }

        public void TerminateGame() => GetTree().Quit();
        private void OnCancel() => GamePaused = false;

        public override void _Draw()
        {
            DrawRect(GetRect(), Background);
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/ResumeButton")]
        public void OnResumeButtonPressed()
        {
            GamePaused = false;
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/SettingsButton")]
        public void OnSettingsButtonPressed()
        {
            SettingsMenu.Active = true;
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/SaveButton")]
        public void OnSaveButtonPressed()
        {
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/LoadButton")]
        public void OnLoadButtonPressed()
        {
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/ExitButton")]
        public void OnExitButtonPressed()
        {
            ExitToMenu();
        }

        [Connect("pressed", "PauseContainer/PausePanel/HBoxContainer/VBoxContainer/DesktopButton")]
        public void OnDesktopButtonPressed()
        {
            TerminateGame();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.PauseKeyIsJustPressed())
            {
                if (!NodeRegistry.Get<SelectionDisplay>().Visible)
                {
                    AcceptEvent();
                    HandleCancelPress();
                }
            }

            FocusOwnerExist = GetFocusOwner() != null;
        }
    }
}
