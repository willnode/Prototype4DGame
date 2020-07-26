using Engine4.Internal;
using Engine4.Physics.Internal;
using Engine4.Rendering;

namespace Engine4.Physics
{
    /// <summary>
    /// Four-dimensional sphere collider for physics simulation
    /// </summary>
    public class SphereCollider4 : Collider4
    {

        internal override ShapeType type { get { return ShapeType.Sphere; } }

        /// <summary>
        /// Size (center to edge distance) of the sphere
        /// </summary>
        public float radius = 0.5f;

        internal override void Synchronize()
        {
            base.Synchronize();

            var sphere = shape as Sphere;

            sphere.Radius = radius * Vector4.MinPerElem(transform4.localScale);
        }

        /// 
        [HideInDocs]
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos4.DrawWireSphere(Vector4.zero, radius);
        }
    }

}
