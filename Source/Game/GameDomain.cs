using System.Collections;
using System.Collections.Generic;
using Godot;
using MSG.Game.Controller;

namespace MSG.Game
{
    public class GameDomainSettings
    {
        public int NpcCount { get; } = 1;
        public float InitialActionSpeed { get; } = 1;
        public float ActionSpeedSteps { get; } = 0.25f;
        public float MaxActionSpeed { get; } = 5f;
    }

    public class GameDomain
    {
        public GameDomainSettings Settings { get; }

        public GameWorld GameWorld { get; }
        public Script.Game Node { get; private set; }

        public delegate void GameDomainChangeAction<T>(GameDomain domain, T arg);

        public event GameDomainChangeAction<float> OnGameSpeedChange;

        private List<GameNationController> _controllers = new List<GameNationController>();

        private float actionSpeed = 1;

        public float ActionSpeed
        {
            get => actionSpeed;
            set
            {
                actionSpeed = (float) GameMath.Clamp(GameMath.Stepify(value, Settings.ActionSpeedSteps), 0,
                    Settings.MaxActionSpeed);
                OnGameSpeedChange?.Invoke(this, actionSpeed);
            }
        }

        public bool IsActionPause => GameMath.IsZeroApprox(ActionSpeed);

        public GameDomain(GameDomainSettings domainSettings = default, GameWorldSettings worldSettings = default)
        {
            Settings = domainSettings ?? new GameDomainSettings();
            GameWorld = new GameWorld(this, worldSettings);
        }

        public void Initialize(Script.Game node)
        {
            Node = node;
            ActionSpeed = Settings.InitialActionSpeed;


            AddController(new GameNationController(this, new GameNationControllerSettings {
                Name = (string)ProjectSettings.GetSetting("extra_settings/client_settings/player_name")
            }));


            for (var npcCounter = Settings.NpcCount; npcCounter > -1; npcCounter--)
                Add(new BotController(id) {Name = Settings.NpcNameGenerator(id++)});
        }

        public void PlusActionSpeed(int steps = 1)
            => ActionSpeed += Settings.ActionSpeedSteps * steps;

        public void SubActionSpeed(int steps = 1)
            => ActionSpeed -= Settings.ActionSpeedSteps * steps;

        private float lastSpeed;

        public void ToggleActionSpeed()
        {
            if (IsActionPause)
            {
                if (GameMath.IsZeroApprox(lastSpeed))
                    PlusActionSpeed();
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

        private void _onControllerSet(GameNationController item)
        {
            item.
        }

        public void AddController(GameNationController item)
        {

        }
    }
}