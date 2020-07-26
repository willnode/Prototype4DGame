using Engine4.Internal;
using Engine4.Physics.Internal;
using Engine4.Rendering;

namespace Engine4.Physics
{
    /// <summary>
    /// Four-dimensional box collider for physics simulation
    /// </summary>
    public class BoxCollider4 : Collider4
    {

        internal override ShapeType type { get { return ShapeType.Box; } }

        /// <summary>
        /// Size (edge to edge distance) of the box
        /// </summary>
        public Vector4 size = Vector4.one;

        internal override void Synchronize()
        {
            base.Synchronize();
            
            var box = shape as Box;

            box.Extent = size * transform4.localScale * 0.5f;
        }

        /// 
        [HideInDocs]
        protected override void OnDrawGizmosSelected ()
        {
            base.OnDrawGizmosSelected();
            Gizmos4.DrawWireCube(Vector4.zero, size * transform4.localScale);
        }

    }
}
