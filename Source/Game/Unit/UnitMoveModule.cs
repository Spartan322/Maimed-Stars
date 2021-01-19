using System;
using System.Collections.Generic;
using Godot;
using MSG.Utility;

namespace MSG.Game.Unit
{
    public class UnitMoveModule : UnitBaseModule
    {
        public const float BRAKING_RADIUS = 50;
        public const float GOAL_THRESHOLD = 0.9f;

        public Vector2? Target { get; private set; }
        public Vector2 Velocity;
        public float TargetRotation;
        public float MaximumSpeedLimit;

        private readonly Queue<Vector2> _targetQueue = new Queue<Vector2>();

        public UnitMoveModule(IUnitController unitController) : base(unitController) {}

        public void AddTarget(Vector2 target)
        {
            if (Target == null) Target = target;
            else _targetQueue.Enqueue(target);
        }

        public void ClearTargets()
        {
            Target = null;
            _targetQueue.Clear();
        }

        public bool MoveToNextTarget()
        {
            if (_targetQueue.Count < 0)
            {
                Target = null;
                return false;
            }
            Target = _targetQueue.Dequeue();
            return true;
        }

        public override PrimT? GetStruct<PrimT>(string name)
        {
            if(name == "Velocity") return (PrimT)(object)(Vector2?)Velocity;
            return base.GetStruct<PrimT>(name);
        }

        public override void PhysicsProcess(float delta)
        {
            if (Target == null) return;
            var dist = UnitController.NodeObject.Position.DistanceTo(Target.Value);
            if (_targetQueue.Count > 0 && dist < BRAKING_RADIUS || dist < GOAL_THRESHOLD)
            {
                MoveToNextTarget();
                Velocity = Vector2.Zero;
                return;
            }

            (Velocity, TargetRotation) = MoveFunc.Arrive(
                (UnitController.NodeObject.Position, UnitController.NodeObject.Rotation),
                Target.Value,
                Velocity,
                MaximumSpeedLimit,
                Mathf.Min(MaximumSpeedLimit, UnitController.MaximumMoveSpeed),
                UnitController.MaximumAcceleration,
                UnitController.Mass,
                BRAKING_RADIUS,
                UnitController.MaximumAngularSpeed,
                delta
            );
        }
    }
}