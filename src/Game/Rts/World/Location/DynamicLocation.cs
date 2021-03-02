using Godot;

namespace MSG.Game.Rts.World.Location
{
    public class DynamicLocation : Location
    {
        public Vector2 Velocity { get; protected set; }

        public override void _PhysicsProcess(float delta)
        {
            if (Velocity.IsEqualApprox(Vector2.Zero)) return;
            Position += Velocity * delta;
        }
    }
}