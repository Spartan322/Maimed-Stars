using MSG.Script.Game.World;

namespace MSG.Game.Rts.World
{
    public class NationController
    {
        public class SettingsClass
        {
            public string Name = "AI";
            public bool IsClient = false;
        }

        public GameDomain Domain { get; }

        public SettingsClass Settings { get; }

        public NationController(GameDomain domain, SettingsClass settings = default)
        {
            Settings = settings ?? new SettingsClass();
            Domain = domain;
        }
    }
}