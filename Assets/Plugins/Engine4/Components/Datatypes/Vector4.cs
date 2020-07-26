using Engine4.Internal;
using System;
using System.Runtime.InteropServices;

namespace Engine4
{
    /// <summary>
    /// Vector in 4D.
    /// </summary>
    /// <remarks>
    /// Not to be confused with UnityEngine's Vector4.
    /// For all operation in Engine4, Vector4 refers to this struct.
    /// We create the duplicate because UnityEngine's variant is lack of static utilities.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4 : IEquatable<Vector4>
    {

        /// <summary>
        /// X axis of the vector.
        /// </summary>
        public float x;

        /// <summary>
        /// Y axis of the vector.
        /// </summary>
        public float y;

        /// <summary>
        /// Z axis of the vector.
        /// </summary>
        public float z;

        /// <summary>
        /// W axis of the vector.
        /// </summary>
        public float w;
        
        /// <summary>
        /// Create uniform vector.
        /// </summary>
        public Vector4(float scale)
        {
            x = y = z = w = scale;
        }

        /// <summary>
        /// Create a new vector data with given individual values.
        /// </summary>
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// Create an euler data with a value for given axis index.
        /// </summary>       
        /// <remarks>See <see cref="this[int]"/> for axis index. Other axis will set as zero by default</remarks>
        public Vector4(int axis, float value)
        {
            x = y = z = w = 0;
            switch (axis)
            {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                case 3: w = value; break;
            }
        }


        /// <summary>
        /// Get or set an axis value of specified index
        /// </summary>
        /// <remarks>
        /// Axis index in order: X, Y, Z, W.
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
                    case 3: return w;
                    default: throw new IndexOutOfRangeException(); // DISCUSS: This throw prevents inlinig!
                }
            }
            set
            {
                switch (i)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default: throw new IndexOutOfRangeException(); // DISCUSS: This throw prevents inlinig!
                }
            }
        }

        /// <summary>
        /// Negate each vector axis.
        /// </summary>
        static public Vector4 operator -(Vector4 lhs)
        {
            return new Vector4(-lhs.x, -lhs.y, -lhs.z, -lhs.w);
        }

        /// <summary>
        /// Axis-wisely add two vector values.
        /// </summary>
        static public Vector4 operator +(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        }

        /// <summary>
        /// Axis-wisely subtract two vector values.
        /// </summary>
        static public Vector4 operator -(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        }
        
        /// <summary>
        /// Axis-wisely multiply two vector values.
        /// </summary>
        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        /// <summary>
        /// Axis-wisely scale vector values with a number.
        /// </summary>
        static public Vector4 operator *(Vector4 lhs, float f)
        {
            return new Vector4(lhs.x * f, lhs.y * f, lhs.z * f, lhs.w * f);
        }

        /// <summary>
        /// Axis-wisely divide vector values with a number.
        /// </summary>
        static public Vector4 operator /(Vector4 lhs, float f)
        {
            return new Vector4(lhs.x / f, lhs.y / f, lhs.z / f, lhs.w / f);
        }
        
        /// <summary>
        /// Compare if A is greater than B in all axes.
        /// </summary>
        static public bool operator >(Vector4 lhs, Vector4 rhs)
        {
            return lhs.x > rhs.x && lhs.y > rhs.y && lhs.z > rhs.z && lhs.w > rhs.w;
        }

        /// <summary>
        /// Compare if A is smaller than B in all axes.
        /// </summary>
        static public bool operator <(Vector4 lhs, Vector4 rhs)
        {
            return lhs.x < rhs.x && lhs.y < rhs.y && lhs.z < rhs.z && lhs.w < rhs.w;
        }

        /// <summary>
        /// Equivalence check.
        /// </summary>
        static public bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return LengthSq(rhs - lhs) < 1e-4;
        }

        /// <summary>
        /// Inequivalence check.
        /// </summary>
        static public bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return LengthSq(rhs - lhs) > 1e-4;
        }

        /// <summary>
        /// Vector with zero values.
        /// </summary>
        public static Vector4 zero { get { return new Vector4(); } }

        /// <summary>
        /// Vector with values of one.
        /// </summary>
        public static Vector4 one { get { return new Vector4(1); } }
        
        /// <summary>
        /// Get the dot operation between two vector.
        /// </summary>
        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// Get the cross operation between three vectors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This cross operation finds another vector which perpendicular to those three vectors.
        /// The convention used such that A and B is equivalent to the left hand rule if C is positive overward.
        /// </para>
        /// </remarks>
        public static Vector4 Cross(Vector4 a, Vector4 b, Vector4 c)
        {
            float A, B, C, D, E, F; // Intermediate Values 

            Vector4 r;
            A = (b.x * c.y) - (b.y * c.x);
            B = (b.x * c.z) - (b.z * c.x);
            C = (b.x * c.w) - (b.w * c.x);
            D = (b.y * c.z) - (b.z * c.y);
            E = (b.y * c.w) - (b.w * c.y);
            F = (b.z * c.w) - (b.w * c.z);

            r.x = (a.y * F) - (a.z * E) + (a.w * D);
            r.y = -(a.x * F) + (a.z * C) - (a.w * B);
            r.z = (a.x * E) - (a.y * C) + (a.w * A);
            r.w = -(a.x * D) + (a.y * B) - (a.z * A);

            return r;
        }

        /// <summary>
        /// Normalize the vector.
        /// </summary>
        public static Vector4 Normalize(Vector4 v)
        {
            float l = LengthSq(v);

            // Avoid calculation when length = 1 (and zero)
            if (l == 1) return v;
            else if (l == 0) return new Vector4(3, 1);
            else return v / (float)Math.Sqrt(l);
        }

        /// <summary>
        /// Interpolate two vector by T and clamp.
        /// </summary>
        /// <remarks> The interpolation is clamped between zero to one </remarks>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return a + (b - a) * Utility.Clamp01(t);
        }

        /// <summary>
        /// Interpolate two vector by T.
        /// </summary>
        /// <remarks> The interpolation is **not** clamped. </remarks>
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t)
        {
            return a + (b - a) * t;
        }


        /// <summary>
        /// Get the length (magnitude) of the vector.
        /// </summary>
        public static float Length(Vector4 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w);
        }

        /// <summary>
        /// Get the squared length of the vector
        /// </summary>
        public static float LengthSq(Vector4 v)
        {
            return v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w;
        }

        /// <summary>
        /// Get the distance between two vector.
        /// </summary>
        public static float Distance(Vector4 a, Vector4 b)
        {
            return Length(b - a);
        }

        /// <summary>
        /// Get the squared distance between two vector.
        /// </summary>
        public static float DistanceSq(Vector4 a, Vector4 b)
        {
            return LengthSq(b - a);
        }

        /// <summary>
        /// Get the absolute version of the vector.
        /// </summary>
        public static Vector4 Abs(Vector4 v)
        {
            return new Vector4(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z), Math.Abs(v.w));
        }

        /// <summary>
        /// Get the sign version of the vector.
        /// </summary>
        public static Vector4 Sign(Vector4 n)
        {
            return new Vector4(Utility.Sign(n.x), Utility.Sign(n.y), Utility.Sign(n.z), Utility.Sign(n.w));
        }

        /// <summary>
        /// Clamp vector T between A and B.
        /// </summary>
        public static Vector4 Clamp(Vector4 a, Vector4 b, Vector4 t)
        {
            return new Vector4(
                Utility.Clamp(a.x, b.x, t.x),
                Utility.Clamp(a.y, b.y, t.y),
                Utility.Clamp(a.z, b.z, t.z),
                Utility.Clamp(a.w, b.w, t.w)
                );
        }

        /// <summary>
        /// Axis-wisely inverse the vector.
        /// </summary>
        public static Vector4 Invert(Vector4 v)
        {
            return new Vector4(1.0f / v.x, 1.0f / v.y, 1.0f / v.z, 1.0f / v.w);
        }


        /// <summary>
        /// Axis-wisely choose the smallest range of two vector.
        /// </summary>
        public static Vector4 Min(Vector4 a, Vector4 b)
        {
            return new Vector4(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z), Math.Min(a.w, b.w));
        }

        /// <summary>
        /// Axis-wisely choose the largest range of two vector.
        /// </summary>
        public static Vector4 Max(Vector4 a, Vector4 b)
        {
            return new Vector4(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z), Math.Max(a.w, b.w));
        }

        /// <summary>
        /// Return the smallest axis value in the vector.
        /// </summary>
        public static float MinPerElem(Vector4 a)
        {
            return Math.Min(Math.Min(a.x, a.w), Math.Min(a.y, a.z));
        }

        /// <summary>
        /// Return the largest axis value in the vector.
        /// </summary>
        public static float MaxPerElem(Vector4 a)
        {
            return Math.Max(Math.Max(a.x, a.w), Math.Max(a.y, a.z));
        }

        /// <summary>
        /// Return the axis index that has smallest value in the vector.
        /// </summary>
        public static int MinPerElemIdx(Vector4 a)
        {
            return a.x < a.y && a.x < a.z && a.x < a.w ? 0 : (a.y < a.z && a.y < a.w ? 1 : (a.z < a.w ? 2 : 3));
        }

        /// <summary>
        /// Return the axis index that has largest value in the vector.
        /// </summary>
        public static int MaxPerElemIdx(Vector4 a)
        {
            return a.x > a.y && a.x > a.z && a.x > a.w ? 0 : (a.y > a.z && a.y > a.w ? 1 : (a.z > a.w ? 2 : 3));
        }

        /// <summary>
        /// Convert to string for debugging ease.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{{ {0:F4}, {1:F4}, {2:F4}, {3:F4} }}", x, y, z, w);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ComputeBasis(Vector4[] vector)
        {
            var m = Matrix4.LookAt(vector[0]);
            vector[1] = m.ex;
            vector[2] = m.ey;
            vector[3] = m.ez;
        }


        /// <summary>
        /// Implicit conversion to UnityEngine's Vector4
        /// </summary>
        public static implicit operator UnityEngine.Vector4(Vector4 v)
        {
            return new UnityEngine.Vector4() { x = v.x, y = v.y, z = v.z, w = v.w };
        }

        /// <summary>
        /// Implicit conversion from UnityEngine's Vector3
        /// </summary>
        public static implicit operator UnityEngine.Vector3(Vector4 v)
        {
            return new UnityEngine.Vector3() { x = v.x, y = v.y, z = v.z };
        }

        /// <summary>
        /// Implicit conversion from UnityEngine's Vector4
        /// </summary>
        public static implicit operator Vector4(UnityEngine.Vector4 v)
        {
            return new Vector4() { x = v.x, y = v.y, z = v.z, w = v.w };
        }

        /// 
        [HideInDocs]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// 
        [HideInDocs]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// 
        [HideInDocs]
        public bool Equals(Vector4 other)
        {
            return this == other;
        }

        /// <summary>
        /// The vector pointing to right direction
        /// </summary>
        public static Vector4 rightward { get { return new Vector4(1, 0, 0, 0); } }
        /// <summary>
        /// The vector pointing to up direction
        /// </summary>
        public static Vector4 upward { get { return new Vector4(0, 1, 0, 0); } }
        /// <summary>
        /// The vector pointing to front direction
        /// </summary>
        public static Vector4 forward { get { return new Vector4(0, 0, 1, 0); } }
        /// <summary>
        /// The vector pointing to over direction
        /// </summary>
        public static Vector4 overward { get { return new Vector4(0, 0, 0, 1); } }
    }
}
