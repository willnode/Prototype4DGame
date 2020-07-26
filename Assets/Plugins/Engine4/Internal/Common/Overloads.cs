using System;

namespace Engine4.Internal
{
    /// <summary>
    /// A library contain duplicates, internally meant for speed and exotic users.
    /// </summary>
    public static class Overloads
    {
        /// <seealso cref="Vector4.ComputeBasis(Vector4[])"/>
        public static Matrix4 ComputeBasis(Vector4 forward)
        {

            // Suppose Vector4 a has all equal components and is a unit Vector4: a = (s, s, s, s)
            // Then 4*s*s = 1, s = sqrt(1/4) = 0.5. This means that at least one component of a
            // unit Vector4 must be greater or equal to 0.5. 

            Vector4 a = forward, b, c;

            if (Math.Abs(a.x) >= 0.5)
            {
                b = new Vector4(a.y, -a.x, a.z, a.w);
                c = new Vector4(a.w, a.z, -a.x, a.y);
            }
            else if (Math.Abs(a.y) > 0.5)
            {
                b = new Vector4(-a.y, a.x, a.z, a.w);
                c = new Vector4(a.w, a.z, -a.y, a.x);
            }
            else
            {
                // X & Y cone
                b = new Vector4(a.x, a.y, a.w, -a.z);
                c = new Vector4(a.w, -a.z, a.x, a.y);
            }

            return new Matrix4(b, c, forward, Vector4.Cross(a, b, c));
        }

    }
}
