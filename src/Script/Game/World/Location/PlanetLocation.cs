using Godot;
using MSG.Game.Rts.World.Location;

namespace MSG.Script.Game.World.Location
{
    public class PlanetLocation : StaticLocation
    {
        [Export]
        public string NationIdentifier;

        public StarLocation Star { get; private set; }

        public GameNation Nation { get; private set; }
    }
}
