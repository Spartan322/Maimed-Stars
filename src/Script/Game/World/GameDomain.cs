using System;
using System.Collections.Generic;
using Godot;
using MSG.Engine.Command;
using MSG.Engine;
using MSG.Game.Rts.World;
using MSG.Global;
using MSG.Script.Gui.Menu;
using SpartansLib.Attributes;
using SpartansLib.Extensions;

namespace MSG.Script.Game.World
{
    [Global]
    public class GameDomain : Node
    {
        public class SettingsClass
        {
            public int NpcCount = 1;
            public float InitialActionSpeed = 1;
            public float ActionSpeedSteps = 0.25f;
            public float MaxActionSpeed = 5f;
            public bool ActivateTutorial = false;
        }
        public SettingsClass Settings = new SettingsClass();

        public GameWorld GameWorld { get; private set; }
        public NationController Client { get; private set; }

        public delegate void GameDomainChangeAction<in T>(GameDomain domain, T arg);
        public event GameDomainChangeAction<float> OnGameSpeedChange;
        public event GameDomainChangeAction<bool> OnDebugModeChange;

        private List<NationController> _controllers = new List<NationController>();

        private float actionSpeed = 1;

        public float ActionSpeed
        {
            get => actionSpeed;
            set
            {
                actionSpeed = (float)GameMath.Clamp(GameMath.Stepify(value, Settings.ActionSpeedSteps), 0,
                    Settings.MaxActionSpeed);
                OnGameSpeedChange?.Invoke(this, actionSpeed);
            }
        }

        public bool IsActionPause => GameMath.IsZeroApprox(ActionSpeed);

        public bool DebugMode
        {
            get => (bool)ProjectSettings.GetSetting("extra_settings/game_settings/debug_mode");
            set => ProjectSettings.SetSetting("extra_settings/game_settings/debug_mode", value);
        }

        public override void _EnterTree()
        {
            if (GlobalScript.Singleton == null) new GlobalScript(GetViewport());
            if (GetTree().HasMeta("ConsoleWindow"))
                GetNode("UI/PrimaryCanvas/Canvas").AddChild((Node)GetTree().GetMeta("ConsoleWindow"));
            ActionSpeed = Settings.InitialActionSpeed;

            AddController(Client = new NationController(this, new NationController.SettingsClass
            {
                Name = (string)ProjectSettings.GetSetting("extra_settings/client_settings/player_name"),
                IsClient = true
            }));


            for (var npcCounter = Settings.NpcCount; npcCounter > -1; npcCounter--)
                AddController(new NationController(this));
        }

        public override void _ExitTree()
        {
            var consoleWindow = ((Node)GetTree().GetMeta("ConsoleWindow"));
            consoleWindow.GetParent().RemoveChild(consoleWindow);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (OS.IsDebugBuild() && Input.IsActionJustPressed("debug_enable_debug_mode"))
                OnDebugModeChange?.Invoke(this, DebugMode = !DebugMode);
        }

        public void AddActionSpeed(int steps = 1)
            => ActionSpeed += Settings.ActionSpeedSteps * steps;

        public void SubActionSpeed(int steps = 1)
            => ActionSpeed -= Settings.ActionSpeedSteps * steps;

        private float lastSpeed;

        public void ToggleActionSpeed()
        {
            if (IsActionPause)
            {
                if (GameMath.IsZeroApprox(lastSpeed))
                    AddActionSpeed();
                else
                {
                    ActionSpeed = lastSpeed;
                    lastSpeed = 0;
                }
            }
            else
            {
                lastSpeed = ActionSpeed;
                ActionSpeed = 0;
            }
        }

        private void _onControllerSet(NationController item)
        {

        }

        public void AddController(NationController item)
        {
            if (_controllers.Contains(item)) return;
            _onControllerSet(item);
            _controllers.Add(item);
        }
    }
}
