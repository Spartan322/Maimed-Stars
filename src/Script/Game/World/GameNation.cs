using System.Collections.Generic;
using System.Linq;
using Godot;
using MSG.Game.Rts.Event;
using MSG.Game.Rts.Unit;
using SpartansLib.Attributes;

namespace MSG.Script.Game.World
{
    public class GameNation : Node2D
    {
        public class SettingsClass { }

        private readonly List<NationController> _controllers = new List<NationController>();

        [Node]
        public Node UnitList;

        public bool ClientNation { get; private set; }

        public GameWorld World { get; internal set; }

        public SettingsClass Settings { get; set; } = new SettingsClass();

        public UnitManager UnitManager { get; private set; }

        public EventManager EventManager { get; private set; }

        public GameNation() { }

        public GameNation(GameWorld gameWorld) { World = gameWorld; _EnterTree(); }

        public override void _EnterTree()
        {
            if (World == null) World = GetParent<GameWorld>();
            UnitManager = new UnitManager(this);
            if (_controllers.Any(cont => cont.Settings.IsClient)) ClientNation = true;
        }
    }
}
