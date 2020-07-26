using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Default visualizer for wires 
    /// </summary>
    public class WireVisualizer4 : IVisualizer
    {
        Buffer3 helper;

        /// 
        [HideInDocs]
        public void Initialize(Buffer3 helper)
        {
            (this.helper = helper).Clear();
        }

        /// 
        [HideInDocs]
        // Because it's a segment, count is expected to be 2
        public void Render(Vector4[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                helper.AddTris(helper.m_Verts.Count);
                helper.AddVert(buffer[i]);
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
            helper.Apply(m, profile, MeshTopology.Lines);
        }

        /// 
        [HideInDocs]
        public void Clear(Mesh m)
        {
            m.Clear();
        }

        /// <summary>
        /// Always return <see cref="SimplexMode.Line"/>
        /// </summary>
        public SimplexMode WorkingSimplexCase { get { return SimplexMode.Line; } }

    }
}
