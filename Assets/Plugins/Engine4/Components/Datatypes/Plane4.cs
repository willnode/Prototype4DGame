namespace Engine4
{
    /// <summary>
    /// Mathematical abstraction of a plane that split spaces in 4D.
    /// </summary>
    public struct Plane4
    {
        /// <summary>
        /// Distance of the plane from center.
        /// </summary>
        public float distance;

        /// <summary>
        /// Normal of the plane.
        /// </summary>
        public Vector4 normal;

        /// <summary>
        /// Create new plane with given normal at given point.
        /// </summary>
        public Plane4(Vector4 norm, Vector4 point)
        {
            normal = Vector4.Normalize(norm);
            distance = Vector4.Dot(normal, point);
        }

        /// <summary>
        /// Create new plane with given normal and distance.
        /// </summary>
        public Plane4(Vector4 norm, float dist)
        {
            normal = Vector4.Normalize(norm);
            distance = dist;
        }

        /// <summary>
        /// Create a new plane from known vertices.
        /// </summary>
        public Plane4(Vector4 a, Vector4 b, Vector4 c, Vector4 d)
        {
            normal = Vector4.Normalize(Vector4.Cross(b - a, c - a, d - a));
            distance = Vector4.Dot(normal, a);
        }

        /// <summary>
        /// Get origin of a plane.
        /// </summary>
        public Vector4 origin { get { return normal * distance; } set { distance = Vector4.Dot(normal, value); } }

        /// <summary>
        /// Get distance between a point and the nearest point on a plane.
        /// </summary>
        /// <remarks> 
        /// This method can return a negative value, which mean the point is behind the plane. 
        /// </remarks>
        public float Distance(Vector4 point)
        {
            return Vector4.Dot(normal, point) - distance;
        }

        /// <summary>
        /// Is the point is above or behind the plane?
        /// </summary>
        public bool GetSide(Vector4 point)
        {
            return Vector4.Dot(this.normal, point) - this.distance > -1e-4;
        }

        /// <summary>
        /// Given an edge represented as two points, return an interpolation where this edge intersects.
        /// </summary>
        /// <remarks> 
        /// The resulting interpolation is unclamped, can go beyond 0..1
        /// </remarks>
        public float Intersect(Vector4 a, Vector4 b)
        {
            // Think of an inverse lerp function
            var x = Vector4.Dot(normal, a);
            var y = Vector4.Dot(normal, b);
            return y == x ? 0 : (distance - x) / (y - x);
        }


        /// <summary>
        /// Project the point to the nearest point on the plane.
        /// </summary>
        public Vector4 Project(Vector4 point)
        {
            return point - normal * Distance(point);
        }

        /// <summary>
        /// Is these two points on the same side of the plane?
        /// </summary>
        public bool SameSide(Vector4 a, Vector4 b)
        {
            var x = Vector4.Dot(normal, a) - distance;
            var y = Vector4.Dot(normal, b) - distance;
            return x * y > 0;
        }
    }
}