using System;
using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Projection helper using Cross section technique.
    /// </summary>
    /// <remarks>
    /// Cross section projects model by cutting it in half. 
    /// [Manual](/manual/rendering/projections/crosssection.md).
    /// </remarks>
    public class CrossSection4 : Projector4
    {

        internal Matrix4x5 view;
        [NonSerialized]
        internal Matrix4x5 viewmodel;

        [NonSerialized]
        bool[] _sides = new bool[4];
        [NonSerialized]
        Vector4[] _temp = new Vector4[4];
        [NonSerialized]
        VertexProfile[] _temp2 = new VertexProfile[4];

        /// 
        [HideInDocs]
        public override bool IsCullable(SphereBounds4 bound)
        {
            // SphereBounds4.IsIntersecting(slicer, bound); <- Unoptimized
            return Math.Abs(bound.center.w + view.position.w) > bound.radius;
        }

        void CheckSidesCapacity(int cap)
        {
            if (_sides.Length < cap)
                _sides = new bool[Math.Max(cap, _sides.Length << 1)];
        }

        /// 
        [HideInDocs]
        public override void Project(Buffer4 source, Matrix4x5 transform, IVisualizer dest)
        {
            viewmodel = view * transform;
            var v4 = source.m_Vertices; var v4c = source.m_VerticesCount; CheckSidesCapacity(v4c);

            // Predetect vertex sides on plane
            for (int i = 0; i < v4c; i++)
                _sides[i] = (Utility.FastMultW(viewmodel, v4[i]) > 0);

            if (source.m_ProfilesCount == source.m_IndicesCount)
                switch (source.simplex)
                {
                    case SimplexMode.Line: InternalProjectProfile1(source, dest); break;
                    case SimplexMode.Triangle: InternalProjectProfile2(source, dest); break;
                    case SimplexMode.Tetrahedron: InternalProjectProfile3(source, dest); break;
                }
            else
                switch (source.simplex)
                {
                    case SimplexMode.Line: InternalProject1(source, dest); break;
                    case SimplexMode.Triangle: InternalProject2(source, dest); break;
                    case SimplexMode.Tetrahedron: InternalProject3(source, dest); break;
                }
        }


        static bool IsEqual(bool a, bool b, bool c)
        {
            return (a == b) && (c == b);
        }

        static bool IsEqual(bool a, bool b, bool c, bool d)
        {
            // Uhh. maybe we can make this faster again? 
            return (a == b) && (c == d) && (b == c);
        }

        // Internal separate projection methods
        void InternalProject1(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;
            // Loop over all point
            for (int i = 0; i < t4c; i += 2)
            {
                // Do initial check if this edge is really intersect
                if (s[t4[i]] == s[t4[i + 1]]) continue;

                // Intersect
                int a = t4[0 + i], b = t4[1 + i];
                t[0] = Utility.CrossInterpolate(viewmodel * v4[a], viewmodel * v4[b]);

                // Push to destination
                dest.Render(t, 1);
            }
        }

        void InternalProject2(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;
            // Loop over all edge
            for (int i = 0; i < t4c; i += 3)
            {
                // Do initial check if this triangle is really intersect
                if (IsEqual(s[t4[i]], s[t4[i + 1]], s[t4[i + 2]])) continue;

                // Intersect
                int iter = 0, a, b;
                for (int j = 0; j < 3; j++)
                {
                    if (s[a = t4[_leftEdges[j] + i]] ^ s[b = t4[_rightEdges[j] + i]])
                    {
                        t[iter++] = Utility.CrossInterpolate(viewmodel * v4[a], viewmodel * v4[b]);
                    }
                }

                // Push to destination
                dest.Render(t, iter);
            }
        }

        void InternalProject3(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;
            // Loop over all triangle
            for (int i = 0; i < t4c; i += 4)
            {
                // Do initial check if this trimid is really intersect
                if (IsEqual(s[t4[i]], s[t4[i + 1]], s[t4[i + 2]], s[t4[i + 3]])) continue;

                // Intersect
                int iter = 0, a, b;
                for (int j = 0; j < 6; j++)
                {
                    if (s[a = t4[_leftEdges[j] + i]] ^ s[b = t4[_rightEdges[j] + i]])
                    {
                        t[iter++] = Utility.CrossInterpolate(viewmodel * v4[a], viewmodel * v4[b]);
                    }
                }

                // Push to destination
                dest.Render(t, iter);
            }
        }


        // Internal separate projection methods with profiles

        void InternalProjectProfile1(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp2; float phase;
            // Loop over all edge
            for (int i = 0; i < t4c; i += 2)
            {
                // Do initial check if this edge is really intersect
                if (s[t4[i + 0]] == s[t4[i + 1]]) continue;

                // Intersect
                t[0] = Utility.CrossInterpolate(viewmodel * v4[t4[i]], viewmodel * v4[t4[1 + i]], out phase);
                p[0] = VertexProfile.LerpUnclamped(p4[i], p4[1 + i], phase);

                // Push to destination
                dest.Render(t, 1);
                dest.Render(p, 1);
            }
        }

        void InternalProjectProfile2(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp2; float phase;
            // Loop over all triangle
            for (int i = 0; i < t4c; i += 3)
            {
                // Do initial check if this triangle is really intersect
                if (IsEqual(s[t4[i + 0]], s[t4[i + 1]], s[t4[i + 2]])) continue;

                // Intersect
                int iter = 0, a, b, c, d;
                for (int j = 0; j < 3; j++)
                {
                    if (s[a = t4[c = _leftEdges[j] + i]] ^ s[b = t4[d = _rightEdges[j] + i]])
                    {
                        t[iter] = Utility.CrossInterpolate(viewmodel * v4[a], viewmodel * v4[b], out phase);
                        p[iter] = VertexProfile.LerpUnclamped(p4[c], p4[d], phase);
                        iter++;
                    }
                }

                // Push to destination
                dest.Render(t, iter);
                dest.Render(p, iter);
            }
        }

        void InternalProjectProfile3(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp2; float phase;

            // Loop over all trimids
            for (int i = 0; i < t4c; i += 4)
            {
                // Do initial check if this trimid is really intersect
                if (IsEqual(s[t4[i + 0]], s[t4[i + 1]], s[t4[i + 2]], s[t4[i + 3]])) continue;

                // Intersect
                int iter = 0, a, b, c, d;
                for (int j = 0; j < 6; j++)
                {
                    if (s[a = t4[c = _leftEdges[j] + i]] ^ s[b = t4[d = _rightEdges[j] + i]])
                    {
                        t[iter] = Utility.CrossInterpolate(viewmodel * v4[a], viewmodel * v4[b], out phase);
                        p[iter] = VertexProfile.LerpUnclamped(p4[c], p4[d], phase);
                        iter++;
                    }
                }

                // Push to destination
                dest.Render(t, iter);
                dest.Render(p, iter);
            }
        }

        //

        /// 
        [HideInDocs]
        public override void Setup(Matrix4x5 viewer)
        {
            view = Matrix4x5.Inverse(viewer);
        }

        /// 
        [HideInDocs]
        public override Vector3 Project(Vector4 v)
        {
            return view * v;
        }

        /// 
        [HideInDocs]
        public override Vector3 Project(Vector4 v, out bool culled)
        {
            culled = Math.Abs(v.w) > 1e-2f;
            return view * v;
        }

        /// <summary>
        /// This method always return the higher version from the simplex.
        /// </summary>
        public override SimplexMode SimplexModeForVisualizing(SimplexMode mode)
        {
            // Cross section needs a hinger dimension than the given simplex
            return (SimplexMode)((int)mode + 1);
        }

        // Left & Right edges of segment & triangles & trimids

        static readonly int[] _leftEdges = new int[] { 0, 0, 1, 2, 1, 0, };

        static readonly int[] _rightEdges = new int[] { 1, 2, 2, 3, 3, 3, };
    }
}
