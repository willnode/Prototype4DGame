namespace Engine4
{
    /// <summary>
    /// Represent of a line that extends to infinity
    /// </summary>
    public struct Ray4
    {

        /// <summary>
        /// Direction of the ray
        /// </summary>
        public Vector4 direction;

        /// <summary>
        /// Starting position of the ray
        /// </summary>
        public Vector4 origin;

        /// <summary>
        /// Create new Ray.
        /// </summary>
        public Ray4 (Vector4 origin, Vector4 direction)
        {
            this.direction = direction;
            this.origin = origin;
        }

        /// <summary>
        /// Get a point that travels for given distance
        /// </summary>
        public Vector4 GetPoint (float distance)
        {
            return origin + direction * distance;
        }

        /// <summary>
        /// The GetPoint's exotic alternative
        /// </summary>
        public static Vector4 operator *(Ray4 ray, float distance)
        {
            return ray.GetPoint(distance);
        }

    }
}
