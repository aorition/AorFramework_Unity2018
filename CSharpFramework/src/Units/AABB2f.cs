using System;

namespace AorBaseUtility
{
    public struct AABB2f
    {
        public Vector2f Max;
        public Vector2f Min;

        public AABB2f(Vector2f min, Vector2f max)
        {
            Max = max;
            Min = min;
        }

        public AABB2f Reset()
        {
            Max.Set(float.NegativeInfinity, float.NegativeInfinity);
            Min.Set(float.PositiveInfinity, float.PositiveInfinity);

            return this;
        }

        public AABB2f AddPoint(Vector2f v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y));
            return this;
        }

        public Vector2f Center
        {
            get { return (Max + Min) / 2f; }
        }

    }
}
