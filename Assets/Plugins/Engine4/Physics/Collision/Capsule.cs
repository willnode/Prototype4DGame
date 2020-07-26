//--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------

using Engine4.Internal;
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
using Math = UnityEngine.Mathf;

namespace Engine4.Physics.Internal
{

    // The capsule uses Y as the polar caps
    public class Capsule : Shape
    {

        internal float extent = 1;

        internal float radius = 0.5f;

        internal override Bounds4 ComputeAABB(Matrix4x5 tx)
        {
            Matrix4x5 world = tx * local;
            Vector4 axis = new Vector4(radius, extent + radius, radius, radius);
            return world * new Bounds4(axis);
        }

        internal override void ComputeMass(out MassData md)
        {
           
            float mass = density * radius * radius * Math.PI * ((4 / 3f * radius) + (extent * 2));
            
            float r = mass * ((21 / 20f) * radius * radius + (6 / 4f) * radius * extent + (4 / 3f) * extent * extent);

            float h = 13 / 10 * mass * radius * radius;

            Tensor4 I = new Tensor4(new Euler4(r, h, r, h, r, h));

            md = new MassData(I, local, mass);
        }

        internal override bool Raycast(ref RaycastHit4 raycast)
        {
            throw new NotImplementedException();
        }

        internal override bool TestPoint(Matrix4x5 tx, Vector4 p)
        {
            Vector4 p0 = p / (tx * local);

            p0.y = Math.Abs(p.y) >= extent ? p.y - Utility.Sign(p.y) * extent : 0;

            return Vector4.LengthSq(p0) <= radius * radius;
        }

        // -- Public accessor

        public Capsule() : base(ShapeType.Capsule) { }

        public float Extent { get { return extent; } set { extent = value; if (body != null) body.flags |= BodyFlags.DirtyMass; } }

        public float Radius { get { return radius; } set { radius = value; if (body != null) body.flags |= BodyFlags.DirtyMass; } }

    }
}
