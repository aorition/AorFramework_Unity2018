using System;

namespace AorBaseUtility
{
    public struct AABB2d
    {
        public Vector2d Max;
        public Vector2d Min;

        public AABB2d(Vector2d min, Vector2d max)
        {
            Max = max;
            Min = min;
        }

        public AABB2d Reset()
        {
            Max.Set(double.NegativeInfinity, double.NegativeInfinity);
            Min.Set(double.PositiveInfinity, double.PositiveInfinity);

            return this;
        }

        public AABB2d AddPoint(Vector2d v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y));
            return this;
        }

        public Vector2d Center
        {
            get { return (Max + Min) / 2; }
        }

    }
}
