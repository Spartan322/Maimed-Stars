using System.Collections.Generic;
using Godot;
using MSG.Game;
using SpartansLib.XML;

namespace MSG.Script.World
{
    public class GameNation : Node
    {
        public class SettingsClass { }

        private readonly List<GameNationController> _controllers = new List<GameNationController>();

        public GameWorld World { get; internal set; }

        public SettingsClass Settings { get; set; } = new SettingsClass();

        public UnitManager UnitManager { get; private set; }

        public GameNation() { }

        public GameNation(GameWorld gameWorld) { World = gameWorld; _Ready(); }

        public override void _Ready()
        {
            UnitManager = new UnitManager(this);
            if (World == null) World = GetParent<GameWorld>();
        }
    }
}
