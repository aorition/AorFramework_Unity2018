using System;

namespace AorBaseUtility
{
    public struct Vector3d
    {
        public const double kEpsilon = 1E-5d;
        public const double equalNum = 9.999999E-11d;

        public static Vector3d UnitX
        {
            get
            {
                return new Vector3d(1, 0, 0);
            }
        }

        public static Vector3d UnitY
        {
            get
            {
                return new Vector3d(0, 1, 0);
            }
        }

        public static Vector3d UnitZ
        {
            get
            {
                return new Vector3d(0, 0, 1);
            }
        }

        public static Vector3d Zero
        {
            get
            {
                return new Vector3d(0, 0, 0);
            }
        }

        public static Vector3d One
        {
            get
            {
                return new Vector3d(1, 1, 1);
            }
        }

        public double x;
        public double y;
        public double z;

        #region static function
        public static Vector3d Lerp(Vector3d from, Vector3d to, double t)
        {
            t = AMath.Clamp01(t);
            return new Vector3d(from.x + ((to.x - from.x) * t), from.y + ((to.y - from.y) * t), from.z + ((to.z - from.z) * t));
        }

        public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed = 1, double deltaTime = 0.02f)
        {
            smoothTime = Math.Max(0.0001d, smoothTime);
            double num = 2 / smoothTime;
            double num2 = num * deltaTime;
            double num3 = 1 / (((1 + num2) + ((0.48d * num2) * num2)) + (((0.235d * num2) * num2) * num2));
            Vector3d vector = current - target;
            Vector3d vector2 = target;
            double maxLength = maxSpeed * smoothTime;
            vector = ClampMagnitude(vector, maxLength);
            target = current - vector;
            Vector3d ykVector3D = (currentVelocity + (num * vector)) * deltaTime;
            currentVelocity = (currentVelocity - (num * ykVector3D)) * num3;
            Vector3d vector4 = target + ((vector + ykVector3D) * num3);
            if ((vector2 - current).Dot(vector4 - vector2) > 0)
            {
                vector4 = vector2;
                currentVelocity = (vector4 - vector2) / deltaTime;
            }
            return vector4;
        }

        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
        {
            if (vector.SqrMagnitude > (maxLength * maxLength))
            {
                return vector.Normalized * maxLength;
            }
            return vector;
        }

        public double SqrMagnitude
        {
            get
            {
                return ((x * x) + (y * y)) + (z * z);
            }
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(((x * x) + (y * y)) + (z * z));
            }
        }

        public Vector3d Normalized
        {
            get
            {
                double num = Magnitude;
                if (num > kEpsilon)
                {
                    return (this / num);
                }
                return Zero;
            }
        }

        public void Normalize()
        {
            double num = Magnitude;
            if (num > kEpsilon)
            {
                x /= num;
                y /= num;
                z /= num;
            }
            else
            {
                x = 0;
                y = 0;
                z = 0;
            }
        }

        public Vector3d Inversed
        {
            get
            {
                return new Vector3d(-x, -y, -z);
            }
        }

        public void Inverse()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public static double Dot(Vector3d lhs, Vector3d rhs)
        {
            return (lhs.x * rhs.x) + (lhs.y * rhs.y) + (lhs.z * rhs.z);
        }

        public double Dot(Vector3d rhs)
        {
            return (x * rhs.x) + (y * rhs.y) + (z * rhs.z);
        }

        public static Vector3d Scale(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
        }

        public static Vector3d Project(Vector3d vector, Vector3d onNormal)
        {
            double num = onNormal.Dot(onNormal);
            if (num < double.Epsilon)
            {
                return Zero;
            }
            return (onNormal * vector.Dot(onNormal)) / num;
        }

        public static Vector3d Exclude(Vector3d excludeThis, Vector3d fromThat)
        {
            return (fromThat - Project(fromThat, excludeThis));
        }

        public static double Angle(Vector3d from, Vector3d to)
        {
            return Math.Acos(AMath.Clamp(from.Normalized.Dot(to.Normalized), -1, 1)) * 57.29578d;
        }

        public double Distance(Vector3d b)
        {
            Vector3d vector = new Vector3d(x - b.x, y - b.y, z - b.z);
            return Math.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }

        public double DistanceSquared(Vector3d b)
        {
            Vector3d vector = new Vector3d(x - b.x, y - b.y, z - b.z);
            return (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);
        }

        public static Vector3d Min(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        public static Vector3d Max(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
        {
            Vector3d vector = target - current;
            double magnitude = vector.Magnitude;
            if ((magnitude > maxDistanceDelta) && (magnitude != 0))
            {
                return (current + ((Vector3d)((vector / magnitude) * maxDistanceDelta)));
            }
            return target;
        }
        #endregion

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vector3d(Vector3d v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    case 2:
                        return this.z;
                }
                throw new IndexOutOfRangeException("Invalid Vector3 index!");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    case 2:
                        this.z = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        public void Set(double new_x, double new_y, double new_z)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
        }

        public void Scale(Vector3d scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
            this.z *= scale.z;
        }

        public void Scale(double v)
        {
            this.x *= v;
            this.y *= v;
            this.z *= v;
        }

        public void Add(Vector3d v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }

        public void Sub(Vector3d v)
        {
            x -= v.x;
            y -= v.y;
            z -= v.z;
        }

        public void Mul(Vector3d v)
        {
            x *= v.x;
            y *= v.y;
            z *= v.z;
        }

        public string ToString(string format)
        {
            object[] args = new object[] { this.x.ToString(format), this.y.ToString(format), this.z.ToString(format) };
            return string.Format("({0}, {1}, {2})", args);
        }

        #region override funtion

        public override int GetHashCode()
        {
            return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3d))
            {
                return false;
            }
            Vector3d vector = (Vector3d)other;
            return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
        }

        public override string ToString()
        {
            object[] args = new object[] { this.x, this.y, this.z };
            return string.Format("({0:F}, {1:F}, {2:F})", args);
        }
        #endregion

        #region reload function
        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator +(Vector3d a, double b)
        {
            return new Vector3d(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator -(Vector3d a)
        {
            return new Vector3d(-a.x, -a.y, -a.z);
        }

        public static Vector3d operator *(Vector3d a, double d)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator *(double d, Vector3d a)
        {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator /(Vector3d a, double d)
        {
            return new Vector3d(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return ((lhs - rhs).SqrMagnitude < equalNum);
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return ((lhs - rhs).SqrMagnitude >= equalNum);
        }
        #endregion
    }
}
