using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirkSoft
{
    public class MathHelper
    {
        /// <summary>
        /// Interpolates smoothly from c1 to c2 based on x compared to a1 and a2. 
        /// </summary>
        /// <param name="x">value</param>
        /// <param name="a1">min</param>
        /// <param name="a2">max</param>
        /// <param name="c1">from</param>
        /// <param name="c2">to</param>
        /// <returns></returns>
        public static double SmoothStep(double x, double a1, double a2, double c1, double c2)
        {
            return c1 + ((x - a1) / (a2 - a1)) * (c2 - c1) / 1.0f;
        }

        public static float Slerp(float a, float b, float t)
        {
            return a + (b - a) * (t * t * (3f - 2f * t));
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float Elerp(float a, float b, float t)
        {
            return a + (b - a) * (t * t);
        }

        // Cosine interpolation 
        // http://paulbourke.net/miscellaneous/interpolation/
        public static float Cerp(float y1, float y2, float mu)
        {
            float mu2;
            mu2 = (1f - (float)Math.Cos(mu * Math.PI)) / 2f;
            return (y1 * (1f - mu2) + y2 * mu2);
        }

        public static float CubicInterpolate(float y0, float y1, float y2, float y3, float mu)
        {
            float a0, a1, a2, a3, mu2;

            mu2 = mu * mu;
            a0 = y3 - y2 - y0 + y1;
            a1 = y0 - y1 - a0;
            a2 = y2 - y0;
            a3 = y1;

            return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
    }
}
