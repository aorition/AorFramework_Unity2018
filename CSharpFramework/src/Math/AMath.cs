using System;
using System.Collections.Generic;

namespace AorBaseUtility
{
    public static class AMath
    {

        /// <summary>
        /// 线性插值
        /// </summary>
        public static double Linear_Interpolate(double a, double b, double x)
        {
            return a * (1 - x) + b * x;
        }

        public static double Linear_Interpolate(float a, float b, float x)
        {
            return a * (1 - x) + b * x;
        }

        /// <summary>
        /// 余弦插值
        /// </summary>
        public static double Cosine_Interpolate(double a, double b, double x)
        {
            double ft = x * 3.1415927d;
            double f = (1d - System.Math.Cos(ft)) * 0.5d;
            return a * (1d - f) + b * f;
        }

        public static float Cosine_Interpolate(float a, float b, float x)
        {
            float ft = x * 3.1415927f;
            float f = (1f - (float)System.Math.Cos(ft)) * 0.5f;
            return a * (1f - f) + b * f;
        }

        /// <summary>
        /// 立方插值
        /// (插值结果介于v1和v2之间)
        /// </summary>
        public static double Cubic_Interpolate(double v0, double v1, double v2, double v3, double x)
        {
            double P = (v3 - v2) - (v0 - v1);
            double Q = (v0 - v1) - P;
            double R = v2 - v0;
            return System.Math.Pow(P * x, 3) + System.Math.Pow(Q * x, 2) + R * x + v1;
        }

        public static float Cubic_Interpolate(float v0, float v1, float v2, float v3, float x)
        {
            float P = (v3 - v2) - (v0 - v1);
            float Q = (v0 - v1) - P;
            float R = v2 - v0;
            return (float)System.Math.Pow(P * x, 3) + (float)System.Math.Pow(Q * x, 2) + R * x + v1;
        }

        /*
        public static double Noise(int x)
        {
            x = (x << 13) ^ x;
            return (1.0 - ((x*(x*x*15731 + 789221) + 1376312589) & 0x7fffffff)/1073741824.0);
        }*/

        /// <summary>
        /// 二分查找
        /// </summary>
        public static int BinarySearch(int[] Array, int key)
        {
            if (Array != null)
            {
                int mid;
                int start = 0;
                int end = Array.Length - 1;
                while (start <= end)
                {
                    mid = (end - start) / 2 + start;
                    if (key < Array[mid])
                    {
                        end = mid - 1;
                    }
                    else if (key > Array[mid])
                    {
                        start = mid + 1;
                    }
                    else
                    {
                        return mid;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 二分搜索法变种
        /// </summary>
        /// <param name="condition">条件判断,返回-1表示"小了",返回1表示"大了",返回0表示"命中"</param>
        public static int BinarySearch<T>(T[] Array, Func<T,int> condition) where T : class 
        {
            if (Array != null)
            {
                int mid;
                int start = 0;
                int end = Array.Length - 1;
                while (start <= end)
                {
                    mid = (end - start) / 2 + start;
                    int cd = condition(Array[mid]);
                    if (cd < 0)
                        end = mid + cd;
                    else if (cd > 0)
                        start = mid + cd;
                    else
                        return mid;
                }
            }
            return -1;
        }


        public static int Clamp(int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Clamp(float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }
        public static double Clamp(double v, double min, double max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Clamp01(float v)
        {
            return v < 0 ? 0 : (v > 1 ? 1 : v);
        }
        public static double Clamp01(double v)
        {
            return v < 0 ? 0 : (v > 1 ? 1 : v);
        }

        public static float Lerp(float from, float to, float t)
        {
            return (from + ((to - from) * Clamp01(t)));
        }

        public static float Repeat(float t, float length)
        {
            return (t - ((float)(Math.Floor(t / length)) * length));
        }

        public static float LerpAngle(float a, float b, float t)
        {
            float num;
            num = Repeat(b - a, 360f);
            if (num <= 180f)
            {
                goto Label_0021;
            }
            num -= 360f;
            Label_0021:
            return (a + (num * Clamp01(t)));
        }

        public static float Distance(float a, float b)
        {
            return Math.Abs(a - b);
        }

        public static double Distance(double a, double b)
        {
            return Math.Abs(a - b);
        }

        public static float Ln(float a)
        {
            return (float)Math.Log(a, Math.E);
        }

        public static double Ln(double a)
        {
            return Math.Log(a, Math.E);
        }


        public static bool IsRange(float value, float min, float max)
        {
            return value < min ? false : (value > max ? false : true);
        }

        public static Vector2d ToVector2(this Vector3d v)
        {
            return new Vector2d(v.x, v.z);
        }

        public static Vector2f ToVector2(this Vector3f v)
        {
            return new Vector2f(v.x, v.z);
        }

        public static Vector3d ToVector3(this Vector2d v, double y = 0)
        {
            return new Vector3d(v.x, y, v.y);
        }

        public static Vector3f ToVector3(this Vector2f v, float y = 0)
        {
            return new Vector3f(v.x, y, v.y);
        }

    }
}
