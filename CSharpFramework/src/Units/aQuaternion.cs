﻿using System;

namespace AorBaseUtility
{
    public struct aQuaternion
    {
        public const double equalNum = 9.999999E-11d;

        public double x, y, z, w;

        public static aQuaternion Identity
        {
            get { return new aQuaternion(0, 0, 0, 1); }
        }

        public static aQuaternion Zero
        {
            get { return new aQuaternion(0, 0, 0, 0); }
        }

        public double Roll
        {
            get
            {
                double fTy = 2.0 * y;
                double fTz = 2.0 * z;
                double fTwz = fTz * w;
                double fTxy = fTy * x;
                double fTyy = fTy * y;
                double fTzz = fTz * z;
                return Math.Atan2(fTxy + fTwz, 1.0 - (fTyy + fTzz));
            }
        }

        public double Pitch
        {
            get
            {
                double fTx = 2.0 * x;
                double fTz = 2.0 * z;
                double fTwx = fTx * w;
                double fTxx = fTx * x;
                double fTyz = fTz * y;
                double fTzz = fTz * z;

                return Math.Atan2(fTyz + fTwx, 1.0 - (fTxx + fTzz));
            }
        }

        public double Yaw
        {
            get
            {
                double fTx = 2.0 * x;
                double fTy = 2.0 * y;
                double fTz = 2.0 * z;
                double fTwy = fTy * w;
                double fTxx = fTx * x;
                double fTxz = fTz * x;
                double fTyy = fTy * y;

                return Math.Atan2(fTxz + fTwy, 1.0 - (fTxx + fTyy));
            }
        }

        public Vector3d AxisX
        {
            get
            {
                double fTy = 2.0 * y;
                double fTz = 2.0 * z;
                double fTwy = fTy * w;
                double fTwz = fTz * w;
                double fTxy = fTy * x;
                double fTxz = fTz * x;
                double fTyy = fTy * y;
                double fTzz = fTz * z;
                return new Vector3d(1.0 - (fTyy + fTzz), fTxy + fTwz, fTxz - fTwy);
            }
        }
        public Vector3d AxisY
        {
            get
            {
                double fTx = 2.0 * x;
                double fTy = 2.0 * y;
                double fTz = 2.0 * z;
                double fTwx = fTx * w;
                double fTwz = fTz * w;
                double fTxx = fTx * x;
                double fTxy = fTy * x;
                double fTyz = fTz * y;
                double fTzz = fTz * z;
                return new Vector3d(fTxy - fTwz, 1.0 - (fTxx + fTzz), fTyz + fTwx);
            }
        }
        public Vector3d AxisZ
        {
            get
            {
                double fTx = 2.0 * x;
                double fTy = 2.0 * y;
                double fTz = 2.0 * z;
                double fTwx = fTx * w;
                double fTwy = fTy * w;
                double fTxx = fTx * x;
                double fTxz = fTz * x;
                double fTyy = fTy * y;
                double fTyz = fTz * y;
                return new Vector3d(fTxz + fTwy, fTyz - fTwx, 1.0 - (fTxx + fTyy));
            }
        }

        public aQuaternion(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(Dot(this));
            }
        }

        public double SqrMagnitude
        {
            get
            {
                return Dot(this);
            }
        }

        public aQuaternion Inversed
        {
            get
            {
                double sqrLen = SqrMagnitude;
                if (sqrLen == 0.0)
                {
                    return Zero;
                }

                aQuaternion q = this;
                q.Scale(1.0 / sqrLen);
                q.Conjugate();
                return q;
            }
        }

        public void Inverse()
        {
            double sqrLen = SqrMagnitude;
            if (sqrLen == 0.0)
            {
                return;
            }

            Scale(1.0 / sqrLen);
            Conjugate();
        }

        public aQuaternion Conjugated
        {
            get
            {
                return new aQuaternion(-x, -y, -z, w);
            }
        }

        public void Conjugate()
        {
            Set(-x, -y, -z, w);
        }

        public void Scale(double value)
        {
            x *= value;
            y *= value;
            z *= value;
            w *= value;
        }

        public void Set(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public double Dot(aQuaternion q)
        {
            return w * q.w + x * q.x + y * q.y + z * q.z;
        }

        public void Mul(aQuaternion q)
        {
            x = w * q.x + x * q.w + y * q.z - z * q.y;
            y = w * q.y + y * q.w + z * q.x - x * q.z;
            z = w * q.z + z * q.w + x * q.y - y * q.x;
            w = w * q.w - x * q.x - y * q.y - z * q.z;
        }

        public aQuaternion Normalised
        {
            get
            {
                aQuaternion q = this;
                q.Normalise();
                return q;
            }
        }

        public void Normalise()
        {
            double sqrLen = SqrMagnitude;
            if (sqrLen == 0.0)
            {
                return;
            }
            Scale(1.0 / Math.Sqrt(sqrLen));
        }

        public static aQuaternion FromToRotation(Vector3d from, Vector3d to)
        {
            aQuaternion quat = Identity;
            Vector3d v0 = from;
            v0.Normalize();
            Vector3d v1 = to;
            v1.Normalize();

            double d = v0.Dot(v1);
            if (1.0 <= d)
            {
                quat.Set(1.0, 0.0, 0.0, 0.0);
            }
            else if (1e-6 - 1.0 > d)
            {
                Vector3d axis = Vector3d.Cross(Vector3d.UnitX, from);
                if (axis == Vector3d.Zero)
                {
                    axis = Vector3d.Cross(Vector3d.UnitY, from);
                }
                axis.Normalize();
                quat = FromAngleAxis(axis, Math.PI);
            }
            else
            {
                float s = (float)Math.Sqrt(2.0 * (1 + d));
                float invs = 1 / s;
                Vector3d c = Vector3d.Cross(v0, v1);
                quat.Set(invs * c.x, invs * c.y, invs * c.z, s * 0.5f);
                quat.Normalise();
            }

            return quat;
        }

        public static aQuaternion FromAxes(Vector3d xaxis, Vector3d yaxis, Vector3d zaxis)
        {
            aQuaternion quat = new aQuaternion();
            double[,] kRot = new double[3, 3];
            double fTrace, fRoot;
            int[] s_iNext = { 1, 2, 0 };
            int i, j, k;

            kRot[0, 0] = xaxis.x;
            kRot[1, 0] = xaxis.y;
            kRot[2, 0] = xaxis.z;

            kRot[0, 1] = yaxis.x;
            kRot[1, 1] = yaxis.y;
            kRot[2, 1] = yaxis.z;

            kRot[0, 2] = zaxis.x;
            kRot[1, 2] = zaxis.y;
            kRot[2, 2] = zaxis.z;

            fTrace = kRot[0, 0] + kRot[1, 1] + kRot[2, 2];
            if (fTrace > 0.0)
            {
                // |w| > 1/2, may as well choose w > 1/2
                fRoot = (float)Math.Sqrt(fTrace + 1.0);  // 2w
                quat.w = 0.5 * fRoot;
                fRoot = 0.5 / fRoot;  // 1/(4w)
                quat.x = (kRot[2, 1] - kRot[1, 2]) * fRoot;
                quat.y = (kRot[0, 2] - kRot[2, 0]) * fRoot;
                quat.z = (kRot[1, 0] - kRot[0, 1]) * fRoot;
            }
            else
            {
                i = 0;
                if (kRot[1, 1] > kRot[0, 0])
                    i = 1;
                if (kRot[2, 2] > kRot[i, i])
                    i = 2;

                j = s_iNext[i];
                k = s_iNext[j];

                fRoot = Math.Sqrt(kRot[i, i] - kRot[j, j] - kRot[k, k] + 1.0);

                _setQuatIndex(ref quat, i, 0.5f * fRoot);
                fRoot = 0.5f / fRoot;
                quat.w = (kRot[k, j] - kRot[j, k]) * fRoot;
                _setQuatIndex(ref quat, j, (kRot[j, i] + kRot[i, j]) * fRoot);
                _setQuatIndex(ref quat, k, (kRot[k, i] + kRot[i, k]) * fRoot);
            }
            return quat;
        }

        public static aQuaternion FromAngleAxis(Vector3d axis, double radAngle)
        {
            aQuaternion quat;

            double halfAng = 0.5 * radAngle;
            double s = Math.Sin(halfAng);

            quat.w = Math.Cos(halfAng);
            quat.x = s * axis.x;
            quat.y = s * axis.y;
            quat.z = s * axis.z;

            return quat;
        }

        public static aQuaternion FromRotationMatrix(ref Matrix4X4 src)
        {
            aQuaternion quat = Identity;
            float fRoot;
            float fTrace = src.m00 + src.m11 + src.m22;

            if (0.0f < fTrace)
            {/* |this.w| > 1/2, may as well choose this.w > 1/2 */
                fRoot = (float)Math.Sqrt(1.0f + fTrace);  /* 2 * this.w */
                quat.w = 0.5f * fRoot;
                fRoot = 0.5f / fRoot;  /* 1/(4 * this.w) */
                quat.x = fRoot * (src.m21 - src.m12);
                quat.y = fRoot * (src.m02 - src.m20);
                quat.z = fRoot * (src.m10 - src.m01);
            }
            else
            {/* |this.w| <= 1/2 */
                int i = 0, j = 0, k = 0;
                int[] s_iNext = { 1, 2, 0 };
                if (src.m11 > src.m00)
                    i = 1;
                if (src.m22 > src[i, i])
                    i = 2;
                j = s_iNext[i];
                k = s_iNext[j];

                fRoot = (float)Math.Sqrt(1.0f + src[i, i] - src[j, j] - src[k, k]);
                _setQuatIndex(ref quat, i, 0.5f * fRoot);
                fRoot = 0.5f / fRoot;
                quat.w = fRoot * (src[k, j] - src[j, k]);
                _setQuatIndex(ref quat, j, fRoot * (src[j, i] + src[i, j]));
                _setQuatIndex(ref quat, k, fRoot * (src[k, i] + src[i, k]));
            }

            return quat;
        }

        public static aQuaternion Lerp(aQuaternion from, aQuaternion to, double frac, bool isShortest = false)
        {
            aQuaternion temp = Identity;
            double fCos = from.Dot(to);
            if (0.0 > fCos && isShortest)
            {/* dst = from + frac * ((-to) - from) */
                temp.Set(0.0f, 0.0f, 0.0f, 0.0f);
                temp = temp - to;
                temp = temp - from;
            }
            else
            {/* dst = from + frac * (to - from) */
                temp = to - from;
            }

            temp.Scale(frac);
            temp = from + temp;
            temp.Normalise();
            return temp;
        }

        private static void _setQuatIndex(ref aQuaternion quat, int index, double val)
        {
            if (index == 0)
            {
                quat.x = val;
            }
            else if (index == 1)
            {
                quat.y = val;
            }
            else if (index == 2)
            {
                quat.z = val;
            }
        }

        public static aQuaternion Slerp(aQuaternion from, aQuaternion to, double frac, bool isShortest = false)
        {

            aQuaternion temp1, temp2, quat;
            double fCos = from.Dot(to);

            if (fCos < 0.0 && isShortest)
            {
                fCos = -fCos;
                temp1 = new aQuaternion(-to.x, -to.y, -to.z, -to.w);
            }
            else
            {
                temp1 = from;
            }
            if (Math.Abs(fCos) >= 0.99999)
            {
                temp2 = from;
                temp2.Scale(1.0 - frac);
                temp1.Scale(frac);
                temp2 = temp2 + temp1;
                quat = temp2.Normalised;
            }
            else
            {

                double fSin = (float)Math.Sqrt(1.0 - fCos * fCos);
                double fRadAngle = (float)Math.Atan2(fSin, fCos);
                double fInvSin = 1.0 / fSin;
                double fCoeff0 = fInvSin * Math.Sin((1.0 - frac) * fRadAngle);
                double fCoeff1 = fInvSin * Math.Sin(frac * fRadAngle);
                temp2 = from;
                temp2.Scale(fCoeff0);
                temp1.Scale(fCoeff1);
                quat = temp2 + temp1;
            }
            return quat;
        }

        public static aQuaternion operator +(aQuaternion src1, aQuaternion src2)
        {
            return new aQuaternion(src1.x + src2.x, src1.y + src2.y, src1.z + src2.z, src1.w + src2.w);
        }
        public static aQuaternion operator -(aQuaternion src1, aQuaternion src2)
        {
            return new aQuaternion(src1.x - src2.x, src1.y - src2.y, src1.z - src2.z, src1.w - src2.w);
        }
        public static aQuaternion operator -(aQuaternion src1)
        {
            return new aQuaternion(-src1.x, -src1.y, -src1.z, -src1.w);
        }
        public static aQuaternion operator *(aQuaternion src1, aQuaternion src2)
        {
            aQuaternion q = src1;
            q.Mul(src2);
            return q;
        }

        public static aQuaternion operator *(aQuaternion src1, double src2)
        {
            return new aQuaternion(src1.x * src2, src1.y * src2, src1.z * src2, src1.w * src2);
        }
        public static aQuaternion operator *(double src1, aQuaternion src2)
        {
            return new aQuaternion(src1 * src2.x, src1 * src2.y, src1 * src2.z, src1 * src2.w);
        }

        public static Vector3d operator *(aQuaternion rotate, Vector3d src)
        {
            Vector3d qvec = new Vector3d(rotate.x, rotate.y, rotate.z);
            Vector3d uv = Vector3d.Cross(qvec, src);
            Vector3d uuv = Vector3d.Cross(qvec, uv);
            uv.Scale(2.0f * rotate.w);
            uuv.Scale(2.0f);
            Vector3d res = src + uv;
            res.Add(uuv);

            return res;
        }

        public static bool operator ==(aQuaternion src1, aQuaternion src2)
        {
            return (src1 - src2).SqrMagnitude < equalNum;
        }

        public static bool operator !=(aQuaternion src1, aQuaternion src2)
        {
            return (src1 - src2).SqrMagnitude >= equalNum;
        }

        public override bool Equals(object obj)
        {
            return (obj is aQuaternion) ? (x == ((aQuaternion)obj).x && y == ((aQuaternion)obj).y && z == ((aQuaternion)obj).z && w == ((aQuaternion)obj).w) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
