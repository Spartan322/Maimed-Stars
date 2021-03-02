using Godot;
using MSG.Engine;
using MSG.Gui.Menu;
using MSG.Script.Gui.Menu.Settings;
using MSG.Script.Gui.Window;
using SpartansLib.Attributes;

namespace MSG.Script.Gui.Menu
{
    public class MainMenu : ParentPanel
    {
        [Node("MenuContainer/MenuPanel")]
        protected override PanelContainer Panel { get; set; }

        [Node("SettingsWindow/SettingsMenu")]
        protected override SettingsMenu SettingsMenu { get; set; }

        public override void _EnterTree()
        {
            if (GetTree().HasMeta("ConsoleWindow"))
            {
                var oldConsoleWindow = GetNode("ConsoleWindow");
                RemoveChild(oldConsoleWindow);
                oldConsoleWindow.QueueFree();
                AddChild((Node)GetTree().GetMeta("ConsoleWindow"));
            }
        }

        public void TerminateGame() => GetTree().Quit();

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/StartButton")]
        public void OnStartButtonPressed()
        {
            var consoleWindow = GetNode<BaseWindow>("ConsoleWindow");
            GetTree().CurrentScene.RemoveChild(consoleWindow);
            GetTree().SetMeta("ConsoleWindow", consoleWindow);
            GetTree().ChangeScene("res://asset/godot-scene/Game/World/GameDomain.tscn");
        }

        [Connect("pressed", "MenuContainer/MenuPanel/HBoxContainer/VBoxContainer/ButtonList/TutorialButton")]
        public void OnTutorialButtonPressed()
        {
            OnStartButtonPressed();
            GetTree().SetMeta("ActivateTutorial", true);
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
