using System;
namespace Netherite.Utils
{
    public static class MathHelper
    {
        public static double Max(params double[] values)
        {
            double result = double.MinValue;
            foreach(double v in values)
            {
                result = Math.Max(result, v);
            }
            return result;
        }

        public static double Min(params double[] values)
        {
            double result = double.MaxValue;
            foreach (double v in values)
            {
                result = Math.Min(result, v);
            }
            return result;
        }

        public const double DegToRad = Math.PI / 180.0;
    }
}
