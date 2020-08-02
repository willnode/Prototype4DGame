using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Gizmo container for rendering gizmo in 4D
    /// </summary>
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter)), DisallowMultipleComponent, ExecuteInEditMode]
    public class GizmosContainer4 : MonoBehaviour4
    {
        /// <summary>
        /// Gizmo renderer for solid objects
        /// </summary>
        public GizmosRenderer4 solid;

        /// <summary>
        /// Gizmo renderer for wire objects
        /// </summary>
        public GizmosRenderer4 wire;

        internal bool dirty;

        void OnEnable()
        {
            Validate();
#if UNITY_EDITOR
            // Script execution order do not work for gizmo, so we cheat
            UnityEditor.EditorApplication.update += Redraw;
#endif
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Redraw;
#endif
        }

        void Validate()
        {
            if (!solid) solid = CreateRenderer(GizmosRenderer4.GizmoRendererType.Solid);
            if (!wire) wire = CreateRenderer(GizmosRenderer4.GizmoRendererType.Wire);
            solid.Clear();
            wire.Clear();
        }

        void Redraw()
        {
            if (dirty)
            {
                Realign();
                solid.Upload();
                wire.Upload();
                dirty = false;
#if UNITY_EDITOR
                // this Redraw() is called by update, so we need to alert manually
                UnityEditor.SceneView.RepaintAll();
#endif
            }
        }

        void OnDrawGizmos()
        {
            if (!dirty && !(solid.buffer.IsEmpty() && wire.buffer.IsEmpty()))
            {
                SetDirty(); // Cleanup the traced mesh
            }
        }

        internal void SetDirty()
        {
            if (!dirty)
            {
                solid.Clear();
                wire.Clear();
                dirty = true;
            }
        }

        internal void Realign ()
        {
            //solid.buffer.Colorize(_color);
            //solid.buffer.Transform(_matrix);
            solid.buffer.Align();
            //wire.buffer.Colorize(_color);
            //wire.buffer.Transform(_matrix);
            wire.buffer.Align();
        }

        static GizmosContainer4 _main;

        /// <summary>
        /// Access to the main component of GizmoContainer4
        /// </summary>
        public static GizmosContainer4 main
        {
            get
            {
                return _main ? _main : CreateContainer();
            }
        }

        static GizmosContainer4 CreateContainer()
        {
            var view = Viewer4.main.gameObject;
            if (_main = view.GetComponentInChildren<GizmosContainer4>()) return _main;
            var obj = new GameObject("Gizmos4 Container", typeof(GizmosContainer4));
            obj.hideFlags = HideFlags.HideAndDontSave;
            obj.transform.SetParent(view.transform, false);
            return _main = obj.GetComponent<GizmosContainer4>();
        }

        GizmosRenderer4 CreateRenderer(GizmosRenderer4.GizmoRendererType type)
        {
            var obj = new GameObject("Gizmos4 Renderer", typeof(GizmosRenderer4));
            obj.hideFlags = HideFlags.HideAndDontSave;
            obj.transform.SetParent(gameObject.transform, false);
            var rend = obj.GetComponent<GizmosRenderer4>();
            rend.Init(type);
            return rend;
        }
    }
}
