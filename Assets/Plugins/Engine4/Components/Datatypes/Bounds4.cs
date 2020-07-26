using Math = UnityEngine.Mathf;
using System;
using Engine4.Internal;

namespace Engine4
{

    /// <summary>
    /// Representation for bounds in 4D.
    /// </summary>
    [Serializable]
    public struct Bounds4
    {
        /// <summary> Minimum position of the bound </summary>
        public Vector4 min;
        /// <summary> Maximum position of the bound </summary>
        public Vector4 max;

        /// <summary> Center position of the bound </summary>
        public Vector4 center {
            get {
                return new Vector4((max.x + min.x) * 0.5f, (max.y + min.y) * 0.5f, (max.z + min.z) * 0.5f, (max.w + min.w) * 0.5f);
            }
        }

        /// <summary> Extent (center to corner distance) of the bound </summary>
        public Vector4 extent {
            get {
                return new Vector4((max.x - min.x) * 0.5f, (max.y - min.y) * 0.5f, (max.z - min.z) * 0.5f, (max.w - min.w) * 0.5f);
            }
        }

        /// <summary> Size (edge to edge distance) of the bound </summary>
        public Vector4 size {
            get {
                return new Vector4(max.x - min.x, max.y - min.y, max.z - min.z, max.w - min.w);
            }
        }

        /// <summary> Returns a bound with negative infinity size</summary>
        public static Bounds4 infinite {
            get {
                return new Bounds4(new Vector4(float.NegativeInfinity));
            }
        }

        /// <summary> Returns a bound with zero size and zero center </summary>
        public static Bounds4 zero {
            get {
                return new Bounds4();
            }
        }

        /// <summary> Expand the bound to given point </summary>
        public void Allocate (Vector4 pos) {
            float x = pos.x, y = pos.y, z = pos.z, w = pos.w;

            min = new Vector4 {
                x = min.x < x ? min.x : x,
                y = min.y < y ? min.y : y,
                z = min.z < z ? min.z : z,
                w = min.w < w ? min.w : w
            };

            max = new Vector4 {
                x = max.x > x ? max.x : x,
                y = max.y > y ? max.y : y,
                z = max.z > z ? max.z : z,
                w = max.w > w ? max.w : w
            };
        }

        /// <summary> Create a bound with zero center and given extent </summary>
        public Bounds4(Vector4 extent)
        {
            min = -extent;
            max = extent;
        }

        /// <summary> Create a bound with given min and max position </summary>
        public Bounds4(Vector4 Min, Vector4 Max)
        {
            min = Min;
            max = Max;
        }

        /// <summary> Determine if and only if A partially contains B </summary>
        public static bool IsIntersecting(Bounds4 a, Bounds4 b)
        {
            return b.max > a.min && b.min < a.max;
        }
        
        /// <summary>
        /// Is the plane hits part of the bound?
        /// </summary>
        public bool IsIntersecting(Plane4 plane)
        {          
            var r = Vector4.Dot(extent, Vector4.Abs(plane.normal));
            var s = Vector4.Dot(center, plane.normal) - plane.distance;
            return Math.Abs(s) <= r;
        }

        /// <summary> Is this bound contains the whole cell of the other bound? </summary>
        public bool Contains(Bounds4 other)
        {
            return min < other.min && max > other.max;
        }

        /// <summary> Is this bound contains the given point </summary>
        public bool Contains(Vector4 point)
        {
            return min < point && max > point;
        }

        /// <summary>
        /// Interpolate two vector by T.
        /// </summary>
        /// <remarks> The interpolation is not clamped </remarks>
        public Vector4 Interpolate(Vector4 t)
        {
            return min + size * t;
        }

        /// <summary> Returns a point that either inside or touching the bound </summary>
        public Vector4 Clamp(Vector4 point)
        {
            Vector4 c = center, e = extent;
            point = point - c;

            for (int i = 0; i < 4; i++)
               point[i] = Utility.Clamp(-e[i], e[i], point[i]);

            return point + c;
        }

        /// <summary> Returns a point that either outside or touching the bound </summary>
        public Vector4 Clip(Vector4 t)
        {
            if (!Contains(t)) return t;

            Vector4 c = center, e = extent;
            t = t - c;

            var d = e - Vector4.Abs(t);
            int a = Vector4.MinPerElemIdx(d);
 
            t[a] = Utility.Clamp(e[a], -e[a], t[a]);

            return t + c;
        }


        /// <summary> Combine two bounds </summary>
        public static Bounds4 Combine(Bounds4 a, Bounds4 b)
        {
            return new Bounds4(Vector4.Min(a.min, b.min), Vector4.Max(a.max, b.max));
        }

        /// <summary> Intersect two bounds </summary>
        public static Bounds4 Intersect(Bounds4 a, Bounds4 b)
        {
            return new Bounds4(Vector4.Max(a.min, b.min), Vector4.Min(a.max, b.max));
        }

        /// <summary> Scales the bound's extent </summary>
        public static Bounds4 Scale(Bounds4 b, Vector4 s)
        {
            Vector4 c = b.center, e = b.extent * s;
            return new Bounds4(c - e, c + e);
        }

        /// <summary> Compare which Bounds4 is the closest. Positive means L is closer </summary>
        public static float Compare(Vector4 center, Bounds4 l, Bounds4 r)
        {
            return Vector4.DistanceSq(r.Clamp(center), center) - Vector4.DistanceSq(l.Clamp(center), center);
        }

        /// <summary> Check if given ray is colliding with the bound </summary>
        public bool Raycast(Vector4 p, Vector4 d)
        {

            var dI = Vector4.Invert(d);
            var t1 = (min - p) * dI;
            var t2 = (max - p) * dI;

            var tmin = Vector4.MaxPerElem(Vector4.Min(t1, t2));
            var tmax = Vector4.MinPerElem(Vector4.Max(t1, t2));

            return tmax >= tmin;
        }
    }


}