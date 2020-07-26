//--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------

/**
Engine4.Physics.Internal Physics Engine (c) 2017 Wildan Mubarok

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software. If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not
be misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/
using System;

namespace Engine4.Physics.Internal
{

    public class Box : Shape
    {

        internal Vector4 extent = Vector4.one * 0.5f;

        internal override Bounds4 ComputeAABB(Matrix4x5 tx)
        {
            return (tx * local) * new Bounds4(extent);
        }

        internal override bool TestPoint(Matrix4x5 tx, Vector4 p)
        {
            Matrix4x5 world = tx * local;
            Vector4 p0 = p / world;

            for (int i = 0; i < 3; ++i)
            {
                if (Math.Abs(p0[i]) > extent[i]) return false;
            }
            return true;
        }

        internal override bool Raycast(ref RaycastHit4 raycast)
        {
            Matrix4x5 world = GetWorldTransform();
            Vector4 d = raycast.ray.direction / world.rotation;
            Vector4 p = raycast.ray.origin / world;
            float tmin = 0;//, tmax = raycast.t;

            // t = (e[ i ] - p.[ i ]) / d[ i ]
            float t0;
            Vector4 n0 = new Vector4();

            for (int i = 0; i < 3; ++i)
            {
                // Check for ray parallel to and outside of Bounds4
                if (Math.Abs(d[i]) < 1e-8)
                {
                    // Detect separating axes
                    if (Math.Abs(p[i]) > extent[i]) return false;
                }
                else
                {
                    float d0 = 1 / d[i];
                    float s = Math.Sign(d[i]);
                    float ei = extent[i] * s;
                    Vector4 n = new Vector4(i, -s);
                    
                    if ((t0 = (-ei - p[i]) * d0) > tmin)
                    {
                        n0 = n;
                        tmin = t0;
                    }
                }
            }

            raycast.hash = hash;
            raycast.normal = world.rotation * n0;
            raycast.distance = tmin;

            return true;
        }

        internal override void ComputeMass(out MassData md)
        {
            // Calculate inertia tensor
            float mass = 8 * extent.x * extent.y * extent.z * extent.w * density;
            Vector4 e2 = (extent * extent) * 4;
            Euler4 e3 = new Euler4(e2.y + e2.z, e2.x + e2.z, e2.x + e2.y, e2.w + e2.x, e2.w + e2.y, e2.w + e2.z);
            Euler4 e4 = e3 * (1f / 12f * mass);
            Tensor4 I = new Tensor4(e4);

            md = new MassData(I, local, mass);
        }

        // -- Public accessor

        public Box() : base(ShapeType.Box) { }

        public Vector4 Extent { get { return extent; } set { extent = value; if (body != null) body.flags |= BodyFlags.DirtyMass; } }

    }


}
