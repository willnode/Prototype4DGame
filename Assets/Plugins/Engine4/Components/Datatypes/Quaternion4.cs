

//using System;
//using UnityEngine.Internal;
//using UnityEngine;


//namespace Engine4
//{
//    /// <summary>
//    /// Quaternion for Vector4 rotation.
//    /// Uses a pair of 3D Quaternion to represent 6 axis of 4D rotations.
//    /// </summary>
//    [Serializable]
//    public struct Quaternion4
//    {

//        public float x;
//        public float y;
//        public float z;
//        public float w;

//        public float t;
//        public float u;
//        public float v;
//        public float s;

//        public Quaternion4(Quaternion xyz, Quaternion tuv) : this()
//        {
//            this.xyz = xyz;
//            this.tuv = tuv;
//        }

//        public Quaternion4(float x, float y, float z, float w, float t, float u, float v, float s)
//        {
//            this.x = x;
//            this.y = y;
//            this.z = z;
//            this.w = w;
//            this.t = t;
//            this.u = u;
//            this.v = v;
//            this.s = s;
//        }


//        public float this[int i]
//        {
//            get
//            {
//                switch (i)
//                {
//                    case 0: return x;
//                    case 1: return y;
//                    case 2: return z;
//                    case 3: return t;
//                    case 4: return u;
//                    case 5: return v;
//                    case 6: return s;
//                    case 7: return w;
//                    default: throw null;
//                }
//            }
//            set
//            {
//                switch (i)
//                {
//                    case 0: x = value; break;
//                    case 1: y = value; break;
//                    case 2: z = value; break;
//                    case 3: t = value; break;
//                    case 4: u = value; break;
//                    case 5: v = value; break;
//                    case 6: s = value; break;
//                    case 7: w = value; break;
//                    default: throw null;
//                }
//            }
//        }

//#if ENGINE_UPDATER_FROM_2_X
//        public Quaternion xyz;
//        public Quaternion tuv;
//#else
//        public Quaternion xyz { get { return new Quaternion(x, y, z, w); } set { x = value.x; y = value.y; z = value.z; w = value.w; } }
//        public Quaternion tuv { get { return new Quaternion(t, u, v, s); } set { t = value.x; u = value.y; v = value.z; s = value.w; } }
//#endif
//        public static Quaternion4 identity
//        {
//            get
//            {
//                return new Quaternion4 { w = 1, s = 1 };
//            }
//        }

//        /// <summary>
//        /// Get Quaternion4 from euler degrees
//        /// </summary>
//        public static Quaternion4 Euler(Euler4 r)
//        {
//            return new Quaternion4 { xyz = Quaternion.Euler(r.x, r.y, r.z), tuv = Quaternion.Euler(r.t, r.u, r.v) };
//        }

//        /// <summary>
//        /// Get Quaternion4 from euler degrees
//        /// </summary>
//        public static Quaternion4 Euler(Vector3 xyz, Vector3 tuv)
//        {
//            return new Quaternion4 { xyz = Quaternion.Euler(xyz), tuv = Quaternion.Euler(tuv) };
//        }

//        /// <summary>
//        /// Get Quaternion4 from euler degrees
//        /// </summary>
//        public static Quaternion4 Euler(float x, float y, float z, float t, float u, float v)
//        {
//            return new Quaternion4 { xyz = Quaternion.Euler(x, y, z), tuv = Quaternion.Euler(t, u, v) };
//        }


//        /// <summary>
//        /// Get Euler representation from Quaternion4
//        /// </summary>
//        public Euler4 ToEuler()
//        {
//            var a = xyz.eulerAngles;
//            var b = tuv.eulerAngles;
//            return new Euler4(a, b);
//        }

//        public void Normalize()
//        {
//            var q = Normalize(this);
//            xyz = q.xyz;
//            tuv = q.tuv;
//        }

//        public Quaternion4 inversed
//        {
//            get { return Inverse(this); }
//        }

//        /// <summary>
//        /// Get Matrix representation from Quaternion4
//        /// </summary>
//        public Matrix4 ToMatrix()
//        {
//            Normalize();
//            return ToMatrix(xyz, tuv);
//        }

//        public Matrix4 ToMatrix(Vector4 scale)
//        {
//            Normalize();
//            return ToMatrix(xyz, tuv) * new Matrix4(scale);
//        }

//        /// <summary>
//        /// Get the matrix representation so it can be multiplied with vectors efficiently
//        /// </summary>
//        static Matrix4 ToMatrix(Quaternion a, Quaternion b)
//        {


//            float x2 = a.x + a.x, y2 = a.y + a.y, z2 = a.z + a.z;
//            float xx2 = a.x * x2, xy2 = a.x * y2, xz2 = a.x * z2;
//            float xw2 = a.w * x2, yy2 = a.y * y2, yz2 = a.y * z2;
//            float yw2 = a.w * y2, zz2 = a.z * z2, zw2 = a.w * z2;

//            float t2 = b.x + b.x, u2 = b.y + b.y, v2 = b.z + b.z;
//            float tt2 = b.x * t2, tu2 = b.x * u2, tv2 = b.x * v2;
//            float tw2 = b.w * t2, uu2 = b.y * u2, uv2 = b.y * v2;
//            float uw2 = b.w * u2, vv2 = b.z * v2, vw2 = b.w * v2, ww2 = b.w * b.w * 2;

//            // Regular 3D quaternion to matrix formula, if you wonder
//            float izzyy2 = 1 - zz2 - yy2, ixxzz2 = 1 - xx2 - zz2, ixxyy2 = 1 - xx2 - yy2;
//            float ixyzw2 = xy2 - zw2, iywxz2 = yw2 + xz2, izwxy2 = zw2 + xy2;
//            float iyzxw2 = yz2 - xw2, ixzyw2 = xz2 - yw2, ixwyz2 = xw2 + yz2;

//            return new Matrix4(
//                 izzyy2 * (1 - tt2) - ixyzw2 * tu2 - iywxz2 * tv2,
//                 ixyzw2 * (1 - uu2) - izzyy2 * tu2 - iywxz2 * uv2,
//                 iywxz2 * (1 - vv2) - izzyy2 * tv2 - ixyzw2 * uv2,
//                 izzyy2 * (0 - tw2) - ixyzw2 * uw2 - iywxz2 * vw2,
//                 izwxy2 * (1 - tt2) - ixxzz2 * tu2 - iyzxw2 * tv2,
//                 ixxzz2 * (1 - uu2) - izwxy2 * tu2 - iyzxw2 * uv2,
//                 iyzxw2 * (1 - vv2) - izwxy2 * tv2 - ixxzz2 * uv2,
//                 izwxy2 * (0 - tw2) - ixxzz2 * uw2 - iyzxw2 * vw2,
//                 ixzyw2 * (1 - tt2) - ixwyz2 * tu2 - ixxyy2 * tv2,
//                 ixwyz2 * (1 - uu2) - ixzyw2 * tu2 - ixxyy2 * uv2,
//                 ixxyy2 * (1 - vv2) - ixzyw2 * tv2 - ixwyz2 * uv2,
//                 ixzyw2 * (0 - tw2) - ixwyz2 * uw2 - ixxyy2 * vw2,
//                 tw2,
//                 uw2,
//                 vw2,
//                 ww2 - 1
//            );
//        }

//        public void Integrate(Euler4 angularVelocity, float dt)
//        {
            
//        }

//        static public Quaternion4 Normalize(Quaternion4 q)
//        {
//            var xyz = q.xyz;
//            var tuv = q.tuv;

//            var scale = (Quaternion.Dot(xyz, xyz));
//            if (scale < 1e-5f)
//                xyz = Quaternion.identity;
//            else if (scale < 0.999f || scale > 1.0001f)
//                xyz = Scale(xyz, 1 / Mathf.Sqrt(scale));

//            scale = (Quaternion.Dot(tuv, tuv));
//            if (scale < 1e-5f)
//                tuv = Quaternion.identity;
//            else if (scale < 0.999f || scale > 1.0001f)
//                tuv = Scale(tuv, 1 / Mathf.Sqrt(scale));

//            return new Quaternion4(xyz, tuv);
//        }

//        static Quaternion Scale(Quaternion q, float scale)
//        {
//            if (scale == 0 || float.IsInfinity(scale))
//                return q;

//            q.x *= scale;
//            q.y *= scale;
//            q.z *= scale;
//            q.w *= scale;
//            return q;
//        }

//        static public Quaternion4 Inverse(Quaternion4 q)
//        {
//            return new Quaternion4 { xyz = Quaternion.Inverse(q.xyz), tuv = Quaternion.Inverse(q.tuv) };
//        }

//        static public Quaternion4 Slerp(Quaternion4 a, Quaternion4 b, float t)
//        {
//            t = Mathf.Clamp01(t);
//            return new Quaternion4(Quaternion.SlerpUnclamped(a.xyz, b.xyz, t), Quaternion.SlerpUnclamped(a.tuv, b.tuv, t));
//        }

//        static public Quaternion4 Lerp(Quaternion4 a, Quaternion4 b, float t)
//        {
//            t = Mathf.Clamp01(t);
//            return new Quaternion4(Quaternion.LerpUnclamped(a.xyz, b.xyz, t), Quaternion.LerpUnclamped(a.tuv, b.tuv, t));
//        }

//        static public Quaternion4 LookRotation(Vector4 forward)
//        {
//            return LookRotation(Vector4.ComputeBasis(forward));
//        }

//        static public Quaternion4 LookRotation(Vector4 forward, Vector4 upward, Vector4 overward)
//        {
//            forward = Vector4.Normalize(forward);
//            upward = Vector4.Normalize(upward);
//            overward = Vector4.Normalize(overward);
//            var right = Vector4.Cross(forward, overward, upward);

//            var m = new Matrix4(right, upward, forward, overward);
//            return LookRotation(m);
//        }

//        static public Quaternion4 LookRotation(Matrix4 m)
//        {
//            // No one in the world know what's this
//            // This equation is directly related to Quaternion4.ToMatrix()
//            float s, t, u, v;

//            s = Mathf.Sqrt((m[3, 3] + 1) / 2f);
//            if (s > 0)
//            {
//                v = m[3, 2] / (s * 2);
//                u = m[3, 1] / (s * 2);
//                t = m[3, 0] / (s * 2);
//            }
//            else
//            {
//                t = 0;
//                u = 0;
//                v = 0;
//            }

//            var n = new Matrix4(
//                 (1 - t * t * 2),
//                 (-2 * t * u),
//                 (-2 * t * v),
//                 (-2 * s * t),
//                 (-2 * t * u),
//                 (1 - u * u * 2),
//                 (-2 * u * v),
//                 (-2 * s * u),
//                 (-2 * t * v),
//                 (-2 * u * v),
//                 (1 - v * v * 2),
//                 (-2 * s * v),
//                 (2 * s * t),
//                 (2 * s * u),
//                 (2 * s * v),
//                 (s * s * 2 - 1)
//            );

//            n = m * Matrix4.Transpose(n);

//            Vector3 l = n.Column2, r = n.Column1;

//            return new Quaternion4(Quaternion.LookRotation(l, r), new Quaternion(t, u, v, s));
//        }

//        public static Quaternion4 operator +(Quaternion4 a, Quaternion4 b)
//        {
//            return new Quaternion4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w, a.t + b.t, a.u + b.u, a.v + b.v, a.s + b.s);
//        }

//        public static Quaternion4 operator -(Quaternion4 a, Quaternion4 b)
//        {
//            return new Quaternion4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w, a.t - b.t, a.u - b.u, a.v - b.v, a.s - b.s);
//        }

//        public static Quaternion4 operator *(Quaternion4 a, Quaternion4 b)
//        {
//            //return LookRotation(a.ToMatrix() * b.ToMatrix());

//            return new Quaternion4(
//                    a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
//                    a.w * b.y + a.y * b.w - a.z * b.x + a.x * b.z,
//                    a.w * b.z + a.z * b.w - a.x * b.y + a.y * b.x,
//                    a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z,
//                    a.s * b.t + a.t * b.s,
//                    a.s * b.u + a.u * b.s,
//                    a.s * b.v + a.v * b.s,
//                    a.s * b.s - a.t * b.t - a.u * b.u - a.v * b.v)
//            + new Quaternion4(
//                 a.v * b.u - a.u * b.v,
//                 a.v * b.t - a.t * b.v,
//                 a.t * b.u - a.u * b.t,
//                 0,
//                 a.y * b.v - a.v * b.y + a.z * b.u - a.z * b.u,
//                 a.x * b.v - a.v * b.x + a.z * b.u - a.u * b.z,
//                 a.x * b.u - a.u * b.x + a.y * b.t - a.t * b.y,
//                 0  
//             );
//        }

//        public static Vector4 operator *(Quaternion4 a, Vector4 b)
//        {
//            return a.ToMatrix() * b;
//        }


//        public static implicit operator Quaternion4(Quaternion q)
//        {
//            return new Quaternion4(q, Quaternion.identity);
//        }

//        public static bool operator ==(Quaternion4 a, Quaternion4 b)
//        {
//            return a.xyz == b.xyz && a.tuv == b.tuv;
//        }

//        public static bool operator !=(Quaternion4 a, Quaternion4 b)
//        {
//            return a.xyz != b.xyz || a.tuv != b.tuv;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is Quaternion4))
//                return false;
//            return this == (Quaternion4)obj;
//        }

//        public override int GetHashCode()
//        {
//            return xyz.GetHashCode() ^ tuv.GetHashCode();
//        }

//        public override string ToString()
//        {
//            return "{" + xyz.ToString() + "} {" + tuv.ToString() + "}";
//        }

//    }


//}
