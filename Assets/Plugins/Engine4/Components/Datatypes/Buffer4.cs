using System;
using UnityEngine;
using AOORE = System.ArgumentOutOfRangeException;
using System.ComponentModel;
using System.Collections.Generic;

namespace Engine4
{

    /// <summary> 
    /// Temporary representation of 4D mesh 
    /// </summary>
    [Serializable]
    public class Buffer4
    {

        /// <summary> Internal simplex (indices) buffer. </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int[] m_Indices = new int[4];

        /// <summary> Internal vertex buffer </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4[] m_Vertices = new Vector4[4];

        /// <summary> Internal vertex profile buffer (color, uv). </summary>
        /// <remarks> Vertex profile respects to indices, not vertices </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public VertexProfile[] m_Profiles = new VertexProfile[4];

        [NonSerialized]
        public Queue<VertexProfile> m_ProfilesQueue = new Queue<VertexProfile>();

        /// <remarks> Get internal indices count </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int m_IndicesCount;

        /// <remarks> Get internal vertices count </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int m_VerticesCount;

        /// <remarks> Get internal profiles </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int m_ProfilesCount;

        /// <summary> Make sure internal indice array is enough for incoming number of data </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal void EnsureIndices(int incoming)
        {
            
            if (m_IndicesCount + incoming > m_Indices.Length)
                Array.Resize(ref m_Indices, Math.Max(m_IndicesCount + incoming, m_Indices.Length << 1));
        }

        /// <summary> Make sure internal indice array is enough for incoming number of data </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal void EnsureVertices(int incoming)
        {
            if (m_VerticesCount + incoming > m_Vertices.Length)
                Array.Resize(ref m_Vertices, Math.Max(m_VerticesCount + incoming, m_Vertices.Length << 1));
        }
        
        /// <summary> Make sure internal indice array is enough for incoming number of data </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal void EnsureProfiles(int incoming)
        {
            if (m_ProfilesCount + incoming > m_Profiles.Length)
                Array.Resize(ref m_Profiles, Math.Max(m_ProfilesCount + incoming, m_Profiles.Length << 1));
        }

        /// <summary> Create an empty buffer </summary>
        public Buffer4() { }

        /// <summary> Create an empty buffer </summary>
        public Buffer4(SimplexMode simplex) { this.simplex = simplex; }

        /// <summary> Is the buffer empty? </summary>
        public bool IsEmpty() { return m_VerticesCount == 0; }

        /// <summary> A handy pointer to shift vertex buffer values relatively. </summary>
        /// <remarks> It's very recommended to control this param using Align() instead. </remarks>
        public int offset = 0;

        /// <summary>
        /// The simplex type that required by the renderer.
        /// Useful if you want to create a custom shape blocks.
        /// </summary>
        /// <remarks> Public for speed. **ONLY SET WHEN ISEMPTY IS TRUE.** </remarks>
        public SimplexMode simplex = SimplexMode.Tetrahedron;

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void Clear()
        {
            m_ProfilesCount = m_VerticesCount = m_IndicesCount = 0;
            offset = 0;
        }

        /// <summary>
        /// Clear and Clean memory traces.
        /// </summary>
        public void Clean()
        {
            Clear();
            m_Indices = new int[4];
            m_Vertices = new Vector4[4];
            m_Profiles = new VertexProfile[4];
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void Clear(SimplexMode mode) { Clear(); simplex = mode; }

        /// <summary>
        /// Move buffer forward to the end.
        /// </summary>
        /// <remarks>
        /// Align are handy if used together with bulk operations (e.g. <see cref="Buffer4Extension.Sequence(Buffer4, SequenceMode)"/>)
        /// </remarks>
        public void Align() { offset = m_VerticesCount; }

        /// <summary>
        /// Move buffer toward the given snapshot.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Snapshot"/> to obtain snapshot at given position.
        /// </remarks>
        /// <seealso cref="Snapshot"/>
        public void Align(int snapshot) { offset = snapshot; }

        /// <summary>
        /// Send copy of current buffer position to be reused later.
        /// </summary>
        /// <seealso cref="Align(int)"/>
        public int Snapshot() { return m_VerticesCount; }

        void AddSimplex(int i)
        {
#if UNITY_EDITOR
            if (i + offset < 0 || i + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
#endif
            EnsureIndices(1);
            m_Indices[m_IndicesCount++] = (i + offset);
        }

        void AddSimplex(int a, int b)
        {
#if UNITY_EDITOR
            if (a + offset < 0 || a + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (b + offset < 0 || b + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
#endif
            EnsureIndices(2);
            m_Indices[m_IndicesCount++] = (a + offset);
            m_Indices[m_IndicesCount++] = (b + offset);
        }

        void AddSimplex(int a, int b, int c)
        {
#if UNITY_EDITOR
            if (a + offset < 0 || a + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (b + offset < 0 || b + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (c + offset < 0 || c + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
#endif
            EnsureIndices(3);
            m_Indices[m_IndicesCount++] = (a + offset);
            m_Indices[m_IndicesCount++] = (b + offset);
            m_Indices[m_IndicesCount++] = (c + offset);
        }

        void AddSimplex(int a, int b, int c, int d)
        {
#if UNITY_EDITOR
            if (a + offset < 0 || a + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (b + offset < 0 || b + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (c + offset < 0 || c + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
            if (d + offset < 0 || d + offset >= m_VerticesCount) throw new AOORE("Simplex is out of range");
#endif
            EnsureIndices(4);

            m_Indices[m_IndicesCount++] = (a + offset);
            m_Indices[m_IndicesCount++] = (b + offset);
            m_Indices[m_IndicesCount++] = (c + offset);
            m_Indices[m_IndicesCount++] = (d + offset);
        }


        /// <summary>
        /// Add an artbitrary point.
        /// </summary>
        public void AddPoint(int v0)
        {
            switch (simplex)
            {
                case SimplexMode.Point:
                    AddSimplex(v0);
                    break;
            }
        }

        /// <summary>
        /// Add a segment. Order doesn't matter.
        /// </summary>
        public void AddSegment(int v0, int v1)
        {
            switch (simplex)
            {
                case SimplexMode.Point:
                    AddSimplex(v0, v1);
                    break;
                case SimplexMode.Line:
                    AddSimplex(v0, v1);
                    break;
            }
        }

        /// <summary>
        /// Add a flat triangle. Order doesn't matter.
        /// </summary>
        public void AddTriangle(int v0, int v1, int v2)
        {
            switch (simplex)
            {
                case SimplexMode.Point:
                    AddSimplex(v0, v1, v2);
                    break;
                case SimplexMode.Line:
                    AddSimplex(v0, v1);
                    AddSimplex(v1, v2);
                    AddSimplex(v2, v0);
                    break;
                case SimplexMode.Triangle:
                    AddSimplex(v0, v1, v2);
                    break;
            }
        }

        /// <summary>
        /// Add a flat quad. Must be either clockwise/counter-clockwise order.
        /// </summary>
        public void AddQuad(int v0, int v1, int v2, int v3)
        {
            switch (simplex)
            {
                case SimplexMode.Point:
                    AddSimplex(v0, v1, v2, v3);
                    break;
                case SimplexMode.Line:
                    AddSimplex(v0, v1);
                    AddSimplex(v1, v2);
                    AddSimplex(v2, v3);
                    AddSimplex(v3, v0);
                    break;
                case SimplexMode.Triangle:
                    AddSimplex(v0, v1, v2);
                    AddSimplex(v2, v3, v0);
                    break;
            }
        }

        /// <summary>
        /// Add a trimid (triangle pyramid) with given vert indexs. Order doesn't matter.
        /// </summary>
        public void AddTrimid(int v0, int v1, int v2, int v3)
        {
            switch (simplex)
            {
                case SimplexMode.Point: // 4 vertexes
                    AddSimplex(v0, v1, v2, v3);
                    break;

                case SimplexMode.Line: // 6 edges
                    AddSimplex(v0, v1);
                    AddSimplex(v0, v2);
                    AddSimplex(v0, v3);
                    AddTriangle(v1, v2, v3);

                    break;
                case SimplexMode.Triangle: // 4 faces                    
                    AddTriangle(v0, v1, v2);
                    AddTriangle(v0, v2, v3);
                    AddTriangle(v0, v3, v1);
                    AddTriangle(v1, v2, v3);
                    break;

                case SimplexMode.Tetrahedron: // 1 cell
                    AddSimplex(v0, v1, v2, v3);
                    break;

            }
        }

        /// <summary>
        /// Helper to add a pyramid from 5 existing verts index.
        /// A pyramid is constructed from 2 trimids, v0 is the tip, the rest is the base.
        /// </summary>
        public void AddPyramid(int v0, int v1, int v2, int v3, int v4)
        {
            switch (simplex)
            {
                case SimplexMode.Point: // 5 vertexes
                    AddSimplex(v0, v1, v2);
                    AddSimplex(v3, v4);
                    break;

                case SimplexMode.Line: // 8 edges
                    AddSimplex(v0, v1);
                    AddSimplex(v0, v2);
                    AddSimplex(v0, v3);
                    AddSimplex(v0, v4);
                    AddQuad(v1, v2, v3, v4);
                    break;
                case SimplexMode.Triangle: // 6 faces (3 quads)
                    AddTriangle(v0, v1, v2);
                    AddTriangle(v0, v2, v3);
                    AddTriangle(v0, v3, v4);
                    AddTriangle(v0, v4, v1);
                    AddQuad(v1, v2, v3, v4);
                    break;

                case SimplexMode.Tetrahedron: // 2 cell
                    AddSimplex(v0, v1, v2, v3);
                    AddSimplex(v0, v1, v3, v4);
                    break;

            }
        }

        /// <summary>
        /// Helper to add a triangular prism from 6 existing verts index.
        /// A prism is constructed from 3 trimids, v0-v1-v2 must be parallel (and in the same order) with v3-v4-v5
        /// </summary>
        public void AddPrism(int v0, int v1, int v2, int v3, int v4, int v5)
        {
            switch (simplex)
            {
                case SimplexMode.Point: // 6 vertexes
                    AddSimplex(v0, v1, v2);
                    AddSimplex(v3, v4, v5);
                    break;

                case SimplexMode.Line: // 10 edges
                    AddTriangle(v0, v1, v2);
                    AddTriangle(v3, v4, v5);

                    AddSimplex(v0, v3);
                    AddSimplex(v1, v4);
                    AddSimplex(v2, v5);
                    break;
                case SimplexMode.Triangle: // 5 faces
                    AddTriangle(v0, v1, v2);
                    AddTriangle(v3, v4, v5);

                    AddQuad(v0, v1, v4, v3);
                    AddQuad(v1, v2, v5, v4);
                    AddQuad(v2, v1, v3, v5);
                    break;

                case SimplexMode.Tetrahedron: // 3 cells
                    AddSimplex(v0, v1, v2, v3);
                    AddSimplex(v3, v4, v5, v2);
                    AddSimplex(v1, v4, v3, v2);
                    break;

            }
        }

        /// <summary>
        /// Helper to add a cube from 8 existing verts index.
        /// A cube is constructed from 5 trimids, v0-v1-v2-v3 must be parallel (and in the same order) with v4-v5-v6-v7
        /// </summary>
        public void AddCube(int v0, int v1, int v2, int v3, int v4, int v5, int v6, int v7)
        {
            switch (simplex)
            {
                case SimplexMode.Point: // 8 vertices
                    AddSimplex(v0, v1, v2, v3);
                    AddSimplex(v4, v5, v6, v7);
                    break;

                case SimplexMode.Line: // 12 edges
                    AddQuad(v0, v1, v2, v3);
                    AddQuad(v4, v5, v6, v7);

                    AddSimplex(v0, v4);
                    AddSimplex(v1, v5);
                    AddSimplex(v2, v6);
                    AddSimplex(v3, v7);
                    break;

                case SimplexMode.Triangle: //  12 faces (6 quads)
                    // topdown
                    AddQuad(v0, v1, v2, v3);
                    AddQuad(v4, v5, v6, v7);
                    // leftright
                    AddQuad(v0, v1, v5, v4);
                    AddQuad(v2, v3, v7, v6);
                    // frontback
                    AddQuad(v1, v2, v6, v5);
                    AddQuad(v0, v3, v7, v4);
                    break;

                case SimplexMode.Tetrahedron: // 5 cells
                    AddSimplex(v0, v1, v2, v5);
                    AddSimplex(v0, v3, v2, v7);
                    AddSimplex(v0, v4, v5, v7);
                    AddSimplex(v2, v5, v7, v6);
                    AddSimplex(v0, v2, v5, v7);
                    break;

            }
        }

        /// <summary> Duplicate vertex and return the cloned index </summary>
        /// <remarks> Useful for sequencing operations </remarks>
        public int AddVertex(int idx)
        {
            EnsureVertices(1);
            m_Vertices[m_VerticesCount] = (m_Vertices[idx + offset]);
            return m_VerticesCount++ - offset;
        }

        /// <summary> Add a vertex with default profile and return the index </summary>
        public int AddVertex(Vector4 vert)
        {
            EnsureVertices(1);
            m_Vertices[m_VerticesCount] = vert;
            return m_VerticesCount++ - offset;
        }

        /// <summary> Add a new profile to remaining indice </summary>
        public void AddProfile(VertexProfile profile)
        {
            EnsureProfiles(1);
            m_Profiles[m_ProfilesCount++] = (profile);
        }

        /// <summary> Modify profile color at given indice index </summary>
        public void SetProfile(int index, Color color)
        {
            CheckProfileAt(index);
            var prof = m_Profiles[index];
            prof.color = color;
            m_Profiles[index] = prof;
        }

        /// <summary> Modify profile uv at given indice index </summary>
        public void SetProfile(int index, UnityEngine.Vector4 uv, int uvIndex)
        {
            CheckProfileAt(index);
            var prof = m_Profiles[index];
            switch (uvIndex)
            {
                case 0: prof.uv = uv; break;
                case 1: prof.uv2 = uv; break;
                case 2: prof.uv3 = uv; break;
            }
            m_Profiles[index] = prof;
        }

        void CheckProfileAt(int index)
        {
            if (index >= m_ProfilesCount)
            {
                m_ProfilesCount = index + 1;
                if (index >= m_Profiles.Length)
                    Array.Resize(ref m_Profiles, Math.Max(index + 1, m_Profiles.Length << 1));
            }
        }
    }

}
