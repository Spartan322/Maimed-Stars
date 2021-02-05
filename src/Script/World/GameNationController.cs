using MSG.Game;

namespace MSG.Script.World
{

    public class GameNationController
    {
        public class SettingsClass
        {
            public string Name = "AI";
            public bool IsClient = false;
        }

        public GameDomain Domain { get; }

        public SettingsClass Settings { get; }

        public GameNationController(GameDomain domain, SettingsClass settings = default)
        {
            Settings = settings ?? new SettingsClass();
            Domain = domain;
        }
    }
}