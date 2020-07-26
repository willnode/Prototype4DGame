using Engine4.Internal;
using System;
using System.Runtime.InteropServices;

namespace Engine4
{
    /// <summary>
    /// A 4x5 matrix to describe 4D transformation.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4x5
    {
        /// <summary>
        /// Fifth column of the matrix, denoted as a translation vector.
        /// </summary>
        public Vector4 position;
        /// <summary>
        /// 4x4 part of the matrix, denoted as a rotational matrix.
        /// </summary>
        public Matrix4 rotation;

        /// <summary>
        /// Create a new 4x5 matrix.
        /// </summary>
        public Matrix4x5(Vector4 position, Matrix4 rotation) { this.position = position; this.rotation = rotation; }

        /// <summary>
        /// Transforms a point.
        /// </summary>
        /// <remarks>
        /// This operator does translate the vector,
        /// Otherwise you should only multiply with the rotational part of this 4x5 matrix.
        /// </remarks>
        public static Vector4 operator *(Matrix4x5 tx, Vector4 v)
        {
            return (tx.rotation * v + tx.position);
        }

        /// <summary>
        /// Combine two transformations.
        /// </summary>
        /// <remarks> 
        /// This operation is not commutative.
        /// </remarks>
        public static Matrix4x5 operator *(Matrix4x5 t, Matrix4x5 u)
        {
            return new Matrix4x5()
            {
                rotation = t.rotation * u.rotation,
                position = t.rotation * u.position + t.position
            };
        }

        /// <summary>
        /// Transform the right-hand plane.
        /// </summary>
        public static Plane4 operator *(Matrix4x5 tx, Plane4 p)
        {
            var origin = p.origin;
            origin = tx * origin;
            var normal = tx.rotation * p.normal;

            return new Plane4 { normal = normal, distance = Vector4.Dot(origin, normal) };
        }
        /// <summary>
        /// Convert to OBB, transform, then wrap it inside AABB back
        /// </summary>
        public static Bounds4 operator *(Matrix4x5 t, Bounds4 u)
        {
            var b = Bounds4.infinite;
            Vector4 e = u.extent, c = u.center;
            for (int i = 0; i < 16; i++)
            {
                b.Allocate(t * (e * Utility.BoxVertices[i]) + c);
            }
            return b;
        }

        /// <summary>
        /// Inversely transforms a point.
        /// </summary>
        /// <remarks>
        /// This operation is equivalent to Inverse(matrix) * vector
        /// </remarks>
        public static Vector4 operator /(Vector4 v, Matrix4x5 tx)
        {
            return Matrix4.Transpose(tx.rotation) * (v - tx.position);
        }

        /// <summary>
        /// Inversely combine two matrices
        /// </summary>
        /// <remarks>
        /// The operation is designed such that A * B / B is equivalent to A.
        /// </remarks>
        public static Matrix4x5 operator /(Matrix4x5 u, Matrix4x5 t)
        {
            return new Matrix4x5()
            {
                rotation = Matrix4.Transpose(t.rotation) * u.rotation,
                position = Matrix4.Transpose(t.rotation) * (u.position - t.position),
            };
        }

        /// <summary>
        /// Get a 4x5 identity matrix
        /// </summary>
        public static Matrix4x5 identity
        {
            get { return new Matrix4x5() { rotation = Matrix4.identity }; }
        }

        /// <summary>
        /// Inverse the matrix
        /// </summary>
        public static Matrix4x5 Inverse(Matrix4x5 t)
        {
            t.rotation = Matrix4.Transpose(t.rotation);
            t.position = t.rotation * -t.position;
            return t;
        }

        /// <summary> Convert to non-orthogonal matrix by applying a scale </summary>
        /// <remarks> Do not inverse or multiply any matrix produced using this method. </remarks>
        public Matrix4x5 ToTRS(Vector4 scale)
        {
            return new Matrix4x5 { position = position, rotation = rotation * new Matrix4(scale) };
        }

        /// <summary> Get the right (X+) axis of the transform </summary>
        public Vector4 rightward { get { return rotation.Column0; } }

        /// <summary> Get the up (Y+) axis of the transform </summary>
        public Vector4 upward { get { return rotation.Column1; } }

        /// <summary> Get the forward (Z+) axis of the transform </summary>
        public Vector4 forward { get { return rotation.Column2; } }

        /// <summary> Get the overward (W+) axis of the transform </summary>
        public Vector4 overward { get { return rotation.Column3; } }

        internal bool IsIdentity()
        {
            return position == Vector4.zero && rotation == Matrix4.identity;
        }
    }
}
