using System;

namespace AorBaseUtility
{
    public struct AABB3d
    {
        public Vector3d Max;
        public Vector3d Min;

        public AABB3d(Vector3d min, Vector3d max)
        {
            Max = max;
            Min = min;
        }

        public AABB3d Reset()
        {
            Max.Set(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Min.Set(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

            return this;
        }

        public AABB3d AddPoint(Vector3d v)
        {
            Max.Set(Math.Max(Max.x, v.x), Math.Max(Max.y, v.y), Math.Max(Max.z, v.z));
            Min.Set(Math.Min(Min.x, v.x), Math.Min(Min.y, v.y), Math.Min(Min.z, v.z));
            return this;
        }

        public Vector3d Center
        {
            get { return (Max + Min) / 2; }
        }

    }
}
