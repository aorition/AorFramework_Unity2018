using System;

namespace AorBaseUtility
{
    public struct Sphere2f
    {
        public Vector2f Center;
        public float Radius;

        public Sphere2f(Vector2f center, float Radius)
        {
            this.Center = center;
            this.Radius = Radius;
        }
    }
}
