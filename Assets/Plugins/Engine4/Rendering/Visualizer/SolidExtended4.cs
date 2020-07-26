using Engine4.Internal;
using System;
using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Extended particle visualizer.
    /// </summary>
    public class SolidExtended4 : Visualizer4
    {
        /// <summary>
        /// Make normal smooth by merging vertices
        /// </summary>
        public bool smoothNormals = false;
        /// <summary>
        /// Make normal consistent
        /// </summary>
        public bool refineNormals = true;

        [NonSerialized]
        SolidVisualizer4 def = new SolidVisualizer4();

        /// <summary>
        /// Always return <see cref="SimplexMode.Triangle"/>
        /// </summary>
        public override SimplexMode WorkingSimplexCase
        {
            get
            {
                return SimplexMode.Triangle;
            }
        }

        /// 
        [HideInDocs]
        public override void Clear(Mesh m)
        {
            m.Clear();
        }

        /// 
        [HideInDocs]
        public override void End(Mesh m, VertexProfiles profile)
        {
            def.End(m, profile);
        }

        /// 
        [HideInDocs]
        public override void Initialize(Buffer3 helper)
        {
            def.smoothNormals = smoothNormals;
            def.refineNormals = refineNormals;
            def.Initialize(helper);
        }

        /// 
        [HideInDocs]
        public override void Render(VertexProfile[] buffer, int count)
        {
            def.Render(buffer, count);
        }

        /// 
        [HideInDocs]
        public override void Render(Vector4[] buffer, int count)
        {
            def.Render(buffer, count);
        }
    }
}
