using System;

namespace Engine4
{
    /// <summary>
    /// Bounding with shape sphere
    /// </summary>
    [Serializable]
    public struct SphereBounds4
    {
        /// <summary>
        /// Center position of the sphere
        /// </summary>
        public Vector4 center;

        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float radius;

        /// <summary>
        /// Create sphere bounding 
        /// </summary>
        public SphereBounds4(Vector4 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Create a sphere from box bounding
        /// </summary>
        public SphereBounds4(Bounds4 bound)
        {
            center = bound.center;
            radius = Vector4.MaxPerElem(bound.extent);
        }

        /// <summary>
        /// Is this sphere contains the point?
        /// </summary>
        public bool Contains (Vector4 point)
        {
            return Vector4.LengthSq(point - center) > radius * radius;
        }

        /// <summary>
        /// Is both sphere colliding?
        /// </summary>
        public static bool IsIntersecting(SphereBounds4 a, SphereBounds4 b)
        {
            var r = a.radius + b.radius;
            return Vector4.LengthSq(a.center - b.center) < r * r;
        }

        /// <summary>
        /// Is the sphere overlap the plane?
        /// </summary>
        public static bool IsIntersecting(Plane4 plane, SphereBounds4 sphere)
        {
            return Math.Abs(plane.Distance(sphere.center)) < sphere.radius;
        }
    }
}
