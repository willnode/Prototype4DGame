using Engine4.Internal;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Math = UnityEngine.Mathf;

namespace Engine4
{
    /// <summary>
    /// A 4x4 matrix to describe 4D rotations.
    /// </summary>
    /// <remarks>
    /// The matrix is represented in row major order.
    /// Most operation that available requires the matrix to be orthogonal.
    /// This should not be confused with Unity's Matrix4x4.
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 : IEquatable<Matrix4>
    {
        /// <summary>
        /// First row of the matrix
        /// </summary>
        public Vector4 ex;
        /// <summary>
        /// Second row of the matrix
        /// </summary>
        public Vector4 ey;
        /// <summary>
        /// Third row of the matrix
        /// </summary>
        public Vector4 ez;
        /// <summary>
        /// Fourth row of the matrix
        /// </summary>
        public Vector4 ew;



        /// <summary>
        /// Create a matrix data with given individual values.
        /// </summary>
        public Matrix4(float a, float b, float c, float d, float e, float f, float g, float h,
                            float i, float j, float k, float l, float m, float n, float o, float p)
        {
            ex = new Vector4(a, b, c, d);
            ey = new Vector4(e, f, g, h);
            ez = new Vector4(i, j, k, l);
            ew = new Vector4(m, n, o, p);
        }

        /// <summary>
        /// Create a diagonal matrix with given scaling value.
        /// </summary>
        public Matrix4(Vector4 scale)
        {
            ex = new Vector4(0, scale.x);
            ey = new Vector4(1, scale.y);
            ez = new Vector4(2, scale.z);
            ew = new Vector4(3, scale.w);
        }

        /// <summary>
        /// Create a matrix data with given individual values (in vector form).
        /// </summary>
        public Matrix4(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
        {
            ex = (x);
            ey = (y);
            ez = (z);
            ew = (w);
        }

        /// <summary>
        /// Get a matrix element at given position
        /// </summary>
        public float this[int row, int column]
        {
            get { return this[row][column]; }
            set
            {
                var r = this[row];
                r[column] = value;
                this[row] = r;
            }
        }

        Euler4 ToEulerInSingularity()
        {
            if (ex.x < 0)
            {
                if (ey.y < 0) return new Euler4(2, Utility.Atan2(ey.x, ex.x) * Math.Rad2Deg); // Z
                if (ez.z < 0) return new Euler4(1, Utility.Atan2(ex.z, ex.x) * Math.Rad2Deg); // Y
                if (ew.w < 0) return new Euler4(3, Utility.Atan2(ew.x, ex.x) * Math.Rad2Deg); // T
            }
            else if (ey.y < 0)
            {
                if (ez.z < 0) return new Euler4(0, Utility.Atan2(ez.y, ey.y) * Math.Rad2Deg); // X
                if (ew.w < 0) return new Euler4(4, Utility.Atan2(ew.y, ey.y) * Math.Rad2Deg); // U
            }
            else if (ez.z < 0 && ew.w < 0)
            {
                return new Euler4(5, Utility.Atan2(ew.z, ez.z) * Math.Rad2Deg); // V
            }
            // (all diag is positive)
            return new Euler4(Math.Asin(ez.y), Math.Asin(ex.z), Math.Asin(ey.x),
                Math.Asin(ew.x), Math.Asin(ew.y), Math.Asin(ew.z)) * Math.Rad2Deg;
        }

        Euler4 ToEulerInHalfSingularity()
        {
            if (ex.x < 0)
            {
                // 90 deg of X, U, V

            }
            else if (ey.y < 0)
            {
                if (ez.z < 0) return new Euler4(0, Utility.Atan2(ey.z, ey.y) * Math.Rad2Deg); // X
                if (ew.w < 0) return new Euler4(4, Utility.Atan2(ew.y, ey.y) * Math.Rad2Deg); // U
            }
            else if (ez.z < 0 && ew.w < 0)
            {
                return new Euler4(5, Utility.Atan2(ew.z, ez.z) * Math.Rad2Deg); // V
            }
            // (all diag is positive)
            return new Euler4(Math.Asin(ey.z), Math.Asin(ez.x), Math.Asin(ex.y),
                Math.Asin(ew.x), Math.Asin(ew.y), Math.Asin(ew.z)) * Math.Rad2Deg;
        }

        /// <summary>
        /// Convert matrix into euler degree rotation
        /// </summary>
        /// <remarks>
        /// The method is not valid in 90 deg singularity (WIP).
        /// The method won't check for orthogonality.
        /// </remarks>
        public Euler4 ToEuler()
        {
            // Singularity check.
            //var d = Diagonal; var t = d.x * d.y * d.z * d.w; var t2 = Vector4.Dot(d, Vector4.one); t2 = t2 * t2;
            //if (t > 0.99F) return ToEulerInSingularity(); // Because Utility.Atan2AvoidPi doesn't care about 180 deg singularities...
                                                          // if (t2 - 1 < 0.001F) return ToEulerInHalfSingularity(); // 90 deg is bad. Some cos() parameters got zero, and could results in chaos because of losing sign information

            // Back to main topic..
            // Based on Euler() sequence, the raw formula is...
            // matrix{{ach, dh, -bch, -j}, {k(bg-adf)-acjl, cfk-djl, k(bdf+ag)+bcjl, -hl}, {-ln(bg-adf)+m(adg+bf)-acjkn, -cgm-cfln-djkn, -ln(bdf+ag)+m(af-bdg)+bcjkn, -hkn}, {lm(bg-adf)+n(adg+bf)+acjkm, cflm+djkm-cgn, lm(bdf+ag)+n(af-bdg)-bcjkm, hkm}}

            // matrix{{ach, -dh, bch, -j}, {k(adf+bg)-acjl, cfk+djl, k(bdf-ag)-bcjl, -hl}, 
            // { -ln(adf+bg)+m(adg-bf)-acjkn, cgm+djkn-cfln, -ln(bdf-ag)+m(bdg+af)-bcjkn, -hkn}, 
            // { lm(adf+bg)+n(adg-bf)+acjkm, cflm-djkm+cgn, lm(bdf-ag)+n(bdg+af)+bcjkm, hkm}}

            Euler4 result = new Euler4();

            result.y = AngleTidy(Math.Atan2(this[0, 2], this[0, 0])); // ach, bch
            result.v = AngleTidy(Math.Atan2(-this[2, 3], this[3, 3])); // hkm, -hkn

            // (So lucky) We got the first and last matrix sequence.  Now make the matrix simpler
            // Simplify into : matrix{{ch, dh, 0, -j}, {-cjl-dfk, cfk-djl, gk, -hl}, {dg, -cg, f, 0}, {cjk-dfl, cfl+djk, gl, hk}}
            // matrix{{ch, -dh, 0, -j}, {dfk-cjl, cfk+djl, -gk, -hl}, {dg, cg, f, 0}, {cjk+dfl, cfl-djk, -gl, hk}}

            var m2 = Euler(5, -result.v) * this * Euler(1, -result.y);
            result.z = AngleTidy(Math.Atan2(-m2[0, 1], m2[0, 0])); // ch, -dh
            result.u = AngleTidy(Math.Atan2(-m2[1, 3], m2[3, 3])); // hk, -hl

            // Idk if it's safe to get x and t from that matrix, but working > efficient
            // Simplify into : matrix{{h, 0, 0, -j}, {0, f, g, 0}, {0, -g, f, 0}, {j, 0, 0, h}}

            var m3 = Euler(4, -result.u) * m2 * Euler(2, -result.z);
            result.x = AngleTidy(Math.Atan2(m3[2, 1], m3[1, 1])); // f, g
            result.t = AngleTidy(Math.Atan2(-m3[0, 3], m3[0, 0])); // h, -j
            
            return result;
        }

        static float AngleTidy(float v)
        {
            // Normalize and convert to degree
            return (v < 0 ? v + Math.PI * 2 : v) * Math.Rad2Deg;
        }

        /// <summary>
        /// Convert degree euler to orthogonal matrix rotation.
        /// </summary>
        public static Matrix4 Euler(Euler4 rot)
        {
            return Euler(rot.x, rot.y, rot.z, rot.t, rot.u, rot.v);
        }

        /// <summary>
        /// Convert degree euler to orthogonal matrix rotation individually.
        /// </summary>
        /// <remarks>
        /// This creates a rotation matrix that rotates a point by Y, Z, X, T, U, then V. In that order.
        /// </remarks>
        public static Matrix4 Euler(float x, float y, float z, float t, float u, float v)
        {
            // Multiplication matrices, in order
            float y2 = y * Math.Deg2Rad, a = Math.Cos(y2), b = Math.Sin(y2); // CW
            float z2 = z * Math.Deg2Rad, c = Math.Cos(z2), d = Math.Sin(z2); // CCW
            float x2 = x * Math.Deg2Rad, f = Math.Cos(x2), g = Math.Sin(x2); // CCW
            float t2 = t * Math.Deg2Rad, h = Math.Cos(t2), j = Math.Sin(t2); // CCW
            float u2 = u * Math.Deg2Rad, k = Math.Cos(u2), l = Math.Sin(u2); // CCW
            float v2 = v * Math.Deg2Rad, m = Math.Cos(v2), n = Math.Sin(v2); // CCW

            // Premultiplied code
            //var p1 = (b * g + a * d * f);
            //var p2 = (b * d * f + a * g);
            //var p3 = (c * j * k);
            //var p4 = -(d * j * k) + (c * f * l);
            //var p6 = (a * d * g - b * f);
            //var p7 = (a * f - b * d * g);
            //var p8 = (j * l);
            //var p9 = (c * h);
            //var p10 = (c * g);

            // This is corrected version than below (duh)
            return new Matrix4(a * c * h,
                 -d * h,
                 b * c * h,
                 -j,
                 k * (a * d * f + b * g) - a * c * j * l,
                 c * f * k + d * j * l,
                 k * (b * d * f - a * g) - b * c * j * l,
                 -h * l,
                 -l * n * (a * d * f + b * g) + m * (a * d * g - b * f) - a * c * j * k * n,
                 c * g * m + d * j * k * n - c * f * l * n,
                 -l * n * (b * d * f - a * g) + m * (b * d * g + a * f) - b * c * j * k * n,
                 -h * k * n,
                 l * m * (a * d * f + b * g) + n * (a * d * g - b * f) + a * c * j * k * m,
                 c * f * l * m - d * j * k * m + c * g * n,
                 l * m * (b * d * f - a * g) + n * (b * d * g + a * f) + b * c * j * k * m,
                 h * k * m
                 );

            // matrix{{ach, dh, -bch, -j}, {k(bg-adf)-acjl, cfk-djl, k(bdf+ag)+bcjl, -hl}, 
            // { -ln(bg-adf)+m(adg+bf)-acjkn, -cgm-cfln-djkn, -ln(bdf+ag)+m(af-bdg)+bcjkn, -hkn}, 
            // { lm(bg-adf)+n(adg+bf)+acjkm, cflm+djkm-cgn, lm(bdf+ag)+n(af-bdg)-bcjkm, hkm}}

            // matrix{{ach, -dh, bch, -j}, {k(adf+bg)-acjl, cfk+djl, k(bdf-ag)-bcjl, -hl}, 
            // { -ln(adf+bg)+m(adg-bf)-acjkn, cgm+djkn-cfln, -ln(bdf-ag)+m(bdg+af)-bcjkn, -hkn}, 
            // { lm(adf+bg)+n(adg-bf)+acjkm, cflm-djkm+cgn, lm(bdf-ag)+n(bdg+af)+bcjkm, hkm}}

            //return new Matrix4(
            //     a * p9,
            //     -d * h,
            //     b * p9,
            //     -j,
            //     k * p1 - a * c * p8,
            //     c * f * k + d * p8,
            //     k * p2 - b * c * p8,
            //     -h * l,
            //     m * p6 - (a * p3 + l * p1) * n,
            //     p10 * m - p4 * n,
            //     m * p7 + (b * p3 - l * p2) * n,
            //     -h * k * n,
            //     n * p6 + (a * p3 + l * p1) * m,
            //     -p10 * n + p4 * m,
            //      n * p7 - (b * p3 - l * p2) * m,
            //     h * k * m
            //  );

        }

        /// <summary>
        /// Convert given degree rotation in given axis to orthogonal matrix rotation.
        /// </summary>
        /// <remarks>This method is much optimized than Euler(new Euler4(axis, degree))</remarks>
        public static Matrix4 Euler(int axis, float degree)
        {
            float s = Math.Sin(degree * Math.Deg2Rad), c = Math.Cos(degree * Math.Deg2Rad);
            var m = identity;
            switch (axis)
            {
                case 0: m.ey.y = c; m.ez.z = c; m.ey.z = -s; m.ez.y = s; return m;
                case 1: m.ex.x = c; m.ez.z = c; m.ex.z = s; m.ez.x = -s; return m;
                case 2: m.ex.x = c; m.ey.y = c; m.ex.y = -s; m.ey.x = s; return m;
                case 3: m.ex.x = c; m.ew.w = c; m.ex.w = -s; m.ew.x = s; return m;
                case 4: m.ey.y = c; m.ew.w = c; m.ey.w = -s; m.ew.y = s; return m;
                case 5: m.ez.z = c; m.ew.w = c; m.ez.w = -s; m.ew.z = s; return m;
                default: throw new ArgumentOutOfRangeException("axis");
            }
        }

        /// <summary>
        /// Get Nth-row of the matrix
        /// </summary>
        public Vector4 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ex;
                    case 1: return ey;
                    case 2: return ez;
                    case 3: return ew;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: ex = value; break;
                    case 1: ey = value; break;
                    case 2: ez = value; break;
                    case 3: ew = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Multiply or rotate a vector by this matrix
        /// </summary>
        static public Vector4 operator *(Matrix4 lhs, Vector4 rhs)
        {
            return new Vector4(Vector4.Dot(lhs.ex, rhs), Vector4.Dot(lhs.ey, rhs), Vector4.Dot(lhs.ez, rhs), Vector4.Dot(lhs.ew, rhs));
        }

        /// <summary>
        /// Multiply or combine rotations between two matrices.
        /// </summary>
        static public Matrix4 operator *(Matrix4 lhs, Matrix4 rhs)
        {
            return new Matrix4()
            {
                Column0 = (lhs * rhs.Column0),
                Column1 = (lhs * rhs.Column1),
                Column2 = (lhs * rhs.Column2),
                Column3 = (lhs * rhs.Column3),
            };
        }

        /// <summary>
        /// Scales the matrix
        /// </summary>
        /// <remarks>This operation could make the matrix not orthogonal anymore</remarks>
        static public Matrix4 operator *(Matrix4 lhs, float f)
        {
            return new Matrix4(lhs.ex * f, lhs.ey * f, lhs.ez * f, lhs.ew * f);
        }

        /// <summary>
        /// Element-wisely add two matrices.
        /// </summary>
        /// <remarks>This operation could make the matrix not orthogonal anymore</remarks>
        static public Matrix4 operator +(Matrix4 lhs, Matrix4 rhs)
        {
            return new Matrix4(lhs.ex + rhs.ex, lhs.ey + rhs.ey, lhs.ez + rhs.ez, lhs.ew + rhs.ew);
        }

        /// <summary>
        /// Element-wisely subtract two matrices.
        /// </summary>
        /// <remarks>This operation could make the matrix not orthogonal anymore</remarks>
        static public Matrix4 operator -(Matrix4 lhs, Matrix4 rhs)
        {
            return new Matrix4(lhs.ex - rhs.ex, lhs.ey - rhs.ey, lhs.ez - rhs.ez, lhs.ew - rhs.ew);
        }

        /// <summary>
        /// Multiply or rotate a vector by the inversed version of this matrix
        /// </summary>
        public static Vector4 operator /(Vector4 v, Matrix4 r)
        {
            return Transpose(r) * v;
        }
        /// <summary>
        /// Inversely Multiply or combine rotations between two matrices.
        /// </summary>
        /// <remarks> B * A / B returns A </remarks>
        public static Matrix4 operator /(Matrix4 q, Matrix4 r)
        {
            return Transpose(r) * q;
        }

        /// <summary>
        /// Returns the transposed version of the matrix.
        /// </summary>
        /// <remarks> When the matrix is orthogonal, it's equivalent as the inversed version of the matrix </remarks>
        public static Matrix4 Transpose(Matrix4 m)
        {
            return new Matrix4(m.Column0, m.Column1, m.Column2, m.Column3);
        }

        /// <summary>
        /// Perform a sandwich operation on B by A
        /// </summary>
        /// <remarks> The product is a rotation of B that oriented relative to A coordinate axes </remarks>
        public static Matrix4 Transform(Matrix4 a, Matrix4 b)
        {
            return Transpose(a) * b * a;
        }

        /// <summary>
        /// Returns the absolute version of the matrix.
        /// </summary>
        public static Matrix4 Abs(Matrix4 m)
        {
            return new Matrix4(Vector4.Abs(m.ex), Vector4.Abs(m.ey), Vector4.Abs(m.ez), Vector4.Abs(m.ew));
        }

        /// <summary>
        /// Create rotation matrix that rotates *from* matrix with *to* matrix
        /// </summary>
        public static Matrix4 Delta(Matrix4 from, Matrix4 to)
        {
            return Transpose(from) * to;
        }

        /// <summary>
        /// Create rotation matrix that rotates *from* direction with *to* direction
        /// </summary>
        public static Matrix4 Delta(Vector4 from, Vector4 to)
        {
            return Transpose(LookAt(from)) * (LookAt(to));
        }
        
        /// <summary>
        /// Get rotation matrix that rotates identity object to given overward axis
        /// </summary>
        public static Matrix4 LookAt(Vector4 overward)
        {
            Vector4 a = Vector4.Normalize(overward);

            // Fastest solution
            return new Matrix4(
                a.w, -a.z, a.y, -a.x,
                a.z, a.w, -a.x, -a.y,
                -a.y, a.x, a.w, -a.z,
                a.x, a.y, a.z, a.w
                );
        }

        /// <summary>
        /// Get rotation matrix that rotates identity object to given overward and forward axis
        /// </summary>
        public static Matrix4 LookAt(Vector4 overward, Vector4 forward)
        {
            Vector4 a = Vector4.Normalize(overward), b = Vector4.Normalize(forward), c;
            c = ((a.x < 0.5f && b.x < 0.5f) ? Vector4.Cross(a, b, Vector4.rightward) : Vector4.Cross(a, b, Vector4.upward));
            return LookAt(a, b, c);
        }

        /// <summary>
        /// Get rotation matrix that rotates identity object to given overward and forward and upward axis
        /// </summary>
        /// <remarks>
        /// Forward and upward is a support direction, meaning they'll change if they're not perpendicular with overward.
        /// </remarks>
        public static Matrix4 LookAt(Vector4 overward, Vector4 forward, Vector4 upward)
        {
            // At this time, `working code` is the priority. We'll research for efficient algo later.
            Vector4 a = Vector4.Normalize(overward), b = Vector4.Normalize(forward);
            Vector4 c = Vector4.Normalize(upward), d = Vector4.Normalize(Vector4.Cross(c, b, a));
            b = Vector4.Normalize(Vector4.Cross(a, d, c)); c = Vector4.Normalize(Vector4.Cross(a, b, d));
            return new Matrix4(d, c, b, a);
        }

        /// <summary>
        /// Get an identity matrix
        /// </summary>
        public static Matrix4 identity { get { return new Matrix4(Vector4.one); } }
        /// <summary>
        /// Get a zero matrix
        /// </summary>
        public static Matrix4 zero { get { return new Matrix4(); } }

        /// <summary> First column of the matrix </summary>
        public Vector4 Column0
        {
            get
            {
                return new Vector4(ex.x, ey.x, ez.x, ew.x);
            }
            set
            {
                ex.x = value.x;
                ey.x = value.y;
                ez.x = value.z;
                ew.x = value.w;
            }
        }

        /// <summary> Second column of the matrix </summary>
        public Vector4 Column1
        {
            get
            {
                return new Vector4(ex.y, ey.y, ez.y, ew.y);
            }
            set
            {
                ex.y = value.x;
                ey.y = value.y;
                ez.y = value.z;
                ew.y = value.w;
            }
        }

        /// <summary> Third column of the matrix </summary>
        public Vector4 Column2
        {
            get
            {
                return new Vector4(ex.z, ey.z, ez.z, ew.z);
            }
            set
            {
                ex.z = value.x;
                ey.z = value.y;
                ez.z = value.z;
                ew.z = value.w;
            }
        }

        /// <summary> Fourth column of the matrix </summary>
        public Vector4 Column3
        {
            get
            {
                return new Vector4(ex.w, ey.w, ez.w, ew.w);
            }
            set
            {
                ex.w = value.x;
                ey.w = value.y;
                ez.w = value.z;
                ew.w = value.w;
            }
        }

        /// <summary>
        /// Get Nth column by index
        /// </summary>
        /// <remarks> It's much better to use the getter property if the index is hard-coded </remarks>
        public Vector4 Column(int i)
        {
            return new Vector4(ex[i], ey[i], ez[i], ew[i]);
        }

        /// <summary>
        /// Set Nth column by index
        /// </summary>
        /// <remarks> It's much better to use the setter property if the index is hard-coded </remarks>
        public void Column(int i, Vector4 value)
        {
            ex[i] = value.x;
            ey[i] = value.y;
            ez[i] = value.z;
            ew[i] = value.w;
        }

        /// <summary>
        /// Access the diagonal row of the matrix
        /// </summary>
        public Vector4 Diagonal
        {
            get
            {
                return new Vector4(ex.x, ey.y, ez.z, ew.w);
            }
        }

        /// <summary>
        /// Implicit conversion to Unity's Matrix4x4
        /// </summary>
        public static implicit operator Matrix4x4(Matrix4 v)
        {
            var m = new Matrix4x4();
            m.SetRow(0, v.ex);
            m.SetRow(1, v.ey);
            m.SetRow(2, v.ez);
            m.SetRow(3, v.ew);
            return m;
        }

        /// <summary>
        /// Implicit conversion from Unity's Matrix4x4
        /// </summary>
        public static implicit operator Matrix4(Matrix4x4 v)
        {
            return new Matrix4()
            {
                ex = v.GetRow(0),
                ey = v.GetRow(1),
                ez = v.GetRow(2),
                ew = v.GetRow(3),
            };
        }

        /// <summary>
        /// Get the determinant of the matrix
        /// </summary>
        public static float Determinant(Matrix4 m)
        {
            return Vector4.Dot(m.ew, Vector4.Cross(m.ez, m.ey, m.ex));
        }

        /// <summary>
        /// Check the equality between two matrices
        /// </summary>
        public static bool operator ==(Matrix4 a, Matrix4 b)
        {
            return a.ex == b.ex && a.ey == b.ey && a.ez == b.ez && a.ew == b.ew;
        }

        /// <summary>
        /// Check the inequality between two matrices
        /// </summary>
        public static bool operator !=(Matrix4 a, Matrix4 b)
        {
            return a.ex != b.ex || a.ey != b.ey || a.ez != b.ez || a.ew != b.ew;
        }

        /// <summary> </summary>
        [HideInDocs]
        public override bool Equals(object obj)
        {
            return obj is Matrix4 && this == (Matrix4)obj;
        }

        /// <summary> Overloaded GetHashCode </summary>
        [HideInDocs]
        public override int GetHashCode()
        {
            return ex.GetHashCode() ^ ey.GetHashCode() << 1 ^ ez.GetHashCode() << 2 ^ ew.GetHashCode() << 3;
        }

        /// <summary> Convert to string for debugging ease. </summary>
        public override string ToString()
        {
            return string.Format("{0},\r\n{1},\r\n{2},\r\n{3}", ex, ey, ez, ew);
        }

        /// <summary> </summary>
        [HideInDocs]
        public bool Equals(Matrix4 other)
        {
            return this == other;
        }
    }



}
