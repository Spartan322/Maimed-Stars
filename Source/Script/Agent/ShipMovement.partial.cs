using System.Collections.Generic;
using Godot;
using MSG.Utility;

namespace MSG.Script.Agent
{
    public partial class Ship
    {
        public const float BRAKING_RADIUS = 50;
        public const float GOAL_THRESHOLD = 0.9f;

        private readonly Queue<Vector2> _movementTargetQueue = new Queue<Vector2>();

        public override void AddMovementTarget(Vector2 target)
        {
            if (MovementTarget == null) MovementTarget = target;
            else _movementTargetQueue.Enqueue(target);
        }

        public override void ClearMovementTargets()
        {
            MovementTarget = null;
            _movementTargetQueue.Clear();
        }

        private bool _MakeMoveToNextTarget()
        {
            if (_movementTargetQueue.Count < 0)
            {
                MovementTarget = null;
                return false;
            }
            MovementTarget = _movementTargetQueue.Dequeue();
            return true;
        }

        private void _HandleMovementPhysicsProcess(float delta)
        {
            if (MovementTarget == null) return;
            var dist = Position.DistanceTo(MovementTarget.Value);
            if (_movementTargetQueue.Count > 0 && dist < BRAKING_RADIUS || dist < GOAL_THRESHOLD)
            {
                _MakeMoveToNextTarget();
                Velocity = Vector2.Zero;
                return;
            }

            (Velocity, TargetRotation) = MoveFunc.Arrive(
                (Position, Rotation),
                MovementTarget.Value,
                Velocity,
                MaximumSpeedLimit,
                Mathf.Min(MaximumSpeedLimit, MaximumMoveSpeed),
                MaximumAcceleration,
                Mass,
                BRAKING_RADIUS,
                MaximumAngularSpeed,
                delta
            );
        } 
    }
}