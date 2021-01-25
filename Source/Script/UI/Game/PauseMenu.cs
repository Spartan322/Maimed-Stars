using System.Collections.Generic;
using Godot;
using MSG.Global;
using MSG.Script.UI.Base;
using MSG.Script.UI.Settings;
using SpartansLib;
using SpartansLib.Attributes;
using SpartansLib.Structure;

namespace MSG.Script.UI.Game
{
    public class PauseMenu : ColorRect
    {
        public static bool FocusOwnerExist { get; private set; } = false;

        [Export]                                   //  rr gg bb aa
        public Color Background = ColorExt.FromRGBA8(0x00_00_00_3c);

        [Node("PauseContainer/PausePanel")] internal PanelContainer PausePanel;

        [Node("PauseContainer/SettingsMenu")] SettingsMenu SettingsMenu;

        internal readonly List<BasePauseMenuPanel> MenuPanels = new List<BasePauseMenuPanel>();

        public override void _Ready()
        {
            //GD.Print(GetNode(PausePanelPath)?.ToString() ?? "Null"); // for some reason fails
            // PausePanel = GetNode<PanelContainer>("PausePanel"); // for some reason works fine
            foreach (var child in GetChild(0).GetChildren())
            {
                if (!(child is BasePauseMenuPanel panel)) continue;
                MenuPanels.Add(panel);
                panel.MenuParent = this;
            }
        }

        public bool GamePaused
        {
            get => Visible;
            set
            {
                if (Visible == value) return;
                Visible = value;
                GetTree().Paused = value;
                if (!value) return;
                Camera.CurrentCamera.Reset();
                NodeRegistry.Get<SelectionBox>().Reset();
                PausePanelActive = true;
            }
        }

        public bool PausePanelActive
        {
            get => PausePanel.Visible;
            set
            {
                PausePanel.Visible = value;
                if (!value) return;
                foreach (var panel in MenuPanels)
                    panel.Active = false;
                ActiveMenu = null;
            }
        }

        public BasePauseMenuPanel ActiveMenu { get; internal set; }

        public void HandleCancelPress()
        {
            if (!GamePaused) GamePaused = true;
            else if (ActiveMenu != null) ActiveMenu.HandleCancelPress();
            else OnCancel();
        }

        public void ExitToMenu()
        {
        }

        public void TerminateGame() => GetTree().Quit();
        private void OnCancel() => GamePaused = false;

        public override void _Draw()
        {
            DrawRect(GetRect(), Background);
        }

        [Connect("pressed","PauseContainer/PausePanel/HBoxContainer/VBoxContainer/ResumeButton")]
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
                if (!NodeRegistry.Get<SelectionMenu>().Visible)
                {
                    AcceptEvent();
                    HandleCancelPress();
                }
            }

            FocusOwnerExist = GetFocusOwner() != null;
        }
    }
}
