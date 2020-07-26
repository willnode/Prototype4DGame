using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// General editing-aid when working for gizmos in 4D
    /// </summary>
    public static class Gizmos4
    {

        static Gizmos4 () { color = Color.white; matrix = Matrix4x5.identity; }
        //internal static Matrix4x5 _matrix;

        //internal static Color _color;

        /// <summary>
        ///  Set the gizmo color used to draw next gizmos.
        /// </summary>
        static public Color color { get; set; }
        /// <summary>
        ///  Set the gizmo matrix used to draw all gizmos.
        /// </summary>
        static public Matrix4x5 matrix { get; set; }
        
        /// <summary> Get Solid Buffer </summary>
        static Buffer4 S { get { return GizmosContainer4.main.solid.buffer; } }

        /// <summary> Get Wire Buffer </summary>
        static Buffer4 W { get { return GizmosContainer4.main.wire.buffer; } }

        /// <summary> Mark as dirty </summary>
        static void D () { GizmosContainer4.main.SetDirty(); }

        /// <summary> Draws a ray starting at from to from + direction </summary>
        public static void DrawRay(Ray r)
        {
            Gizmos.DrawLine(r.origin, r.origin + r.direction);
        }

        /// <summary> Draws a ray starting at from to from + direction </summary>
        public static void DrawRay(Vector4 from, Vector4 direction)
        {
            D();
            S.Align();
            S.AddSegment(S.AddVertex(from), S.AddVertex(from + direction));
            W.Transform(matrix);
            W.Colorize(color);
        }

        /// <summary> Draws a line starting at from towards to </summary>
        public static void DrawLine(Vector4 from, Vector4 to)
        {
            D();
            S.Align();
            S.AddSegment(S.AddVertex(from), S.AddVertex(to));
            W.Transform(matrix);
            W.Colorize(color);
        }

        /// <summary> Draws a wireframe sphere with center and radius. </summary>
        public static void DrawWireSphere(Vector4 center, float radius)
        {
            D();
            Buffer4Template.MakeHypersphereMeridians(W, radius);
            W.Transform(matrix, center);
            W.Colorize(color);
        }

        /// <summary> Draws a solid sphere with center and radius. </summary>
        public static void DrawSphere(Vector4 center, float radius)
        {
            D();
            Buffer4Template.MakeHypersphere(S, radius);
            S.Transform(matrix, center);
            S.Colorize(color);
        }


        /// <summary> Draw a wireframe box with center and size.</summary>
        public static void DrawWireCube(Vector4 center, Vector4 size)
        {
            D();
            Buffer4Template.MakeHypercube(W, size * 0.5f);
            W.Transform(matrix, center);
            W.Colorize(color);
        }

        /// <summary> Draw a solid box with center and size. </summary>
        public static void DrawCube(Vector4 center, Vector4 size)
        {
            D();
            Buffer4Template.MakeHypercube(S, size * 0.5f);
            S.Transform(matrix, center);
            S.Colorize(color);
        }


        /// <summary>
        ///   <para>Draw a camera frustum using the currently set Gizmos.matrix for it's location and rotation.</para>
        /// </summary>
        public static void DrawFrustum(Vector4 center, float min, float max, float focal, float perspectiveness)
        {
            D();
            Buffer4Template.MakeHyperfrustum(W, min, max, focal, perspectiveness);
            W.Transform(matrix, center);
            W.Colorize(color);
        }

    }
}
