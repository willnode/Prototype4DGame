using Engine4.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Extended particle visualizer.
    /// </summary>
    [ExecuteInEditMode]
    public class ParticleExtended4 : Visualizer4
    {
        /// <summary>
        /// Size of particle
        /// </summary>
        public float size;

        Buffer3 helper;
        List<Vector3> positions = new List<Vector3>();
        Mesh m;

        /// <summary>
        /// Always return <see cref="SimplexMode.Point"/>
        /// </summary>
        public override SimplexMode WorkingSimplexCase
        {
            get
            {
                return SimplexMode.Point;
            }
        }

        static Vector3[] verts = new Vector3[] { new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0) };
        static UnityEngine.Vector4[] uvs = new UnityEngine.Vector4[] { new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0) };

        /// 
        [HideInDocs]
        public void EndContinued(Camera camera)
        {
            if (m == null) return;
            var pos = viewer4.projector.Project(transform4.position);
            var q = Quaternion.LookRotation(camera.transform.position - pos);
            for (int i = 0; i < positions.Count; i++)
            {
                var p = positions[i];

                for (int j = 0; j < 4; j++)
                {
                    helper.m_Verts[i * 4 + j] = (q * verts[j] * size + p);
                }
            }

            m.SetVertices(helper.m_Verts);
        }
        /// 
        [HideInDocs]
        public override void End(Mesh m, VertexProfiles profile)
        {
            var camera = Runtime.GetCurrentCamera();
            if (camera == null) return;
            var pos = viewer4.projector.Project(transform4.position);
            var q = Quaternion.LookRotation(camera.position - pos);
            var iter = 0;

            for (int i = 0; i < positions.Count; i++)
            {
                var p = positions[i];

                for (int j = 0; j < 4; j++)
                {
                    helper.AddVert(q * verts[j] * size + p);
                    helper.AddTris(iter++);
                }
            }

            profile |= VertexProfiles.UV;

            for (int i = 0; i < positions.Count; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    helper.m_Uv0.Add(uvs[j]);
                }
            }
            
            helper.Apply(m, profile, MeshTopology.Quads);
            this.m = m;
        }

        /// 
        [HideInDocs]
        public override void Initialize(Buffer3 helper)
        {
            (this.helper = helper).Clear();
            positions.Clear();
        }

        /// 
        [HideInDocs]
        public override void Render(VertexProfile[] buffer, int count)
        {
            // Particle system handle profiles itself
        }

        /// 
        [HideInDocs]
        public override void Render(Vector4[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
            {
                positions.Add(buffer[i]);
            }
        }

        /// 
        [HideInDocs]
        public override void Clear(Mesh m)
        {
            this.m = null;
            m.Clear();
        }


        void OnEnable()
        {
            Camera.onPreCull += EndContinued;
        }

        void OnDisable()
        {
            helper.Clear();
            Camera.onPreCull -= EndContinued;
        }

    }
}
