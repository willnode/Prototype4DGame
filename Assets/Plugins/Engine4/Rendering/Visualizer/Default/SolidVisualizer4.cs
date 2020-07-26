using Engine4.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Default Engine4 triangle visualizer
    /// </summary>
    public class SolidVisualizer4 : IVisualizer
    {
        Buffer3 helper;

        /// <summary>
        /// Should the visualizer recalculate the normal? (default is true)
        /// </summary>
        public bool refineNormals = true;

        /// <summary>
        /// Should the visualizer merges any duplicate vertex? (default is false)
        /// </summary>
        public bool smoothNormals = false;

        /// 
        [HideInDocs]
        public void Initialize(Buffer3 helper)
        {
            (this.helper = helper).Clear();
        }

        /// It's not always a triangle, sometimes could be a polygon
        [HideInDocs]
        public void Render(Vector4[] buffer, int count)
        {
            var o = helper.m_Verts.Count;
            helper.AddVert(buffer[0]);
            for (int i = 1; i < count;)
            {
                helper.AddVert(buffer[i]);
                helper.AddTris(0 + o, i + o, (++i % count) + o);
            }
        }

        /// 
        [HideInDocs]
        public void Render(VertexProfile[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                helper.AddProfile(buffer[i]);
            }
        }

        /// 
        [HideInDocs]
        public void End(Mesh m, VertexProfiles profile)
        {
            if (smoothNormals)
                MergeVertices();

            if (refineNormals)
                RefineTriangleOrder();

            helper.Apply(m, profile, MeshTopology.Triangles);
        }

        /// 
        [HideInDocs]
        public void Clear(Mesh m)
        {
            m.Clear();
        }

        /// <summary>
        /// Always return <see cref="SimplexMode.Triangle"/>
        /// </summary>
        public SimplexMode WorkingSimplexCase { get { return SimplexMode.Triangle; } }

        void RefineTriangleOrder()
        {
            // Make normal consistent so we don't get trouble with shadows.
            // TODO : How much this function cost?
            var t = helper.m_Tris[0]; var v = helper.m_Verts;

            Vector3 median = Utility.GetAverage(v) - Vector3.one * Mathf.Epsilon;

            for (int i = 0; i < t.Count; i += 3)
            {
                int a = t[i], b = t[i + 1], c = t[i + 2];
                Vector3 A = v[a], B = v[b], C = v[c];
                Vector3 N = Vector3.Cross(C - A, B - A);
                if (Vector3.Dot(N, median) < Vector3.Dot(N, A))
                {
                    // Flip
                    t[i] = c;
                    t[i + 2] = a;
                }
            }
        }

        Dictionary<Vector3, int> _merger = new Dictionary<Vector3, int>();

        /// <summary>
        /// Merge vertices, so that the resulted mesh is always have a smooth normal.
        /// </summary>
        void MergeVertices ()
        {
            
            var t = helper.m_Tris[0]; var v = helper.m_Verts;
            _merger.Clear();

            for (int i = 0; i < t.Count; i++)
            {
                var key = v[t[i]];// Utility.VectorToKey(v[t[i]]);
                if (_merger.ContainsKey(key))
                    t[i] = _merger[key];
                else
                    _merger[key] = t[i];
            }
        }


    }
}
