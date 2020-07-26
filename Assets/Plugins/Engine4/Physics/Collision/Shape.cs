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

namespace Engine4.Physics.Internal
{
    public abstract class Shape : IEquatable<Shape>
    {

        protected Shape(ShapeType type) 
        {
            this.type = type;
        }

        internal Body body;

        internal int hash;

        internal int broadPhaseIndex;

        internal readonly ShapeType type;

        internal float density = 1f;

        internal bool sensor = false;

        internal float friction = 0.2f;

        internal float restitution = 0.1f;

        internal Matrix4x5 local = Matrix4x5.identity;

        internal abstract bool TestPoint(Matrix4x5 tx, Vector4 p);

        internal abstract bool Raycast(ref RaycastHit4 raycast);

        internal abstract Bounds4 ComputeAABB(Matrix4x5 tx);

        internal abstract void ComputeMass(out MassData md);

        internal void CheckHash()
        {
            if (body == null || body.scene == null) throw new InvalidOperationException();
            var hashes = body.scene.Shapes;
            if (hashes.Contains(hash)) return;
            if (hash == 0)
            {
                if (hashes.Count >= int.MaxValue - 1)
                    throw new InvalidOperationException("Dude. Too much objects!");

                do hash = Utility.Random(); while (hashes.Contains(hash) || hash == 0);

                hashes.Add(this);
            }
            else
            {
                // Something wrong?
                throw new InvalidOperationException();
            }
        }

        internal void RemoveHash()
        {
            if (body == null || body.scene == null) throw new InvalidOperationException();
            var hashes = body.scene.Shapes;
            if (hashes.Contains(hash))
            {
                hashes.Remove(this);
                hash = 0;
            }
        }

        internal Matrix4x5 GetWorldTransform()
        {
            return (body.flags & BodyFlags.Identity) > 0 ? body.Tx : body.Tx * local;
        }

        // -- Public accessor

        public object Tag { get; set; }

        public Body Body { get { return body; } }

        public bool Sensor { get { return sensor; } set { sensor = value; } }

        public float Density { get { return density; } set { density = Math.Max(0, value); } }

        public float Friction { get { return friction; } set { friction = Utility.Clamp01(value); } }

        public float Restitution { get { return restitution; } set { restitution = Utility.Clamp01(value); } }

        public Matrix4x5 Local { get { return local; } set { local = value; if (body != null) body.flags |= BodyFlags.DirtyMass; } }

        public static Shape CreateShape(ShapeType type)
        {
            switch (type)
            {
                case ShapeType.Box: return new Box();
                case ShapeType.Sphere: return new Sphere();
                case ShapeType.Capsule: return new Capsule();
                default: throw new ArgumentException();
            }
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public bool Equals(Shape other)
        {
            return other != null && other.hash == hash;
        }

        public override bool Equals(object obj)
        {
            return obj is Shape && ((Shape)obj).hash == hash;
        }
    }

    public struct MassData
    {
        public Tensor4 inertia;
        public Vector4 center;
        public float mass;

        public MassData(Tensor4 I, Matrix4x5 local, float mass)
        {
            // Matrix4x5 Inertia based on local space

            center = local.position;
            inertia = Tensor4.Transform(I, local, mass);
            this.mass = mass;
        }
    }

    public enum ShapeType
    {
        Box = 0,
        Sphere = 1,
        Capsule = 2,
    }

}