using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Default Engine4 visualizer for particles
    /// </summary>
    public class ParticleVisualizer4 : IVisualizer
    {

        Buffer3 helper;
        /// 
        [HideInDocs]
        public void Initialize(Buffer3 helper)
        {
            (this.helper = helper).Clear();
        }

        // Because it's a point, count is expected to be 1
        /// 
        [HideInDocs]
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
            helper.Apply(m, profile, MeshTopology.Points);
        }

        /// 
        [HideInDocs]
        public void Clear(Mesh m)
        {
            m.Clear();
        }

        /// <summary>
        /// Always return <see cref="SimplexMode.Point"/>
        /// </summary>
        public SimplexMode WorkingSimplexCase { get { return SimplexMode.Point; } }
    }
}
