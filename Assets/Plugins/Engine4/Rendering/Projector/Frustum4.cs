using System;
using UnityEngine;
//using Math = UnityEngine.Mathf;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Projection helper using Frustum technique.
    /// </summary>
    /// <remarks>
    /// Frustum behaves the same way as conventional game engine does.
    /// [Manual](/manual/rendering/projections/frustum.md).
    /// </remarks>
    public class Frustum4 : Projector4
    {

        internal Matrix4x5 view;
        internal Matrix4x5 viewmodel;
        internal Plane4 nearPlane;
        internal Plane4 farPlane;

        internal Plane4 xMaxPlane;
        internal Plane4 xMinPlane;
        internal Plane4 yMaxPlane;
        internal Plane4 yMinPlane;
        internal Plane4 zMaxPlane;
        internal Plane4 zMinPlane;

        internal float ratio;

        [NonSerialized]
        bool[] _sides = new bool[4];
        [NonSerialized]
        Vector4[] _temp = new Vector4[9];
        [NonSerialized]
        Vector4[] _temp2 = new Vector4[9];
        [NonSerialized]
        VertexProfile[] _temp3 = new VertexProfile[9];

        /// <summary>
        /// Transition between orthographic (0) and perspective (1)
        /// </summary>
        [Range(0, 1)]
        public float perspectiveness = 1;
        /// <summary>
        /// Focal length of the camera
        /// </summary>
        public float focalLength = 1;
        /// <summary>
        /// Minimum projection distance limit 
        /// </summary>
        public float nearClip = 0.1f;
        /// <summary>
        /// Maximum projection distance limit 
        /// </summary>
        public float farClip = 1000f;
        /// <summary>
        /// Backward shift of the camera
        /// </summary>
        /// <remarks>
        /// It's equivalent to move camera in negative W direction
        /// </remarks>
        public float shiftClip = 0f;
        /// <summary>
        /// Use additional frustum culling?
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool useFrustumCulling = true;

        /// <summary>
        /// Access the calculated field of view angle in degree
        /// </summary>
        public float fieldOfView
        {
            get { return Mathf.Atan2(1f, focalLength) * Mathf.Rad2Deg; }
            set { focalLength = 1 / Mathf.Tan(Mathf.Clamp(value, 0.01f, 179.9f) * Mathf.Deg2Rad); }
        }

        /// 
        [HideInDocs]
        public override void Setup(Matrix4x5 viewer)
        {
            viewer.position -= viewer.rotation * new Vector4(3, shiftClip);
            view = Matrix4x5.Inverse(viewer);
            ratio = 1 / focalLength;
            nearPlane = new Plane4() { normal = new Vector4(3, 1), distance = nearClip };
            farPlane = new Plane4() { normal = new Vector4(3, -1), distance = farClip };

            if (useFrustumCulling)
            {
                var normalizer = 1 / Mathf.Sqrt(ratio * ratio + 1); // To keep the length = 1
                xMinPlane = new Plane4() { normal = (new Vector4(3, -ratio) - new Vector4(0, 1)) * normalizer };
                xMaxPlane = new Plane4() { normal = (new Vector4(3, -ratio) + new Vector4(0, 1)) * normalizer };
                yMinPlane = new Plane4() { normal = (new Vector4(3, -ratio) - new Vector4(1, 1)) * normalizer };
                yMaxPlane = new Plane4() { normal = (new Vector4(3, -ratio) + new Vector4(1, 1)) * normalizer };
                zMinPlane = new Plane4() { normal = (new Vector4(3, -ratio) - new Vector4(2, 1)) * normalizer };
                zMaxPlane = new Plane4() { normal = (new Vector4(3, -ratio) + new Vector4(2, 1)) * normalizer };
            }
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
                _sides[i] = GetSideTest(viewmodel * v4[i]);

            if (source.m_ProfilesCount == source.m_IndicesCount)
                switch (source.simplex)
                {
                    case SimplexMode.Point: InternalProjectProfile1(source, dest); break;
                    case SimplexMode.Line: InternalProjectProfile2(source, dest); break;
                    case SimplexMode.Triangle: InternalProjectProfile3(source, dest); break;
                }
            else
                switch (source.simplex)
                {
                    case SimplexMode.Point: InternalProject1(source, dest); break;
                    case SimplexMode.Line: InternalProject2(source, dest); break;
                    case SimplexMode.Triangle: InternalProject3(source, dest); break;
                }
        }

        Vector4 ProjectPoint(Vector4 v)
        {
            // if (perspectiveness < 1e-4f) return v;
            return v * Mathf.Lerp(1, ratio / v.w, perspectiveness);
        }

        // The optimized frustum <-> segment intersection.
        Vector4 Clip(Vector4 vertex, Vector4 other)
        {
            // Get intersection between vertex (which lies outside) and other (lies inside)

            float rP = ratio * vertex.w, rN = -rP, interpol = 0;

            if (vertex.w < nearClip) interpol = Math.Max(interpol, nearPlane.Intersect(vertex, other));
            else if (vertex.w > farClip) interpol = Math.Max(interpol, farPlane.Intersect(vertex, other));

            if (useFrustumCulling)
            {
                if (vertex.x < rN) interpol = Math.Max(interpol, xMinPlane.Intersect(vertex, other));
                else if (vertex.x > rP) interpol = Math.Max(interpol, xMaxPlane.Intersect(vertex, other));

                if (vertex.y < rN) interpol = Math.Max(interpol, yMinPlane.Intersect(vertex, other));
                else if (vertex.y > rP) interpol = Math.Max(interpol, yMaxPlane.Intersect(vertex, other));

                if (vertex.z < rN) interpol = Math.Max(interpol, zMinPlane.Intersect(vertex, other));
                else if (vertex.z > rP) interpol = Math.Max(interpol, farPlane.Intersect(vertex, other));
            }

            return Vector4.LerpUnclamped(vertex, other, interpol);
        }

        // The optimized frustum <-> triangle intersection.
        //int ClipTriangle()
        //{
        //    // Get intersection between vertex (which lies outside) and other (lies inside)

        //}

        int OrthographicXYZ(int axis, Vector4[] input, int length, Vector4[] result)
        {
            int count = 0;

            for (int i = 0; i < length; ++i)
            {
                Vector4 a = input[i], b = input[(i + 1) % length];

                float da = Math.Abs(a[axis]) - ratio * a.w, db = Math.Abs(b[axis]) - ratio * b.w;

                if (da < 0 && db < 0)
                    result[count++] = b;
                else if (da < 0 ^ db < 0)
                {
                    // Intersection point between A and B
                    var cv = a + (b - a) * (da / (da - db));

                    if (da < 0)
                        result[count++] = cv;
                    else
                    {
                        result[count++] = cv;
                        result[count++] = b;
                    }
                }
            }
            return count;
        }

        int OrthographicW(Vector4[] input, int length, Vector4[] result)
        {
            int count = 0;

            for (int i = 0; i < length; ++i)
            {
                Vector4 a = input[i], b = input[(i + 1) % length];

                float da = -(a.w) + nearClip, db = -(b.w) + nearClip;

                if (da < 0 && db < 0)
                    result[count++] = b;
                else if (da < 0 ^ db < 0)
                {
                    // Intersection point between A and B
                    var cv = a + (b - a) * (da / (da - db));

                    if (da < 0)
                        result[count++] = cv;
                    else
                    {
                        result[count++] = cv;
                        result[count++] = b;
                    }
                }
            }

            int count2 = 0;

            for (int i = 0; i < count; ++i)
            {
                Vector4 a = result[i], b = result[(i + 1) % count];

                float da = (a.w) - farClip, db = (b.w) - farClip;

                if (da < 0 && db < 0)
                    input[count2++] = b;
                else if (da < 0 ^ db < 0)
                {
                    // Intersection point between A and B
                    var cv = a + (b - a) * (da / (da - db));

                    if (da < 0)
                        input[count2++] = cv;
                    else
                    {
                        input[count2++] = cv;
                        input[count2++] = b;
                    }
                }
            }

            return count2;
        }

        // The optimized frustum <-> segment intersection.
        Vector4 Clip(Vector4 vertex, Vector4 other, out float interpol)
        {
            // Get intersection between vertex (which lies outside) and other (lies inside)

            var flag = GetSide(vertex); interpol = 0;

            if (useFrustumCulling)
            {
                if ((flag & 0x1) > 0) interpol = Math.Max(interpol, xMinPlane.Intersect(vertex, other));
                else if ((flag & 0x2) > 0) interpol = Math.Max(interpol, xMaxPlane.Intersect(vertex, other));

                if ((flag & 0x4) > 0) interpol = Math.Max(interpol, yMinPlane.Intersect(vertex, other));
                else if ((flag & 0x8) > 0) interpol = Math.Max(interpol, yMaxPlane.Intersect(vertex, other));

                if ((flag & 0x10) > 0) interpol = Math.Max(interpol, zMinPlane.Intersect(vertex, other));
                else if ((flag & 0x20) > 0) interpol = Math.Max(interpol, zMaxPlane.Intersect(vertex, other));
            }

            if ((flag & 0x40) > 0) interpol = Math.Max(interpol, nearPlane.Intersect(vertex, other));
            else if ((flag & 0x80) > 0) interpol = Math.Max(interpol, farPlane.Intersect(vertex, other));

            return Vector4.LerpUnclamped(vertex, other, interpol);
        }


        // 0x1=x- 0x2=x+ 0x4=y- 0x8=y+ 0x10=z- 0x20=z+
        // 0x40=w- 0x80=w+ 0 = inside. Can be Multiflagged
        int GetSide(Vector4 v)
        {
            int flag = 0; float rP = ratio * v.w, rN = -rP;

            if (v.w < nearClip) flag |= 0x40;
            else if (v.w > farClip) flag |= 0x80;

            if (useFrustumCulling)
            {
                if (v.x < rN) flag |= 0x1;
                else if (v.x > rP) flag |= 0x2;

                if (v.y < rN) flag |= 0x4;
                else if (v.y > rP) flag |= 0x8;

                if (v.z < rN) flag |= 0x10;
                else if (v.z > rP) flag |= 0x20;
            }

            return flag;
        }

        bool GetSideTest(Vector4 v)
        {
            if (v.w < nearClip || v.w > farClip)
                return false;

            if (useFrustumCulling)
            {
                var ratio = this.ratio * v.w;
                if (Math.Abs(v.x) > ratio) return false;
                if (Math.Abs(v.y) > ratio) return false;
                if (Math.Abs(v.z) > ratio) return false;
            }

            return true; // Definitely inside
        }


        // Internal separate projection methods

        void InternalProject1(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;
            // Loop over all point
            for (int i = 0; i < t4c; i += 1)
            {
                // Get the case
                switch (GetCase(s[t4[i + 0]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[t4[i]]);
                        dest.Render(t, 1);
                        break;
                }
            }
        }

        void InternalProject2(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;
            // Loop over all edge
            for (int i = 0; i < t4c; i += 2)
            {
                // Get the case
                int a, b;
                switch (GetCase(s[a = t4[i + 0]], s[b = t4[i + 1]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[a]);
                        t[1] = ProjectPoint(viewmodel * v4[b]);
                        dest.Render(t, 2);
                        break;
                    case FrustumCase.Intersect:
                        if (s[a])
                        {
                            t[0] = ProjectPoint(viewmodel * v4[a]);
                            t[1] = ProjectPoint(Clip(viewmodel * v4[b], viewmodel * v4[a]));
                        }
                        else
                        {
                            t[0] = ProjectPoint(Clip(viewmodel * v4[a], viewmodel * v4[b]));
                            t[1] = ProjectPoint(viewmodel * v4[b]);
                        }
                        dest.Render(t, 2);
                        break;
                }
            }
        }

        void InternalProject3(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp;

            // Loop over all triangle
            for (int i = 0; i < t4c; i += 3)
            {
                // Get the case
                int a, b, c;
                switch (GetCase(s[a = t4[i + 0]], s[b = t4[i + 1]], s[c = t4[i + 2]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[a]);
                        t[1] = ProjectPoint(viewmodel * v4[b]);
                        t[2] = ProjectPoint(viewmodel * v4[c]);
                        dest.Render(t, 3);
                        break;
                    case FrustumCase.Intersect:
                        t[0] = (viewmodel * v4[a]);
                        t[1] = (viewmodel * v4[b]);
                        t[2] = (viewmodel * v4[c]);

                        // Special algorithm
                        int count;
                        count = OrthographicW(_temp, 3, _temp2);
                        if (useFrustumCulling)
                        {
                            count = OrthographicXYZ(0, _temp, count, _temp2);
                            count = OrthographicXYZ(1, _temp2, count, _temp);
                            count = OrthographicXYZ(2, _temp, count, _temp2);
                        }

                        for (int j = 0; j < _temp2.Length; j++) _temp2[j] = ProjectPoint(_temp2[j]);
                        dest.Render(_temp2, count);
                        break;
                }
            }
        }


        // Internal separate projection methods with profiles

        void InternalProjectProfile1(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp3;
            // Loop over all point
            for (int i = 0; i < t4c; i += 1)
            {
                // Get the case
                switch (GetCase(s[t4[i + 0]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[t4[i]]);
                        p[0] = p4[t4[i]];

                        dest.Render(t, 1);
                        dest.Render(p, 1);
                        break;
                }
            }
        }

        void InternalProjectProfile2(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp3;
            // Loop over all edge
            for (int i = 0; i < t4c; i += 2)
            {
                // Get the case
                int a, b;
                switch (GetCase(s[a = t4[i + 0]], s[b = t4[i + 1]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[a]);
                        t[1] = ProjectPoint(viewmodel * v4[b]);
                        p[0] = p4[a];
                        p[1] = p4[b];
                        break;
                    case FrustumCase.Intersect:
                        float phase;
                        if (s[a])
                        {
                            t[0] = ProjectPoint(viewmodel * v4[a]);
                            t[1] = ProjectPoint(Clip(viewmodel * v4[b], viewmodel * v4[a], out phase));
                            p[0] = p4[a];
                            p[1] = VertexProfile.LerpUnclamped(p4[b], p4[a], phase);
                        }
                        else
                        {
                            t[0] = ProjectPoint(Clip(viewmodel * v4[a], viewmodel * v4[b], out phase));
                            t[1] = ProjectPoint(viewmodel * v4[b]);
                            p[0] = VertexProfile.LerpUnclamped(p4[a], p4[b], phase);
                            p[1] = p4[a];
                        }
                        break;
                }
                dest.Render(t, 2);
                dest.Render(p, 2);
            }
        }

        void InternalProjectProfile3(Buffer4 source, IVisualizer dest)
        {
            int[] t4 = source.m_Indices; Vector4[] v4 = source.m_Vertices; VertexProfile[] p4 = source.m_Profiles;
            var t4c = source.m_IndicesCount; var s = _sides; var t = _temp; var p = _temp3;

            // Loop over all trimids
            for (int i = 0; i < t4c; i += 3)
            {
                // Get the case
                int a, b, c;
                switch (GetCase(s[a = t4[i + 0]], s[b = t4[i + 1]], s[c = t4[i + 2]]))
                {
                    case FrustumCase.Inside:
                        // Add 'em all & Push
                        t[0] = ProjectPoint(viewmodel * v4[a]);
                        t[1] = ProjectPoint(viewmodel * v4[b]);
                        t[2] = ProjectPoint(viewmodel * v4[c]);
                        p[0] = p4[i + 0];
                        p[1] = p4[i + 1];
                        p[2] = p4[i + 2];
                        dest.Render(t, 3);
                        dest.Render(p, 3);
                        break;
                    case FrustumCase.Intersect:
                        int iter = 0;
                        float phase;
                        // Loop over edges
                        for (int j = 0; j < 3; j++)
                        {
                            if (s[a = t4[j + i]] & s[b = t4[(j + 1) % 3 + i]])
                            {
                                t[iter] = ProjectPoint(viewmodel * v4[b]);
                                p[iter] = p4[b];
                                iter++;
                            }
                            else if (s[b])
                            {
                                t[iter] = ProjectPoint(viewmodel * v4[b]);
                                p[iter] = p4[b];
                                iter++;
                                t[iter] = ProjectPoint(Clip(viewmodel * v4[a], viewmodel * v4[b], out phase));
                                p[iter] = VertexProfile.LerpUnclamped(p4[a], p4[b], phase);
                                iter++;
                            }
                            else if (s[a])
                            {
                                t[iter] = ProjectPoint(Clip(viewmodel * v4[b], viewmodel * v4[a], out phase));
                                p[iter] = VertexProfile.LerpUnclamped(p4[b], p4[a], phase);
                                iter++;
                            }
                        }

                        dest.Render(t, iter);
                        dest.Render(p, iter);
                        break;
                }
            }
        }




        static FrustumCase GetCase(bool a)
        {
            return a ? FrustumCase.Inside : FrustumCase.Outside;
        }

        static FrustumCase GetCase(bool a, bool b)
        {
            return (a == b) ? (a ? FrustumCase.Inside : FrustumCase.Outside) : FrustumCase.Intersect;
        }

        static FrustumCase GetCase(bool a, bool b, bool c)
        {
            return (a == b) & (b == c) ? (a ? FrustumCase.Inside : FrustumCase.Outside) : FrustumCase.Intersect;
        }

        /// 
        [HideInDocs]
        public override bool IsCullable(SphereBounds4 bound)
        {
            return !IsSphereInFrustum(bound);
        }

        // Keep culling fast by assuming the bound is a sphere
        bool IsSphereInFrustum(SphereBounds4 bound)
        {
            var radius = bound.radius;
            if (useFrustumCulling)
            {
                var xyz = view * bound.center;
                var ratio = this.ratio * xyz.w;

                if (xyz.w + radius < nearClip || xyz.w - radius > farClip) return false;
                if (xyz.x + radius < -ratio || xyz.x - radius > ratio) return false;
                if (xyz.y + radius < -ratio || xyz.y - radius > ratio) return false;
                if (xyz.z + radius < -ratio || xyz.z - radius > ratio) return false;
                return true;
            }
            else
            {
                float n = Vector4.Dot(view.overward, bound.center + view.position);
                return (n + radius >= nearClip && n - radius < farClip);
            }
        }

        /// 
        [HideInDocs]
        public override Vector3 Project(Vector4 v)
        {
            return ProjectPoint(view * v);
        }

        /// 
        [HideInDocs]
        public override Vector3 Project(Vector4 v, out bool culled)
        {
            culled = GetSideTest(view * v);
            return ProjectPoint(view * v);
        }

        /// <summary>
        /// This method always return the same simplex mode.
        /// </summary>
        public override SimplexMode SimplexModeForVisualizing(SimplexMode mode)
        {
            return mode;
        }

        enum FrustumCase { Inside, Intersect, Outside }

        void OnDrawGizmosSelected()
        {
            if (useFrustumCulling)
            {
                Gizmos4.DrawFrustum(-view.position, nearClip, farClip, focalLength, perspectiveness);
            }
        }
    }
}
