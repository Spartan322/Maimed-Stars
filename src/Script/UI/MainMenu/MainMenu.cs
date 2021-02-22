using System.Collections.Generic;
using Godot;
using MSG.Global;
using MSG.Script.UI.Base;
using MSG.Script.UI.Settings;
using SpartansLib.Attributes;

namespace MSG.Script.UI.MainMenu
{
    public class MainMenu : TextureRect, IMenuParent
    {
        [Node("MenuContainer/MenuPanel")] internal PanelContainer MenuPanel;

        [Node("MenuContainer/SettingsMenu")] SettingsMenu SettingsMenu;

        public List<BaseMenuPanel> _menuPanels = new List<BaseMenuPanel>();
        public IList<BaseMenuPanel> MenuPanels => _menuPanels;
        public override void _Ready()
        {
            //GD.Print(GetNode(PausePanelPath)?.ToString() ?? "Null"); // for some reason fails
            // PausePanel = GetNode<PanelContainer>("PausePanel"); // for some reason works fine
            foreach (var child in GetChild(0).GetChildren())
            {
                if (!(child is BaseMenuPanel panel)) continue;
                MenuPanels.Add(panel);
                panel.MenuParent = this;
            }
        }

        public bool IsPanelActive
        {
            get => MenuPanel.Visible;
            set
            {
                MenuPanel.Visible = value;
                if (!value) return;
                foreach (var panel in MenuPanels)
                    panel.Active = false;
                ActiveMenu = null;
            }
        }

        public BaseMenuPanel ActiveMenu { get; internal set; }

        public void RequestSetActiveMenu(BaseMenuPanel panel)
        {
            if (ActiveMenu == null && MenuPanels.Contains(panel))
                ActiveMenu = panel;
        }

        public void HandleCancelPress()
        {
            if (ActiveMenu != null) ActiveMenu.HandleCancelPress();
        }

        public void TerminateGame() => GetTree().Quit();

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/StartButton")]
        public void OnStartButtonPressed()
        {
            GetTree().ChangeScene("res://asset/godot-scene/World/GameDomain.tscn");
        }

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/TutorialButton")]
        public void OnTutorialButtonPressed()
        {
        }

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/SettingsButton")]
        public void OnSettingsButtonPressed()
        {
            SettingsMenu.Active = true;
        }

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/LoadButton")]
        public void OnLoadButtonPressed()
        {
        }

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/QuitButton")]
        public void OnQuitPressed()
        {
            TerminateGame();
        }
    }
}
