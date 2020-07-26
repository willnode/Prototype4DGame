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
using Math = UnityEngine.Mathf;

namespace Engine4.Physics.Internal
{
    public class Sphere : Shape
    {
        internal float radius = 0.5f;
        
        internal override Bounds4 ComputeAABB(Matrix4x5 tx)
        {
            Matrix4x5 world = tx * local;
            Vector4 axis = new Vector4(radius);
            return new Bounds4(world.position - axis, world.position + axis);
        }

        internal override void ComputeMass(out MassData md)
        {
            // Calculate inertia tensor

            float mass = 4 / 3f * Math.PI * radius * radius * radius * density;
            float i = 2 / 5f * mass * radius * radius;
            Tensor4 I = new Tensor4(i);

            // Matrix4x5 tensor to local space
            md = new MassData(I, local, mass);
        }

        internal override bool Raycast(ref RaycastHit4 raycast)
        {
            Matrix4x5 world = GetWorldTransform();
            Vector4 d = raycast.ray.direction / world.rotation;
            Vector4 p = raycast.ray.origin / world;

            var tca = Vector4.Dot(p, d);
            // If this sphere behind the ray
            if (tca < 0) return false;

            var tcd = Vector4.Dot(p, p) - tca * tca;
            // If this ray completely miss it
            if (tcd > radius * radius) return false;

            // 100% chance of hit. Calculate now.
            var thc = Math.Sqrt(radius * radius - tcd);
            var toi = tca - thc;

            raycast.Set(toi, Vector4.Normalize(p + d * toi), hash);

            return true;
        }

        internal override bool TestPoint(Matrix4x5 tx, Vector4 p)
        {
            return Vector4.LengthSq(p / (tx * local)) <= radius * radius;
        }
        
        // -- Public accessor

        public Sphere() : base(ShapeType.Sphere) { }

        public float Radius { get { return radius; } set { radius = value; if (body != null) body.flags |= BodyFlags.DirtyMass; } }


    }
}
