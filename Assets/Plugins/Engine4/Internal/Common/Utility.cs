using UnityEngine;
using System;
using System.Collections.Generic;
using Random = System.Random;

namespace Engine4.Internal
{
    /// <summary>
    /// Mathematical Utilities and Struct Extras
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Sets the w and return Vector4
        /// </summary>
        static public Vector4 SetW(this Vector3 xyz, float w)
        {
            return new Vector4(xyz.x, xyz.y, xyz.z, w);
        }

        /// <summary>
        /// Clear and push a new item to the stack
        /// </summary>
        public static void Clear<T>(this Stack<T> stack, T item)
        {
            stack.Clear();
            stack.Push(item);
        }

        /// <summary>
        /// Expand list capacity to given capacity target
        /// </summary>
        static public void Expand<T>(this List<T> list, int capacity)
        {
            if (list.Capacity < capacity)
                list.Capacity = capacity;
        }

        /// <summary>
        /// Expand list capacity to what another list has
        /// </summary>
        static public void Expand<T, T2>(this List<T> list, List<T2> capacity)
        {
            if (list.Capacity < capacity.Capacity)
                list.Capacity = capacity.Capacity;
        }


        /// <summary>
        /// Get Hue variation
        /// </summary>
        public static Color Hue(float h) { return Color.HSVToRGB(h, 1, 1); }

        /// <summary>
        /// Get Hue variation in gradient
        /// </summary>
        public static Gradient Hue()
        {
            var grad = new Gradient();
            grad.colorKeys = (new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.green, 0.333f), new GradientColorKey(Color.blue, 0.666f), new GradientColorKey(Color.red, 1) });
            return grad;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        public static Vector3 GetAverage(this List<Vector3> v)
        {
            var median = new Vector3();
            for (int i = v.Count; i-- > 0;) median += v[i];
            return median / v.Count;
        }

        /// <summary>
        /// Get bounding min-max
        /// </summary>
        public static Bounds4 GetBounding(this Vector4[] v, int count)
        {
            var bound = count == 0 ? Bounds4.zero : Bounds4.infinite;
            for (int i = 0; i < count;) bound.Allocate(v[i++]);
            return bound;
        }

        /// <summary>
        /// Quad into Two triangles
        /// </summary>
        public static readonly int[] QuadToTris = new int[] { 0, 1, 2, 1, 2, 3 };

        /// <summary>
        /// Swap two object
        /// </summary>
        public static void Swap<T>(ref T l, ref T r)
        {
            var tmp = l;
            l = r;
            r = tmp;
        }

        /// <summary>
        /// Sign function (x => x / Abs(x))
        /// </summary>
        /// <remarks>
        /// This function will never return zero compared with .NET Math
        /// </remarks>
        public static float Sign(float v) { return v >= 0 ? 1f : -1f; }

        /// <summary>
        /// Inverse function (x => 1 / x)
        /// </summary>
        public static float Invert(float i) { return 1 / i; }

        /// <summary>
        /// Clamp T between A and B
        /// </summary>
        public static float Clamp(float a, float b, float t) { return t < a ? a : (t > b ? b : t); }

        /// <summary>
        /// Clamp T between 0 and 1
        /// </summary>
        public static float Clamp01(float t) { return t < 0 ? 0 : (t > 1 ? 1 : t); }

        /// <summary>
        /// Remove unnecessary trailing digits for debugging ease.
        /// </summary>
        public static float Tidy(float v)
        {
            return (float)Math.Round(v, 2);
        }

        /// <summary>
        /// Remove unnecessary trailing digits for debugging ease.
        /// </summary>
        public static Vector4 Tidy(Vector4 v)
        {
            return new Vector4(Tidy(v.x), Tidy(v.y), Tidy(v.z), Tidy(v.w));
        }

        /// <summary>
        /// Remove unnecessary trailing digits for debugging ease.
        /// </summary>
        public static Matrix4 Tidy(Matrix4 v)
        {
            return new Matrix4(Tidy(v.ex), Tidy(v.ey), Tidy(v.ez), Tidy(v.ew));
        }

        /// <summary>
        /// Remove unnecessary trailing digits for debugging ease.
        /// </summary>
        public static Euler4 Tidy(Euler4 v)
        {
            return new Euler4(Tidy(v.x), Tidy(v.y), Tidy(v.z), Tidy(v.t), Tidy(v.u), Tidy(v.v));
        }

        /// <summary>
        /// Quickly remove an item if list order doesn't matter.
        /// </summary>
        public static void RemoveFast<T>(this List<T> list, T item)
        {
            list[list.IndexOf(item)] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        static Random _randomer = new Random();

        /// <summary>
        /// Next random integer
        /// </summary>
        public static int Random()
        {
            return _randomer.Next();
        }

        /// <summary>
        /// Give sign to any nonzero vector elements
        /// </summary>
        public static Vector4 GiveSign(Vector4 v, int x, int y)
        {
            int loop = 0;
            for (int i = 0; i < 4; i++)
            {
                if (v[i] != 0)
                    v[i] *= loop++ == 0 ? x : y;
            }
            return v;
        }

        /// <summary>
        /// Give sign to any nonzero vector elements
        /// </summary>
        public static Vector4 GiveSign(Vector4 v, int x, int y, int z)
        {
            int loop = 0;
            for (int i = 0; i < 4; i++)
            {
                if (v[i] != 0)
                    v[i] *= ++loop == 1 ? x : (loop == 2 ? y : z);
            }
            return v;
        }

        /// <summary>
        /// Give sign to any nonzero vector elements
        /// </summary>
        public static Vector4 GiveSign(Vector4 v, int x, int y, int z, int w)
        {
            return v * new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Get a unique hash from the vector
        /// </summary>
        public static int VectorToKey(Vector3 v)
        {
            // precision 0.05
            return (int)(v.x * 20) ^ (int)(v.y * 20) << 2 ^ (int)(v.z * 20) << 4;
        }

        /// <summary>
        /// Get a unique hash from the vector
        /// </summary>
        public static int VectorToKey(Vector4 v)
        {
            // precision 0.05
            return (int)(v.x * 20) ^ (int)(v.y * 20) << 2 ^ (int)(v.z * 20) << 4 ^ (int)(v.w * 20) << 6;
        }

        /// <summary>
        /// Atan2 with better handling of near 0, 0 singularity
        /// </summary>
        public static float Atan2(float y, float x)
        {
            if (y * y + x * x < 1e-2f)
                return 0;
            else
                return (float)Math.Atan2(y, x);
        }

        /// <summary>
        /// Atan2 with clear meaning that we don't want pi simply to avoid headache
        /// </summary>
        public static float Atan2AvoidPi(float y, float x)
        {
            //if (y * y + x * x < 1e-2f)
            //    return (float)Math.Atan2(Math.Abs(y), Math.Abs(x));
            //if ((y * y < 1e-2f))
            //    return (float)Math.Atan2(y, Math.Abs(x));
            //else if ((x * x < 1e-2f))
            //    return (float)Math.Atan2(Math.Abs(y), (x));
            //else
            return (float)Math.Atan2(y, x);
        }

        /// <summary>
        /// Convert value range -pi to pi into 0 to 2pi
        /// </summary>
        /// <returns></returns>
        public static float AngleLoop(float v)
        {
            return v < 0 ? v + Mathf.PI * 2F : v;
        }

        /// <summary>
        /// Get a unique hash from the vector
        /// </summary>
        public static V GetValue<K, V>(this Dictionary<K, V> dict, K key, V fallback)
        {
            V val;
            if (!dict.TryGetValue(key, out val)) val = fallback;
            return val;
        }

        /// <summary>
        /// List of tesseract vertices permutations
        /// </summary>
        public static readonly Vector4[] BoxVertices = new Vector4[16]{
            new Vector4( -1, -1, -1, -1 ),
            new Vector4( -1, -1,  1, -1 ),
            new Vector4( -1,  1, -1, -1 ),
            new Vector4( -1,  1,  1, -1 ),
            new Vector4(  1, -1, -1, -1 ),
            new Vector4(  1, -1,  1, -1 ),
            new Vector4(  1,  1, -1, -1 ),
            new Vector4(  1,  1,  1, -1 ),
            new Vector4( -1, -1, -1, 1 ),
            new Vector4( -1, -1,  1, 1 ),
            new Vector4( -1,  1, -1, 1 ),
            new Vector4( -1,  1,  1, 1 ),
            new Vector4(  1, -1, -1, 1 ),
            new Vector4(  1, -1,  1, 1 ),
            new Vector4(  1,  1, -1, 1 ),
            new Vector4(  1,  1,  1, 1 ),
            };

        /// <summary>
        /// Internally do Plane4.Intersect(), then Vector4.Lerp()
        /// </summary>
        public static Vector4 CrossInterpolate(Vector4 x, Vector4 y)
        {
            return /*x.w == y.w ? x :*/ x + (y - x) * (x.w / (x.w - y.w));
        }

        /// <summary>
        /// Internally do Plane4.Intersect(), then Vector4.Lerp(), and make the phase out
        /// </summary>
        public static Vector4 CrossInterpolate(Vector4 x, Vector4 y, out float phase)
        {
            /*if (x.w == y.w)
            {
                phase = 0;
                return x;
            }
            else*/
            return x + (y - x) * (phase = (x.w / (x.w - y.w)));
        }

        /// <summary>
        /// Perform multiplication, but only return the W
        /// </summary>
        public static float FastMultW(Matrix4x5 m, Vector4 v)
        {
            return Vector4.Dot(m.rotation.ew, v) + m.position.w;
        }

    }
}