using Engine4.Internal;
using Engine4.Physics.Internal;
using Engine4.Rendering;

namespace Engine4.Physics
{
    /// <summary>
    /// Collider with shape of capsule
    /// </summary>
    public class CapsuleCollider4 : Collider4
    {

        internal override ShapeType type { get { return ShapeType.Capsule; } }

        /// <summary>
        /// Radius of the capsule
        /// </summary>
        public float radius = 0.5f;

        /// <summary>
        /// Height of the capsule
        /// </summary>
        public float height = 1f;

        internal override void Synchronize()
        {
            base.Synchronize();

            var capsule = shape as Capsule;

            capsule.Radius = radius;

            capsule.Extent = height * 0.5f;
        }

        /// 
        [HideInDocs]
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos4.DrawWireSphere(new Vector4(1, height * 0.5f), radius);
            base.OnDrawGizmosSelected();
            Gizmos4.DrawWireSphere(new Vector4(1, -height * 0.5f), radius);
        }
    }
}
