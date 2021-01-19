using System;
using Godot;
using SpartansLib;
using SpartansLib.Extensions;
using SpartansLib.Structure;

namespace MSG.Utility
{
    public static class MoveFunc
    {
        public static Vector2 GetDesiredVelocity(Vector2 position, Vector2 target, float strength)
            => (target - position).Normalized() * strength;

        public static Vector2 GetSteerForce(Vector2 velocity,
            Vector2 desiredVelocity,
            float maxSpeed,
            float maxForce,
            float mass)
            => (((desiredVelocity - velocity).Clamped(maxForce) / mass) + velocity).Clamped(maxSpeed);

        public static Vector2 Seek(Vector2 start,
            Vector2 target,
            Vector2 velocity,
            float steerStrength,
            float maxSpeed,
            float maxForce,
            float mass)
            => GetSteerForce(velocity, GetDesiredVelocity(start, target, steerStrength), maxSpeed, maxForce, mass);

        public static Vector2 Flee(Vector2 start,
            Vector2 target,
            Vector2 velocity,
            float steerStrength,
            float maxSpeed,
            float maxForce,
            float mass)
            => GetSteerForce(velocity, -GetDesiredVelocity(start, target, steerStrength), maxSpeed, maxForce, mass);

        public static Vector2 Arrive(Vector2 start,
            Vector2 target,
            Vector2 velocity,
            float steerStrength,
            float maxSpeed,
            float maxForce,
            float mass,
            float arriveRadius)
        {
            var desiredVelocity = target - start;
            var distance = desiredVelocity.Length();
            desiredVelocity = desiredVelocity.Normalized() * steerStrength;
            if (distance < arriveRadius) desiredVelocity *= distance / arriveRadius;
            return GetSteerForce(velocity, desiredVelocity, maxSpeed, maxForce, mass);
        }

        public static Offset Arrive(Offset start,
            Vector2 target,
            Vector2 velocity,
            float steerStrength,
            float maxSpeed,
            float maxForce,
            float mass,
            float arriveRadius,
            float maxAngle,
            float angleWeight)
        {
            if (target == start.Position) return new Offset();
            if (Mathf.IsZeroApprox(steerStrength))
            {
                Logger.Warn($"{nameof(steerStrength)} set to 0, no movement calculated.");
                return new Offset(Vector2.Zero,
                    RotateLerp(start.Rotation, target.AngleToPoint(start.Position), maxAngle, angleWeight));
            }

            var desiredVelocity = start.Position - target;
            var distance = desiredVelocity.Length();
            var truncSpeed = Mathf.Min(steerStrength, steerStrength * (distance / arriveRadius * mass));
            var truncVelocity = desiredVelocity * (truncSpeed / distance);
            var steering = (truncVelocity - velocity).Clamped(maxForce);
            velocity += (steering / mass).MakeRotation(start.Rotation);

            Angle startAngle = start.Rotation,
                endAngle = target.AngleToPoint(start.Position);

            var offset = new Offset(
                velocity.Clamped(distance < arriveRadius * mass
                    ? distance / (arriveRadius * mass) * steerStrength
                    : steerStrength),
                RotateLerp(startAngle, endAngle, maxAngle, angleWeight));

            //if (endAngle > startAngle)
            //    offset.Rotation = Mathf.Min(offset.Rotation, endAngle);
            //else if(startAngle > endAngle)
            //offset.Rotation = Mathf.Max(offset.Rotation, endAngle);

            return offset;
        }

        public static float RotateLerp(float start, float end, float max, float weight)
        {
            //float normStart = Angle.NormalizeToFloat(start, true),
            //    normEnd = Angle.NormalizeToFloat(end, true);
            //if (Mathf.Abs(normEnd - normStart) > max * weight)
            //{
            //    end = normEnd;
            //    end = normStart > end ? normStart - max * weight : normStart + max * weight;
            //    end = Angle.NormalizeToFloat(end);
            //}
            return new Angle(start, true).LerpAngle(end, weight * max);
            start += Mathf.Pi;
            end += Mathf.Pi;
            if (end > start) end = Mathf.Min(end, start + max);
            else if (start > end) end = Mathf.Max(end, start - max);
            return new Angle(start, true).LerpAngle(end, weight).Clamp(-end, end); // fix this
        }

        public static float? RotateLerpChecked(float start,
            float end,
            float max,
            float thresholdForRotate,
            float weight)
        {
            start = Angle.Normalize(start);
            end = Angle.Normalize(end);
            float diff;
            if ((diff = Math.Abs(end - start)) > Mathf.Epsilon
                && Mathf.Abs(diff - thresholdForRotate) > Mathf.Epsilon)
                return RotateLerp(start, end, max, weight);
            return null;
        }
    }
}