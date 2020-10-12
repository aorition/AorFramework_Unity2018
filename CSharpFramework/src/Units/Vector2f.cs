//-----------------------------------------------------------------------
//| by:Qcbf                                                             |
//-----------------------------------------------------------------------
using System;

namespace AorBaseUtility
{
    public struct Vector2f
    {
        public const double kEpsilon = 1E-5d;
        public const double equalNum = 9.999999E-11d;

        public float x;
        public float y;

        public static Vector2f Zero
        {
            get
            {
                return new Vector2f(0, 0);
            }
        }

        public static Vector2f One
        {
            get
            {
                return new Vector2f(1, 1);
            }
        }

        public static Vector2f Max
        {
            get
            {
                return new Vector2f(float.MaxValue, float.MaxValue);
            }
        }

        public static Vector2f UnitX
        {
            get
            {
                return new Vector2f(1, 0);
            }
        }

        public static Vector2f UnitY
        {
            get
            {
                return new Vector2f(0, 1);
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public float SqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }


        public Vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2f(Vector2f vec2)
        {
            this.x = vec2.x;
            this.y = vec2.y;
        }

        public Vector2f Normalized
        {
            get
            {

                Vector2f res = new Vector2f(this);
                float num = res.Magnitude;
                if (num <= 1e-05f)
                {
                    res.x = 0;
                    res.y = 0;
                }
                else
                {
                    res.x /= num;
                    res.y /= num;
                }

                return res;
            }
        }

        public Vector2f Inversed
        {
            get
            {
                return new Vector2f(-x, -y);
            }
        }

        public void Inverse()
        {
            x = -x;
            y = -y;
        }

        public void Normalize()
        {
            float num = Magnitude;
            if (num <= 1e-05f)
            {
                x = 0;
                y = 0;
            }
            else
            {
                x /= num;
                y /= num;
            }
        }

        public void Set(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public void Add(Vector2f a)
        {
            x += a.x;
            y += a.y;
        }

        public void Add(float _x, float _y)
        {
            x += _x;
            y += _y;
        }

        public void Sub(Vector2f a)
        {
            x -= a.x;
            y -= a.y;
        }
        public void Sub(float _x, float _y)
        {
            x -= _x;
            y -= _y;
        }

        public void Mul(float _x, float _y)
        {
            x *= _x;
            y *= _y;
        }

        public float Dot(Vector2f a)
        {
            return x * a.x + y * a.y;
        }

        public void MoveForward(float angle, float speed)
        {
            x += speed * (float)Math.Cos(angle);
            y += speed * (float)Math.Sin(angle);
        }

        public float Distance(Vector2f b)
        {
            return (float)Math.Sqrt(DistanceSquared(b));
        }

        public float DistanceSquared(Vector2f b)
        {
            return (float)((x - b.x) * (x - b.x) + (y - b.y) * (y - b.y));
        }

        public float Angle(Vector2f b)
        {
            return (float)Math.Atan2(b.y - this.y, b.x - this.x) * 180f / (float)Math.PI;
        }

        public override string ToString()
        {
            string dx = x.ToString("f4");
            string dy = y.ToString("f4");
            return string.Format("SimpleVector2({0}, {1})", dx, dy);
        }


        public static Vector2f Lerp(Vector2f from, Vector2f to, float t)
        {
            return new Vector2f(from.x + ((to.x - from.x) * t), from.y + ((to.y - from.y) * t));
        }


        public static float Distance(Vector2f a, Vector2f b)
        {
            return a.Distance(b);
        }

        public static float Angle(Vector2f from, Vector2f to)
        {
            return from.Angle(to);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x + b.x, a.y + b.y);
        }
        public static Vector2f operator +(Vector2f a, float[] b)
        {
            return new Vector2f(a.x + b[0], a.y + b[1]);
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.x - b.x, a.y - b.y);
        }
        public static Vector2f operator -(Vector2f a, float[] b)
        {
            return new Vector2f(a.x - b[0], a.y - b[1]);
        }

        public static Vector2f operator *(Vector2f a, int b)
        {
            return new Vector2f(a.x * b, a.y * b);
        }

        public static Vector2f operator *(Vector2f a, float b)
        {
            return new Vector2f(a.x * b, a.y * b);
        }

        public static Vector2f operator /(Vector2f a, int b)
        {
            return new Vector2f(a.x / b, a.y / b);
        }

        public static Vector2f operator /(Vector2f a, float b)
        {
            return new Vector2f(a.x / b, a.y / b);
        }

        public static bool operator ==(Vector2f lhs, Vector2f rhs)
        {
            return ((lhs - rhs).SqrMagnitude < equalNum);
        }

        public static bool operator !=(Vector2f lhs, Vector2f rhs)
        {
            return ((lhs - rhs).SqrMagnitude >= equalNum);
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : (x == ((Vector2f)obj).x && y == ((Vector2f)obj).y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
