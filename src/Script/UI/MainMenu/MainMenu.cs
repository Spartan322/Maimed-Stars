using System.Collections.Generic;
using Godot;
using MSG.Global;
using MSG.Script.UI.Base;
using MSG.Script.UI.Game;
using MSG.Script.UI.Settings;
using SpartansLib.Attributes;

namespace MSG.Script.UI.MainMenu
{
    public class MainMenu : MenuParent
    {
        [Node("MenuContainer/MenuPanel")]
        protected override PanelContainer Panel { get; set; }

        [Node("MenuContainer/SettingsMenu")]
        protected override SettingsMenu SettingsMenu { get; set; }

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

        public override void _Input(InputEvent @event)
        {
            if (@event.PauseKeyIsJustPressed())
            {
                AcceptEvent();
                HandleCancelPress();
            }
        }
    }
}
