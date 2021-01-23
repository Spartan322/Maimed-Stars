using System;
using Godot;

namespace MSG.Script.Agent
{
    public partial class Ship
    {
        private const float ANGULAR_START_THRESHOLD = 7 * ((float) Math.PI / 180f);

        private void _HandleAimPhysicsProcess(float delta)
        {
            if (!CanAim || AimTarget == null) return;
            var rotation = Utility.MoveFunc.RotateLerpChecked(Rotation,
                AimTarget.Value.AngleToPoint(Position),
                MaximumAngularSpeed,
                ANGULAR_START_THRESHOLD,
                delta);
            if (rotation != null)
                TargetAngle = rotation.Value;
        }
    }
}