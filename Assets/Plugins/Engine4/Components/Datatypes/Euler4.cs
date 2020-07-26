using UnityEngine;
using System;
using Engine4.Internal;

namespace Engine4
{
    /// <summary>
    /// Rotation unit on each plane in 4D.
    /// </summary>
    /// <remarks>
    /// Euler4 is basically a vector with 6 axis (plane), and can be manipulated like other vector.
    /// In most cases when dealing with 4D rotations, you'll use Euler4, since it can be 
    /// represented as an euler rotation, or angular velocity of an object.
    /// </remarks>
    /// <seealso cref="Matrix4.ToEuler()"/><seealso cref="Rigidbody.angularVelocity"/>
    [Serializable]
    public struct Euler4 : IEquatable<Euler4>
    {
        /// <summary> YZ Plane </summary>
        public float x;
        /// <summary> ZX Plane </summary>
        public float y;
        /// <summary> XY Plane </summary>
        public float z;
        /// <summary> XW Plane </summary>
        public float t;
        /// <summary> YW Plane </summary>
        public float u;
        /// <summary> ZW Plane </summary>
        public float v;

        /// <summary>
        /// Create an euler data with given individual values.
        /// </summary>
        public Euler4(float x, float y, float z, float t, float u, float v)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.t = t;
            this.u = u;
            this.v = v;
        }

        /// <summary>
        /// Create new euler with given individual values from a pair of Vector3.
        /// </summary>
        public Euler4(Vector3 xyz, Vector3 tuv)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
            t = tuv.x;
            u = tuv.y;
            v = tuv.z;
        }

        /// <summary>
        /// Create new euler with a value for given axis index.
        /// </summary>       
        /// <remarks>See <see cref="this[int]"/> for axis index. Other axis will set as zero by default</remarks>
        public Euler4(int axis, float value)
        {
            x = y = z = t = u = v = 0;
            this[axis] = value;
        }

        /// <summary>
        /// Get or set a vector value in given axis index
        /// </summary>
        /// <remarks>
        /// Axis index in order: X, Y, Z, T, U, V
        /// </remarks>
        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return t;
                    case 4: return u;
                    case 5: return v;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: t = value; break;
                    case 4: u = value; break;
                    case 5: v = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Negate each euler axis
        /// </summary>
        static public Euler4 operator -(Euler4 lhs)
        {
            return new Euler4(-lhs.x, -lhs.y, -lhs.z, -lhs.t, -lhs.u, -lhs.v);
        }

        /// <summary>
        /// Axis-wisely add two euler values
        /// </summary>
        static public Euler4 operator +(Euler4 lhs, Euler4 rhs)
        {
            return new Euler4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.t + rhs.t, lhs.u + rhs.u, lhs.v + rhs.v);
        }

        /// <summary>
        /// Axis-wisely subtract two euler values
        /// </summary>
        static public Euler4 operator -(Euler4 lhs, Euler4 rhs)
        {
            return new Euler4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.t - rhs.t, lhs.u - rhs.u, lhs.v - rhs.v);
        }
        
        /// <summary>
        /// Axis-wisely scale both euler.
        /// </summary>
        public static Euler4 operator *(Euler4 a, Euler4 b)
        {
            return new Euler4(a.x * b.x, a.y * b.y, a.z * b.z, a.t * b.t, a.u * b.u, a.v * b.v);
        }

        /// <summary>
        /// Axis-wisely scale an euler values with a number
        /// </summary>
        static public Euler4 operator *(Euler4 lhs, float f)
        {
            return new Euler4(lhs.x * f, lhs.y * f, lhs.z * f, lhs.t * f, lhs.u * f, lhs.v * f);
        }
        
        /// <summary>
        /// Axis-wisely divide an euler values with a number
        /// </summary>
        static public Euler4 operator /(Euler4 lhs, float f)
        {
            return lhs * (1 / f);
        }

        /// <summary>
        /// Euler with zero values
        /// </summary>
        public static Euler4 zero { get { return new Euler4(); } }

        /// <summary>
        /// Get XYZ part of the euler
        /// </summary>
        public Vector3 xyz { get { return new Vector3(x, y, z); } }

        /// <summary>
        /// Get TUV part if the euler
        /// </summary>
        public Vector3 tuv { get { return new Vector3(t, u, v); } }
        
        /// <summary>
        /// Equivalence check 
        /// </summary>
        static public bool operator ==(Euler4 lhs, Euler4 rhs)
        {
            return LengthSq(rhs - lhs) < 1e-5f;
        }

        /// <summary>
        /// Inequivalence check 
        /// </summary>
        static public bool operator !=(Euler4 lhs, Euler4 rhs)
        {
            return LengthSq(rhs - lhs) > 1e-5f;
        }

        /// <summary>
        /// Get the squared length of the euler
        /// </summary>
        public static float LengthSq(Euler4 v)
        {
            return Dot(v, v);
        }

        /// <summary>
        /// Get the dot product of the euler
        /// </summary>
        public static float Dot(Euler4 a, Euler4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.t * b.t + a.u * b.u + a.v * b.v;
        }

        /// <summary>
        /// Interpolate euler rotation degree from A to B by T in each axes
        /// </summary>
        /// <remarks> This method does loop back from 360 to zero if that's necessary. The interpolation is not clampled. </remarks>
        public static Euler4 LerpAngle(Euler4 a, Euler4 b, float t)
        {
            return new Euler4(LerpAngle(a.x, b.x, t), LerpAngle(a.y, b.y, t), LerpAngle(a.z, b.z, t),
                LerpAngle(a.t, b.t, t), LerpAngle(a.u, b.u, t), LerpAngle(a.v, b.v, t));
        }

        static float LerpAngle(float a, float b, float t)
        {
            return a == b ? a : a + Mathf.DeltaAngle(a, b) * t;
        }

        /// <summary>
        /// Creates an euler rotation from two vector
        /// </summary>
        /// <remarks> 
        /// This method is equivalent to the wedge operation between vector and vector in Geometry Algebra. It is used by internal physics engine. 
        /// </remarks>
        public static Euler4 Cross(Vector4 a, Vector4 b)
        {
            return new Euler4(
                (a.y * b.z) - (b.y * a.z),
                (a.z * b.x) - (b.z * a.x),
                (a.x * b.y) - (b.x * a.y),
                (a.x * b.w) - (b.x * a.w),
                (a.y * b.w) - (b.y * a.w),
                (a.z * b.w) - (b.z * a.w)
                );
        }

        /// <summary>
        /// Rotates the vector by euler rotation
        /// </summary>
        /// <remarks> 
        /// This method is equivalent to the wedge operation between vector and vector in Geometry Algebra. It is used by internal physics engine. 
        /// </remarks>
        internal static Vector4 Cross(Euler4 a, Vector4 b)
        {
            return new Vector4(
                 (a.y * b.z) - (a.z * b.y) - (a.t * b.w),
                 (a.z * b.x) - (a.x * b.z) - (a.u * b.w),
                 (a.x * b.y) - (a.y * b.x) - (a.v * b.w),
                 (a.t * b.x) + (a.u * b.y) + (a.v * b.z));
        }

        // Internally used for Tensor inversion
        internal static Euler4 Cross(Euler4 a, Euler4 b, Euler4 c, Euler4 d, Euler4 e)
        {

            var A4545 = d[4] * e[5] - d[5] * e[4];
            var A3545 = d[3] * e[5] - d[5] * e[3];
            var A3445 = d[3] * e[4] - d[4] * e[3];
            var A2545 = d[2] * e[5] - d[5] * e[2];
            var A2445 = d[2] * e[4] - d[4] * e[2];
            var A2345 = d[2] * e[3] - d[3] * e[2];
            var A1545 = d[1] * e[5] - d[5] * e[1];
            var A1445 = d[1] * e[4] - d[4] * e[1];
            var A1345 = d[1] * e[3] - d[3] * e[1];
            var A1245 = d[1] * e[2] - d[2] * e[1];
            var A0545 = d[0] * e[5] - d[5] * e[0];
            var A0445 = d[0] * e[4] - d[4] * e[0];
            var A0345 = d[0] * e[3] - d[3] * e[0];
            var A0245 = d[0] * e[2] - d[2] * e[0];
            var A0145 = d[0] * e[1] - d[1] * e[0];

            var B345345 = c[3] * A4545 - c[4] * A3545 + c[5] * A3445;
            var B245345 = c[2] * A4545 - c[4] * A2545 + c[5] * A2445;
            var B235345 = c[2] * A3545 - c[3] * A2545 + c[5] * A2345;
            var B234345 = c[2] * A3445 - c[3] * A2445 + c[4] * A2345;
            var B145345 = c[1] * A4545 - c[4] * A1545 + c[5] * A1445;
            var B135345 = c[1] * A3545 - c[3] * A1545 + c[5] * A1345;
            var B134345 = c[1] * A3445 - c[3] * A1445 + c[4] * A1345;
            var B125345 = c[1] * A2545 - c[2] * A1545 + c[5] * A1245;
            var B124345 = c[1] * A2445 - c[2] * A1445 + c[4] * A1245;
            var B123345 = c[1] * A2345 - c[2] * A1345 + c[3] * A1245;
            var B045345 = c[0] * A4545 - c[4] * A0545 + c[5] * A0445;
            var B035345 = c[0] * A3545 - c[3] * A0545 + c[5] * A0345;
            var B034345 = c[0] * A3445 - c[3] * A0445 + c[4] * A0345;
            var B025345 = c[0] * A2545 - c[2] * A0545 + c[5] * A0245;
            var B024345 = c[0] * A2445 - c[2] * A0445 + c[4] * A0245;
            var B023345 = c[0] * A2345 - c[2] * A0345 + c[3] * A0245;
            var B015345 = c[0] * A1545 - c[1] * A0545 + c[5] * A0145;
            var B014345 = c[0] * A1445 - c[1] * A0445 + c[4] * A0145;
            var B013345 = c[0] * A1345 - c[1] * A0345 + c[3] * A0145;
            var B012345 = c[0] * A1245 - c[1] * A0245 + c[2] * A0145;

            var C23452345 = b[2] * B345345 - b[3] * B245345 + b[4] * B235345 - b[5] * B234345;
            var C13452345 = b[1] * B345345 - b[3] * B145345 + b[4] * B135345 - b[5] * B134345;
            var C12452345 = b[1] * B245345 - b[2] * B145345 + b[4] * B125345 - b[5] * B124345;
            var C12352345 = b[1] * B235345 - b[2] * B135345 + b[3] * B125345 - b[5] * B123345;
            var C12342345 = b[1] * B234345 - b[2] * B134345 + b[3] * B124345 - b[4] * B123345;
            var C03452345 = b[0] * B345345 - b[3] * B045345 + b[4] * B035345 - b[5] * B034345;
            var C02452345 = b[0] * B245345 - b[2] * B045345 + b[4] * B025345 - b[5] * B024345;
            var C02352345 = b[0] * B235345 - b[2] * B035345 + b[3] * B025345 - b[5] * B023345;
            var C02342345 = b[0] * B234345 - b[2] * B034345 + b[3] * B024345 - b[4] * B023345;
            var C01452345 = b[0] * B145345 - b[1] * B045345 + b[4] * B015345 - b[5] * B014345;
            var C01352345 = b[0] * B135345 - b[1] * B035345 + b[3] * B015345 - b[5] * B013345;
            var C01342345 = b[0] * B134345 - b[1] * B034345 + b[3] * B014345 - b[4] * B013345;
            var C01252345 = b[0] * B125345 - b[1] * B025345 + b[2] * B015345 - b[5] * B012345;
            var C01242345 = b[0] * B124345 - b[1] * B024345 + b[2] * B014345 - b[4] * B012345;
            var C01232345 = b[0] * B123345 - b[1] * B023345 + b[2] * B013345 - b[3] * B012345;

            return new Euler4(
              (a[1] * C23452345 - a[2] * C13452345 + a[3] * C12452345 - a[4] * C12352345 + a[5] * C12342345),
              -(a[0] * C23452345 - a[2] * C03452345 + a[3] * C02452345 - a[4] * C02352345 + a[5] * C02342345),
              (a[0] * C13452345 - a[1] * C03452345 + a[3] * C01452345 - a[4] * C01352345 + a[5] * C01342345),
              -(a[0] * C12452345 - a[1] * C02452345 + a[2] * C01452345 - a[4] * C01252345 + a[5] * C01242345),
              (a[0] * C12352345 - a[1] * C02352345 + a[2] * C01352345 - a[3] * C01252345 + a[5] * C01232345),
             -(a[0] * C12342345 - a[1] * C02342345 + a[2] * C01342345 - a[3] * C01242345 + a[4] * C01232345));
        }

        /// <summary>
        /// Convert to string for debugging ease.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{{ {0:F2}; {1:F2}; {2:F2}; {3:F2}; {4:F2}; {5:F2} }}", x, y, z, t, u, v);
        }

        /// 
        [HideInDocs]
        public override bool Equals(object obj)
        {
            return obj is Euler4 && ((Euler4)obj == this);
        }

        /// 
        [HideInDocs]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// 
        [HideInDocs]
        public bool Equals(Euler4 other)
        {
            return this == other;
        }
    }
}