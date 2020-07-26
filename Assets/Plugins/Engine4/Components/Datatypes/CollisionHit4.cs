using Engine4.Physics.Internal;

namespace Engine4.Physics
{
    /// <summary> Impact informations during physics collision </summary>
    public struct CollisionHit4
    {
        /// <summary>
        /// The state of the collision
        /// </summary>
        public CollisionState state;

        /// <summary> Is this a collision with other trigger? </summary>
        /// <remarks> Trigger-trigger collision is not supported </remarks>
        public bool sensor;

        /// <summary> The other collider we are colliding with </summary>
        public Collider4 other;

        /// <summary> The internal access to the manifold </summary>
        public Manifold manifold;

        /// <summary> The normal direction of the collision </summary>
        /// <remarks> The returning direction is toward to other's body </remarks>
        public Vector4 normal { get { return manifold.B.Tag.Equals(other) ? manifold.normal : -manifold.normal; } }

        /// <summary> The contact points of the collision </summary>
        /// <remarks> The returning point is the average between two overlapping points </remarks>
        public Vector4[] points { get { return manifold.position; } }

        /// <summary> The contact penetrations of the collision </summary>
        /// <remarks> Use this param to get precise information about contact points </remarks>
        public float[] depths { get { return manifold.depth; } }

        /// <summary>
        /// Internal quick constructor
        /// </summary>
        public CollisionHit4(CollisionState state, Contact c, bool isForA)
        {
            this.state = state;
            sensor = c.sensor;
            manifold = c.manifold;
            other = (Collider4)(isForA ? c.B.Tag : c.A.Tag);
        }

        /// <summary>
        /// Internal quick constructor
        /// </summary>
        public CollisionHit4(CollisionState state) : this()
        {
            this.state = state;
        }
    }
}
