using Engine4.Internal;
using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Internal class to handle gizmo rendering in 4D
    /// </summary>
    /// <remarks>
    /// The component works magically under the hood
    /// </remarks>
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [AddComponentMenu("")]
    public class GizmosRenderer4 : MonoBehaviour4
    {
        
        internal IVisualizer visualizer;
        internal Buffer4 buffer = new Buffer4();
        internal Buffer3 output = new Buffer3();
        internal Mesh mesh;
        internal Material material;
        /// <summary>
        /// The gizmo renderer type
        /// </summary>
        public GizmoRendererType type;

        void OnEnable ()
        {
            if (type != GizmoRendererType.None)
                Init(type);
        }

        /// 
        [HideInDocs]
        public void Init(GizmoRendererType type)
        {
            if (!mesh) { mesh = new Mesh() { hideFlags = HideFlags.DontSave, name = "Gizmos4 Buffer" }; }

            if (!material) { material = new Material(Shader.Find("Hidden/Internal-Colored")) { hideFlags = HideFlags.DontSave, name = "Gizmos4 Material" }; }

            this.type = type;
            Clear();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().material = material;

        }

        /// 
        [HideInDocs]
        public void Clear()
        {

            if (type == GizmoRendererType.Solid)
            {
                buffer.Clear(viewer4.projector.SimplexModeForVisualizing(SimplexMode.Triangle));
                visualizer = (visualizer as SolidVisualizer4) ?? new SolidVisualizer4();
            }
            else
            {
                buffer.Clear(viewer4.projector.SimplexModeForVisualizing(SimplexMode.Line));
                visualizer = (visualizer as WireVisualizer4) ?? new WireVisualizer4();
            }

            output.Clear();
            mesh.Clear();
        }

        /// 
        [HideInDocs]
        public void Upload()
        {
            if (!buffer.IsEmpty())
            {
                visualizer.Initialize(output);
                viewer4.projector.Project(buffer, Matrix4x5.identity, visualizer);
                visualizer.End(mesh, VertexProfiles.Color);
            }
            else
                mesh.Clear();
        }

        /// 
        public enum GizmoRendererType
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Solid
            /// </summary>
            Solid = 1,
            /// <summary>
            /// Wire
            /// </summary>
            Wire = 2,
        }
    }
}
