namespace MSG.Game
{
    public class GameNationControllerSettings
    {
        public string Name = "AI";
    }

    public class GameNationController
    {
        public GameDomain Domain { get; }

        public GameNationControllerSettings Settings { get; }

        public GameNationController(GameDomain domain, GameNationControllerSettings settings = default)
        {
            Settings = settings ?? new GameNationControllerSettings();
            Domain = domain;
        }
    }
}