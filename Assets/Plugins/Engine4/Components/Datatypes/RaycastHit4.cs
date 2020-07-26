namespace Engine4.Physics
{
    /// <summary>
    /// Raycasting information.
    /// </summary>
    public struct RaycastHit4
    {
        /// <summary> The collider hash that hits </summary>
        public int hash;

        /// <summary> The distance to impact point </summary>
        public float distance;

        /// <summary> The surface normal of the impact </summary>
        public Vector4 normal;

        /// <summary> The ray that generates the raycast </summary>
        public Ray4 ray;

        /// <summary> The world point to the impact </summary>
        public Vector4 point
        {
            get
            {
                return ray * distance;
            }
        }

        /// <summary>
        /// Create and initialize the raycast.
        /// </summary>
        public RaycastHit4(Ray4 ray) : this()
        {
            this.ray = ray;
        }

        /// <summary>
        /// Create, initialize, and report the raycast.
        /// </summary>
        public RaycastHit4(Ray4 ray, float distance, Vector4 normal, int hash)
        {
            this.ray = ray;
            this.distance = distance;
            this.normal = normal;
            this.hash = hash;
        }

        /// <summary>
        /// Report the raycast
        /// </summary>
        public void Set (float distance, Vector4 normal, int hash)
        {
            if (!hit || this.distance > distance)
            {
                this.distance = distance;
                this.normal = normal;
                this.hash = hash;
            }
        }

        /// <summary> Does raycasting succeeded? </summary>
        public bool hit { get { return hash > 0; } }

        /// <summary> The collider that hits </summary>
        public Collider4 collider { get { return hash > 0 ? (Collider4)Physics4.main.scene.Shapes[hash].Tag : null; } }
    }
}
