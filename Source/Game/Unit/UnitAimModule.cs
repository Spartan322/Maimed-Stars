using System;
using Godot;
using MSG.Utility;

namespace MSG.Game.Unit
{
    public class UnitAimModule : UnitBaseModule
    {
        private const float ANGULAR_START_THRESHOLD = 7 * ((float) Math.PI / 180f);

        public Vector2? Target;
        public float TargetAngle;
        public bool CanAim;

        public UnitAimModule(IUnitController unitController) : base(unitController) {}

        public override void PhysicsProcess(float delta)
        {
            if (!CanAim || Target == null) return;
            var rotation = MoveFunc.RotateLerpChecked(UnitController.NodeObject.Rotation,
                Target.Value.AngleToPoint(UnitController.NodeObject.Position),
                UnitController.MaximumAngularSpeed,
                ANGULAR_START_THRESHOLD,
                delta);
            if (rotation != null)
                TargetAngle = rotation.Value;
        }
    }
}