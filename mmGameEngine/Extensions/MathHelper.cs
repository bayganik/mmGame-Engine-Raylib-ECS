﻿using System;
using System.Collections.Generic;
using System.Text;

namespace mmGameEngine
{
    public static class MathHelper
    {
        public const double E = (double)Math.E;
        public const double Log10E = 0.4342945f;                            //Represents the log base ten of e(0.4342945).
        public const double Log2E = 1.442695f;                              //Represents the log base two of e(1.442695).
        public const double Pi = (double)Math.PI;                           //Represents the value of pi(3.14159274).
        public const double PiOver2 = (double)(Math.PI / 2.0);              //Represents the value of pi divided by two(1.57079637).
        public const double PiOver4 = (double)(Math.PI / 4.0);              //Represents the value of pi divided by four(0.7853982).
        public const double TwoPi = (double)(Math.PI * 2.0);                //Represents the value of pi times two(6.28318548).

        public static double Barycentric(double value1, double value2, double value3, double amount1, double amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static double CatmullRom(double value1, double value2, double value3, double value4, double amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (double)(0.5 * (2.0 * value2 +
                (value3 - value1) * amount +
                (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        public static double Clamp(double value, double min, double max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
        public static int Clamp(int value, int min, int max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }

        public static double Distance(double value1, double value2)
        {
            return Math.Abs(value1 - value2);
        }

        public static double Hermite(double value1, double tangent1, double value2, double tangent2, double amount)
        {
            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                    (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                    t1 * s +
                    v1;
            return (double)result;
        }


        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static double Max(double value1, double value2)
        {
            return Math.Max(value1, value2);
        }
        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }
        public static int Max(int value1, int value2)
        {
            return Math.Max(value1, value2);
        }

        public static double Min(double value1, double value2)
        {
            return Math.Min(value1, value2);
        }
        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }
        public static int Min(int value1, int value2)
        {
            return Math.Min(value1, value2);
        }
        public static double SmoothStep(double value1, double value2, double amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
#if(USE_FARSEER)
            double result = SilverSpriteMathHelper.Clamp(amount, 0f, 1f);
            result = SilverSpriteMathHelper.Hermite(value1, 0f, value2, 0f, result);
#else
            double result = MathHelper.Clamp(amount, 0f, 1f);
            result = MathHelper.Hermite(value1, 0f, value2, 0f, result);
#endif
            return result;
        }

        public static double ToDegrees(double radians)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = 180 / pi
            return (double)(radians * 57.295779513082320876798154814105);
        }
        public static float ToDegrees(float radians)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = 180 / pi
            return (float)(radians * 57.295779513082320876798154814105);
        }
        public static float ToRadians(float angle)
        {
            return angle * (float)Math.PI / 180;
        }
        public static double ToRadians(double degrees)
        {
            // This method uses double precission internally,
            // though it returns single double
            // Factor = pi / 180
            return (double)(degrees * 0.017453292519943295769236907684886);
        }

        public static double WrapAngle(double angle)
        {
            angle = (double)Math.IEEERemainder((double)angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }

        public static bool IsPowerOfTwo(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }
    }
}


