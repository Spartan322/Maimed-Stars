using System;
using MSG.Game.Rts.Unit;

namespace MSG.Script.Game.Unit
{
    public partial class SingleUnit
    {
        private const float ANGULAR_START_THRESHOLD = 7 * ((float)Math.PI / 180f);

        private void _HandleAimPhysicsProcess(float delta)
        {
            if (!CanProcessAim || AimTarget == null) return;
            var rotation = MoveFunctions.RotateLerpChecked(Rotation,
                AimTarget.Value.AngleToPoint(Position),
                MaximumAngularSpeed,
                ANGULAR_START_THRESHOLD,
                delta);
            if (rotation != null)
                TargetAngle = rotation.Value;
        }
    }
}