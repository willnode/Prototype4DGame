using Engine4.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine4
{
    /// <summary> Temporary representation of a 3D mesh </summary>
    [Serializable]
    public sealed class Buffer3
    {
        /// <summary> Vertices data </summary>
        public List<Vector3> m_Verts = new List<Vector3>();
        /// <summary> Color data </summary>
        public List<Color> m_Colors = new List<Color>();
        /// <summary> First UV data </summary>
        public List<UnityEngine.Vector4> m_Uv0 = new List<UnityEngine.Vector4>();
        /// <summary> Second UV data </summary>
        public List<UnityEngine.Vector4> m_Uv2 = new List<UnityEngine.Vector4>();
        /// <summary> Third UV data </summary>
        public List<UnityEngine.Vector4> m_Uv3 = new List<UnityEngine.Vector4>();
        /// <summary> Triangle or indices data </summary>
        public List<List<int>> m_Tris = new List<List<int>>();

        [SerializeField]
        int m_subMesh = 0;

        [NonSerialized]
        List<int> m_CurTris; // quick reference to current tris

        /// <summary> Create an empty buffer </summary>
        public Buffer3() { Clear(); }

        /// <summary> Create and import from a mesh </summary>
        public Buffer3(Mesh m) : this(m, -1) { }

        /// <summary> Create and import from a mesh within given submesh index </summary>
        public Buffer3(Mesh m, int submesh)
        {
            Clear();
            m.GetVertices(m_Verts);
            m.GetColors(m_Colors);
            m.GetUVs(0, m_Uv0);
            m.GetUVs(1, m_Uv2);
            m.GetUVs(2, m_Uv3);

            if (submesh >= 0)
            {
                SetSubmesh(0);
                m.GetTriangles(m_Tris[0], submesh);
            }
            else
            {
                SetSubmesh(m.subMeshCount - 1);
                for (int i = m.subMeshCount; i-- > 0;)
                {
                    m.GetIndices(m_Tris[i], i);
                }
            }
        }

        /// <summary> Clear the buffer </summary>
        public void Clear()
        {
            for (int i = m_Tris.Count; i-- > 0;)
            {
                ListPool<int>.Push(m_Tris[i]);
            }
            m_Verts.Clear();
            m_Tris.Clear();
            m_Colors.Clear();
            m_Uv0.Clear();
            m_Uv2.Clear();
            m_Uv3.Clear();
            m_Tris.Add(m_CurTris = ListPool<int>.Pop());
            m_subMesh = 0;
        }

        /// <summary> Apply to the mesh with given policy </summary>
        public void Apply(Mesh m, VertexProfiles profile, MeshTopology topology = MeshTopology.Triangles)
        {
            m.Clear();

            if (m_Verts.Count == 0) return;

            if (m_Verts.Count >= 0xFFFF)
                m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            m.SetVertices(m_Verts);

            m.subMeshCount = m_Tris.Count;

            for (int i = 0; i < m_Tris.Count; i++)
            {
                if (topology == MeshTopology.Triangles)
                    m.SetTriangles(m_Tris[i], i, false);
                else
                    m.SetIndices(m_Tris[i], topology, i, false);
            }

            m.RecalculateBounds();

            if (topology <= MeshTopology.Quads)
            {
                m.RecalculateNormals();
                m.RecalculateTangents();

                if (profile > 0)
                {
                    var uv = (int)profile;
                    if ((uv & 1) > 0) m.SetUVs(0, m_Uv0);
                    if ((uv & 2) > 0) m.SetUVs(1, m_Uv2);
                    if ((uv & 4) > 0) m.SetUVs(2, m_Uv3);
                }
            }

            if ((profile & VertexProfiles.Color) > 0)
                m.SetColors(m_Colors);

        }

        /// <summary> Add a single vertex </summary>
        public void AddVert(Vector3 v)
        {
            m_Verts.Add(v);
        }

        /// <summary> Add multiple vertices </summary>
        public void AddVert(IEnumerable<Vector3> v)
        {
            m_Verts.AddRange(v);
        }

        /// <summary> Add an indice </summary>
        public void AddTris(int i)
        {
            m_CurTris.Add(i);
        }

        /// <summary> Add triangle </summary>
        public void AddTris(int i, int j, int k)
        {
            m_CurTris.Add(i);
            m_CurTris.Add(j);
            m_CurTris.Add(k);
        }

        /// <summary> Add multiple indices </summary>
        public void AddTris(IEnumerable<int> i)
        {
            m_CurTris.AddRange(i);
        }
        /// <summary> Add an indice to given submesh </summary>
        public void AddTris(int i, int subMesh)
        {
            m_CurTris.Add(i);
        }

        /// <summary> Advance to the next submesh </summary>
        public void SetSubmesh()
        {
            if (m_CurTris.Count > 0)
                SetSubmesh(m_subMesh + 1);
        }

        /// <summary> Set current submesh writing index </summary>
        public void SetSubmesh(int idx)
        {
            while (m_Tris.Count <= idx)
            {
                m_Tris.Add(ListPool<int>.Pop());
            }
            m_CurTris = m_Tris[m_subMesh = idx];
        }

        /// <summary> Add a vertex profile </summary>
        public void AddProfile(VertexProfile p)
        {
            m_Colors.Add(p.color);
            m_Uv0.Add(p.uv);
            m_Uv2.Add(p.uv2);
            m_Uv3.Add(p.uv3);
        }

        /// <summary> Add multiple vertx profiles </summary>
        public void AddProfile(IEnumerable<VertexProfile> p)
        {
            foreach (var profile in p)
            {
                m_Colors.Add(profile.color);
                m_Uv0.Add(profile.uv);
                m_Uv2.Add(profile.uv2);
                m_Uv3.Add(profile.uv3);
            }
        }

        /// <summary> Import from a mesh </summary>
        public void AddMesh(Mesh m, bool mergeSubmesh = true, int subMeshIdx = -1)
        {
            if (m.vertexCount == 0)
                return;
            var v = new Buffer3(m, subMeshIdx);
            Append(v, mergeSubmesh, mergeSubmesh || m_Tris[m_subMesh].Count == 0 ? m_subMesh : m_subMesh + 1);
        }

        /// <summary> Combine with another buffer </summary>
        public void Append(Buffer3 v, bool mergeSubmesh, int startingSubmesh)
        {
            var offset = m_Verts.Count;
            m_Verts.AddRange(v.m_Verts);
            m_Uv0.AddRange(v.m_Uv0);
            m_Uv2.AddRange(v.m_Uv2);
            m_Uv3.AddRange(v.m_Uv3);

            for (int i = 0; i < v.m_Tris.Count; i++)
            {
                if (i == 0)
                    SetSubmesh(startingSubmesh);
                else if (!mergeSubmesh)
                    SetSubmesh();
                var t = v.m_Tris[i];

                for (int j = 0; j < t.Count; j++)
                {
                    AddTris(offset + t[j]);
                }
            }
        }

    }

}
