using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Base class for custom visualizer
    /// </summary>
    [RequireComponent(typeof(Renderer4)), DisallowMultipleComponent]
    public abstract class Visualizer4 : MonoBehaviour4, IVisualizer
    {

        /// <summary> Marks the beginning of the visualizing function </summary>
        public abstract void Initialize(Buffer3 helper);

        /// <summary> 
        /// Per-simplex visualize method
        /// Assume Vector4[] is Vector3[] with w = 0 each
        /// </summary>
        public abstract void Render(Vector4[] buffer, int count);

        /// <summary> 
        /// Per-simplex visualize method
        /// Assume Vector4[] is Vector3[] with w = 0 each
        /// </summary>
        public abstract void Render(VertexProfile[] buffer, int count);

        /// <summary> Marks the end of the visualizing function </summary>
        public abstract void End(Mesh m, VertexProfiles profile);

        /// <summary> Is this visualizer supports given simplex case? </summary>
        public abstract SimplexMode WorkingSimplexCase { get; }

        /// <summary> Tell to clear the buffer </summary>
        public abstract void Clear(Mesh m);

        /// 
        [HideInDocs]
        protected virtual void OnValidate()
        {
            renderer4.SetDirty(DirtyLevel.Transform);
        }

    }

    /// <summary>
    /// Inteface to the visualizer
    /// </summary>
    public interface IVisualizer
    {

        /// <summary> Marks the beginning of the visualizing function </summary>
        void Initialize(Buffer3 helper);

        /// <summary> 
        /// Per-simplex visualize method
        /// Assume Vector4[] is Vector3[] with w = 0 each
        /// </summary>
        void Render(Vector4[] buffer, int count);

        /// <summary> 
        /// Per-simplex profile visualize method
        /// </summary>
        void Render(VertexProfile[] buffer, int count);

        /// <summary> Marks the end of the visualizing function </summary>
        void End(Mesh m, VertexProfiles profile);

        /// <summary> Tell to clear the buffer </summary>
        void Clear(Mesh m);

        /// <summary> Is this visualizer supports given simplex case? </summary>
        SimplexMode WorkingSimplexCase { get; }
    }
}
