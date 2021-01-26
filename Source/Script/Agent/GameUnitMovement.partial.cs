using Godot;

namespace MSG.Script.Agent
{
    public partial class GameUnit
    {
        public Vector2? MovementTarget { get; protected set; }
        public Vector2 Velocity;
        public float MaximumSpeedLimit = 100;
        public virtual bool CanProcessMovement { get; set; } = true;

        public bool MoveTo(Vector2 target)
        {
            if(!CanMoveTo(target)) return false;
            AddMovementTarget(target);
            return true;
        }

        public virtual bool CanMoveTo(Vector2 target) => true;

        public virtual void AddMovementTarget(Vector2 target) { }

        public virtual void ClearMovementTargets() { }
    }
}