using System.Collections.Generic;
using Godot;
using MSG.Utility;

namespace MSG.Script.Agent
{
    public partial class Ship
    {
        public const float BRAKING_RADIUS = 70;
        public const float GOAL_THRESHOLD = 1f;

        //TODO: figure out units for all Movement related variables

        public float MaximumAngularSpeed { get; } = 2.5f;
        public float MaximumAcceleration { get; } = 5;

        public override bool CanProcessAim
        {
            get
            {
                if(MovementTarget != null) return false;
                return base.CanProcessAim;
            }
            set => base.CanProcessAim = value;
        }

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
            if (_movementTargetQueue.Count < 1)
            {
                MovementTarget = null;
                return false;
            }
            MovementTarget = _movementTargetQueue.Dequeue();
            return true;
        }

        private void _HandleMovementPhysicsProcess(float delta)
        {
            if (!CanProcessMovement || MovementTarget == null) return;
            var dist = Position.DistanceSquaredTo(MovementTarget.Value);
            if (_movementTargetQueue.Count > 0 && dist < BRAKING_RADIUS*BRAKING_RADIUS || dist < GOAL_THRESHOLD*GOAL_THRESHOLD)
            {
                _MakeMoveToNextTarget();
                Velocity = Vector2.Zero;
                return;
            }

            (Velocity, Rotation) = MoveFunc.Arrive(
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