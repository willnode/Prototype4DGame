using System;

namespace Engine4
{
    /// <summary> Axis in enumeration </summary>
    public enum Axis4
    {
        /// <summary> X (first) axis </summary>
        X,
        /// <summary> Y (second) axis </summary>
        Y,
        /// <summary> Z (third) axis </summary>
        Z,
        /// <summary> W (fourth) axis </summary>
        W
    }
    /// <summary> Additional channels to be included when generating the mesh output </summary>
    [Flags]
    public enum VertexProfiles
    {
        /// <summary> Only generate vertices and indices </summary>
        None = 0,
        /// <summary> Generate first UV </summary>
        UV = 1,
        /// <summary> Generate first, second UV </summary>
        UV2 = 2 | 1,
        /// <summary> Generate first, second, and third UV </summary>
        UV3 = 4 | 2 | 1,
        /// <summary> Generate color </summary>
        Color = 8,
        /// <summary> Generate color and first UV </summary>
        ColorAndUV = 8 | 1,
        /// <summary> Generate color and first, second UV </summary>
        ColorAndUV2 = 8 | 1 | 2,
        /// <summary> Generate color and first, secord, third UV </summary>
        ColorAndUV3 = 8 | 1 | 2 | 4,
    }

    /// <summary> Projection modes </summary>
    /// <seealso cref="Viewer4"/>
    public enum ProjectionMode
    {
        /// <summary> Cross Section </summary>
        CrossSection = 0,
        /// <summary> Frustum </summary>
        Frustum = 1,
    }

    /// <summary> Simplex contain points. Simplex count must be divisible by 1 </summary>
    public enum SimplexMode
    {
        /// <summary> Simplex contain points. Simplex count must be divisible by 1 </summary>
        Point = 0,
        /// <summary> Simplex contain line segments. Simplex count must be divisible by 2 </summary>
        Line = 1,
        /// <summary> Simplex contain flat triangles. Simplex count must be divisible by 3 </summary>
        Triangle = 2,
        /// <summary> Simplex contain volume tetrahedrons. Simplex count must be divisible by 4 </summary>
        Tetrahedron = 3,
    }

    /// <summary> What kind of visualization need to be shown? </summary>
    public enum VisualizeMode
    {
        /// <summary> Visualize using mesh points </summary>
        Particle = 0,
        /// <summary> Visualize using mesh wires (wireframe) </summary>
        Wire = 1,
        /// <summary> Visualize using mesh triangles </summary>
        Solid = 2,
        /// <summary> Use custom visualizer attached in the game object </summary>
        Custom = 3,
    }

    /// <summary> Space relatives for transformations </summary>
    public enum Space4
    {
        /// <summary> Transform is applied from the local coordinate of an object </summary>
        Self,
        /// <summary> Transform is applied from the four-dimensional world coordinate of an object </summary>
        World,
        /// <summary> Transform is applied from the projected three-dimensional world coordinate of an object </summary>
        View,
    }

    /// <summary> Collision state of an impact </summary>
    public enum CollisionState
    {
        /// <summary> Collision is just started </summary>
        Enter = 0,
        /// <summary> Collision is still happening </summary>
        Stay = 1,
        /// <summary> Collision have been ended </summary>
        Exit = 2,
    }

    /// <summary> Type of a rigidbody </summary>
    public enum BodyType
    {
        /// <summary> The rigidbody doesn't receive and react to any impact and forces </summary>
        StaticBody,
        /// <summary> The rigidbody is simulated like a real object </summary>
        DynamicBody,
        /// <summary> The rigidbody only gives impact to other bodies </summary>
        KinematicBody
    }

    /// <summary> Kind of validation requested </summary>
    public enum DirtyLevel
    {
        /// <summary> No update request </summary>
        None = 0,
        /// <summary> Request visualizer update </summary>
        Visualizer = 1,
        /// <summary> Request transform and visualizer update </summary>
        Transform = 2,
        /// <summary> Request modifier and transform and visualizer update </summary>
        Modifier = 3,
        /// <summary> Request model and modifier and transform and visualizer update </summary>
        Model = 4,
    }

    /// <summary>
    /// Vertex profile extensions
    /// </summary>
    public static class VertexProfilesUtility
    {
        /// <summary>
        /// Is UV calculated?
        /// </summary>
        public static bool HasUV(this VertexProfiles v)
        {
            return ((int)v & 1) > 0;
        }

        /// <summary>
        /// Is UV2 calculated?
        /// </summary>
        public static bool HasUV2(this VertexProfiles v)
        {
            return ((int)v & 2) > 0;
        }

        /// <summary>
        /// Is UV3 calculated?
        /// </summary>
        public static bool HasUV3(this VertexProfiles v)
        {
            return ((int)v & 4) > 0;
        }

        /// <summary>
        /// Is Color calculated?
        /// </summary>
        public static bool HasColor(this VertexProfiles v)
        {
            return ((int)v & 8) > 0;
        }

    }

}
