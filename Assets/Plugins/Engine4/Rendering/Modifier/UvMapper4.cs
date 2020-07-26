using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// UV map unwrapping methods
    /// </summary>
    public enum UvMapMethod
    {
        /// <summary>
        /// Rotate only. Nothing special.
        /// </summary>
        None,
        /// <summary>
        /// Flatten Indices based on its normal
        /// </summary>
        NormalBased,
        /// <summary>
        /// Flatten Indices based on given normal
        /// </summary>
        Directional,
        /// <summary>
        /// Spherical unwrapping
        /// </summary>
        Spherical,
        /// <summary>
        /// Cylindrical unwrapping
        /// </summary>
        Cylindrical
    }
  
    /// 
    [HideInDocs]
    public class UvMapper4 : Modifier4
    {
        /// <summary>
        /// Specify unwrapping method
        /// </summary>
        public UvMapMethod method;

        /// <summary>
        /// For <see cref="UvMapMethod.Directional"/> and <see cref="UvMapMethod.Cylindrical"/> specify a specific axis
        /// </summary>
        public Axis4 axis = Axis4.Y;

        /// <summary>
        /// Rotation matrix
        /// </summary>
        [Matrix4AsEuler]
        public Matrix4 rotate = Matrix4.identity;

        /// 
        [HideInDocs]
        public override void ModifyModel(Buffer4 buffer)
        {

            if ((renderer4.profile & VertexProfiles.UV) == 0) return;

            switch (method)
            {
                case UvMapMethod.None:
                    RotateOnly(buffer);
                    break;
                case UvMapMethod.NormalBased:
                    GenerateNormalBased(buffer);
                    break;
                case UvMapMethod.Directional:
                    GenerateDirectional(buffer);
                    break;
                case UvMapMethod.Spherical:
                    break;
                case UvMapMethod.Cylindrical:
                    break;
                default:
                    break;
            }
        }

        void RotateOnly(Buffer4 buffer)
        {
            var P = buffer.m_Profiles; var Pc = buffer.m_ProfilesCount;
            var m = (UnityEngine.Matrix4x4)rotate;
            for (int i = 0; i < Pc; i++)
            {
                P[i] = VertexProfile.Transform(m, P[i]);
            }
        }

        void GenerateNormalBased (Buffer4 buffer)
        {
            var I = buffer.m_Indices; var Ic = buffer.m_IndicesCount;
            var V = buffer.m_Vertices;
            Vector4 a, b, c, d;

            switch (buffer.simplex)
            {
                case SimplexMode.Point: // 0D UV?
                    break;
                case SimplexMode.Line: // 1D UV
                    for (int i = 0; i < Ic; i += 2)
                    {
                        a = V[I[i]];
                        b = V[I[i + 1]];
                        var q = Matrix4.LookAt(b - a);
                        var n = -q.ew;
                        // ProjectOnPlane
                        buffer.SetProfile(i + 0, (q * (a - n * Vector4.Dot(a, n))), 0);
                        buffer.SetProfile(i + 1, (q * (b - n * Vector4.Dot(b, n))), 0);
                    }
                    break;
                case SimplexMode.Triangle: // 2D UV
                    for (int i = 0; i < Ic; i += 3)
                    {
                        a = V[I[i]];
                        b = V[I[i + 1]];
                        c = V[I[i + 2]];
                        var q = Matrix4.LookAt(c - a, b - a);
                        var n = q.ew;
                        q = Matrix4.identity;// Matrix4.Transpose(q);

                        // ProjectOnPlane
                        buffer.SetProfile(i + 0, (q * (a - n * Vector4.Dot(a, n))), 0);
                        buffer.SetProfile(i + 1, (q * (b - n * Vector4.Dot(b, n))), 0);
                        buffer.SetProfile(i + 2, (q * (c - n * Vector4.Dot(c, n))), 0);
                    }
                    break;
                case SimplexMode.Tetrahedron: // 3D UV
                    for (int i = 0; i < Ic; i += 4)
                    {
                        a = V[I[i]];
                        b = V[I[i + 1]];
                        c = V[I[i + 2]];
                        d = V[I[i + 3]];

                        var q = Matrix4.LookAt(d - a, c - a, b - a);
                        var n = -q.ew;
                        // ProjectOnPlane
                        buffer.SetProfile(i + 0, (q * (n * Vector4.Dot(a, n))), 0);
                        buffer.SetProfile(i + 1, (q * (n * Vector4.Dot(b, n))), 0);
                        buffer.SetProfile(i + 2, (q * (n * Vector4.Dot(c, n))), 0);
                        buffer.SetProfile(i + 3, (q * (n * Vector4.Dot(d, n))), 0);
                    }
                    break;
                default:
                    break;
            }
        }

        void GenerateDirectional(Buffer4 buffer)
        {
            var I = buffer.m_Indices; var Ic = buffer.m_IndicesCount;
            var V = buffer.m_Vertices;
            var n = new Vector4((int)axis, 1);
            var q = rotate * Matrix4.Transpose(Matrix4.LookAt(n));

            for (int i = 0; i < Ic; i++)
            {
                var v = V[I[i]];
                buffer.SetProfile(i, (q * (v - n * Vector4.Dot(v, n))), 0);
            }
        }        
    }
}
