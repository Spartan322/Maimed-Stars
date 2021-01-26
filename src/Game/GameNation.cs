using System.Collections.Generic;
using SpartansLib.XML;

namespace MSG.Game
{
    public struct GameNationSettings
    {
    }

    public class GameNation
    {
        private readonly List<GameNationController> _controllers = new List<GameNationController>();

        public GameWorld World { internal set; get; }

        public GameNationSettings Settings { get; }

        public UnitManager UnitManager { get; }

        public GameNation(GameWorld gameWorld, GameNationSettings gameNationSettings = default)
        {
            World = gameWorld;
            Settings = gameNationSettings;
            UnitManager = new UnitManager(this);
        }

    }
}