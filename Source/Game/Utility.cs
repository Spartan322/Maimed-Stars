using System;

namespace MSG.Game
{
    public static class GameMath
    {
        public static double Stepify(double s, double step)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (step != 0f)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                s = Math.Floor(s / step + 0.5) * step;
            return s;
        }

        public static double Clamp(double value, double min, double max)
            => (value < min) ? min : ((value > max) ? max : value);

        public static long Clamp(long value, long min, long max)
            => (value < min) ? min : ((value > max) ? max : value);

        public static bool IsZeroApprox(double value)
            => Abs(value) < 1E-06;

        public static double Abs(double value)
            => Math.Abs(value);
    }
}