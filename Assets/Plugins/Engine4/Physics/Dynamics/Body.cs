
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Engine4.Physics.Internal
{


    [Flags]
    public enum BodyFlags
    {
        /// The body is static (passive)
        Static = 0x001,
        /// The body is dynamic (active)
        Dynamic = 0x002,
        /// The body is kinematic (passive but can interact)
        Kinematic = 0x004,
        /// The body won't translate in the simulation
        ZeroMass = 0x008,
        /// The body won't rotate in the simulation
        ZeroInertia = 0x010,
        /// The body is calculated in any physical interaction
        Active = 0x020,
        /// The body is allowed to sleep
        AllowSleep = 0x040,
        /// The body is actively listening and receiving contacts
        Awake = 0x080,
        /// The body has been solved in island
        Island = 0x100,
        /// The body has only one shape with identity local transformation
        Identity = 0x200,
        /// They body mass and inertia need to be recalculated
        DirtyMass = 0x400,
    }


    public class Body
    {

        internal Tensor4 invInertiaModel;

        internal Tensor4 invInertiaWorld;

        internal float mass;

        internal float invMass;

        internal Vector4 linearVelocity;

        internal Euler4 angularVelocity;

        internal Vector4 force;

        internal Euler4 torque;

        /// Compound transform
        internal Matrix4x5 Tx = Matrix4x5.identity;

        /// Local body center
        internal Vector4 C = Vector4.zero;

        /// World body center
        internal Vector4 P = Vector4.zero;

        internal float sleepTime;

        internal int islandIndex;

        internal BodyFlags flags = BodyFlags.Active | BodyFlags.AllowSleep | BodyFlags.Static | BodyFlags.Awake | BodyFlags.DirtyMass;

        internal readonly List<Shape> shapes = new List<Shape>();

        internal readonly List<ContactEdge> contactList = new List<ContactEdge>();

        internal Scene scene;

        internal int layers = 0x1;

        internal float gravityScale = 1;

        internal float linearDamping = 0.1f;

        internal float angularDamping = 0.1f;

        internal void CalculateMassData()
        {

            SynchronizeProxies();

            Tensor4 inertia = Tensor4.zero;
            Vector4 lc = Vector4.zero;
            invMass = mass = 0;
            MassData md;

            flags &= ~BodyFlags.DirtyMass;

            
            if (shapes.Count == 1 && shapes[0].local.IsIdentity())
                flags |= BodyFlags.Identity;
            else
                flags &= ~BodyFlags.Identity;

            if ((flags & (BodyFlags.Static | BodyFlags.Kinematic)) > 0)
            {
                C = Vector4.zero;
                P = Tx.position;
                invInertiaModel = invInertiaWorld = Tensor4.zero;
                return;
            }

            foreach (var shape in shapes)
            {
                if (shape.density == 0)
                    continue;

                shape.ComputeMass(out md);
                mass += md.mass;
                inertia += md.inertia;
                lc += md.center * md.mass;
            }

            if (mass > 0)
            {
                lc *= (invMass = 1 / mass);
                inertia -= Tensor4.Transform(lc, mass);
                invInertiaModel = Tensor4.Inverse(inertia);
            }
            else
            {
                // force the dynamic body to have some mass
                mass = 0; invMass = 1;
                invInertiaModel = invInertiaWorld = Tensor4.zero;
            }


            if ((flags & BodyFlags.ZeroMass) > 0) invMass = 0;
            if ((flags & BodyFlags.ZeroInertia) > 0) invInertiaModel = Tensor4.zero;

            P = Tx * (C = lc);
        }

        internal void SynchronizeProxies()
        {

            if (scene == null)
                return;

            BroadPhase broadphase = scene.ContactManager.Broadphase;

            Tx.position = P - Tx.rotation * C;

            foreach (var shape in shapes)
            {
                broadphase.Update(shape.broadPhaseIndex, shape.ComputeAABB(Tx));
            }
        }

        internal void SetScene(Scene scene)
        {
            this.scene = scene;
            foreach (var shape in shapes)
            {
                shape.CheckHash();
            }
        }
        
        // -- Public Accessor

        public void AddShape(Shape shape)
        {
            if (shape.body != null) throw new InvalidOperationException();

            shapes.Add(shape);

            shape.body = this;

            if (scene != null) shape.CheckHash();

            flags |= BodyFlags.DirtyMass;

            scene.ContactManager.Broadphase.InsertShape(shape, shape.ComputeAABB(Tx));
        }

        public void RemoveShape(Shape shape)
        {
            if (shape.body != this) throw new InvalidOperationException();

            shape.RemoveHash();

            shapes.Remove(shape);

            // Remove all contacts associated with this shape
            for (int i = contactList.Count; i-- > 0;)
            {
                Contact contact = contactList[i].contact;

                if (shape == contact.A || shape == contact.B)
                    scene.ContactManager.RemoveContact(contact);
            }

            flags |= BodyFlags.DirtyMass;

            scene.ContactManager.Broadphase.RemoveShape(shape);

        }

        public void RemoveAllShapes()
        {
            for (int i = shapes.Count; i-- > 0;)
            {
                scene.ContactManager.Broadphase.RemoveShape(shapes[i]);
                shapes.RemoveAt(i);
            }

            scene.ContactManager.RemoveContactsFromBody(this);
        }

        public void ApplyForce(Vector4 force)
        {
            SetToAwake();

            this.force += force;
        }

        public void ApplyForceAtWorldPoint(Vector4 force, Vector4 point)
        {
            SetToAwake();

            this.force += force;
            this.torque += Euler4.Cross(point - P, force);

        }

        public void ApplyLinearImpulse(Vector4 impulse)
        {
            SetToAwake();

            this.linearVelocity += impulse * invMass;

        }

        public void ApplyLinearImpulseAtWorldPoint(Vector4 impulse, Vector4 point)
        {
            SetToAwake();

            linearVelocity += impulse * invMass;
            angularVelocity += invInertiaWorld * Euler4.Cross(point - P, impulse);

        }

        public void ApplyTorque(Euler4 torque)
        {
            SetToAwake();

            this.torque += torque;
        }

        public void SetToAwake()
        {
            if ((flags & BodyFlags.DirtyMass) > 0)
                CalculateMassData();
            if ((flags & BodyFlags.Awake) == 0)
            {
                flags |= BodyFlags.Awake;
                sleepTime = 0;
            }
        }

        public void SetToSleep()
        {
            if ((flags & BodyFlags.Awake) > 0)
            {
                flags &= ~BodyFlags.Awake;
                sleepTime = 0;
                linearVelocity = force = Vector4.zero;
                angularVelocity = torque = Euler4.zero;
            }
        }

        public bool IsAwake()
        {
            return (flags & BodyFlags.Awake) > 0;
        }

        public bool Active
        {
            get { return (flags & BodyFlags.Active) > 0; }
            set
            {
                if (!value)
                {
                    scene.ContactManager.RemoveContactsFromBody(this);
                    SetToSleep();
                    flags &= ~BodyFlags.Active;
                }
                else
                {
                    flags |= BodyFlags.Active;
                    SetToAwake();
                }
            }
        }

        public Vector4 LinearVelocity { get { return linearVelocity; } set { linearVelocity = value; if (Vector4.LengthSq(value) > 1e-4f) SetToAwake(); } }

        public Euler4 AngularVelocity { get { return angularVelocity; } set { angularVelocity = value; if (Euler4.LengthSq(value) > 1e-4f) SetToAwake(); } }

        public float LinearDamping { get { return linearDamping; } set { linearDamping = Math.Max(0, value); } }

        public float AngularDamping { get { return angularDamping; } set { angularDamping = Math.Max(0, value); } }

        public float GravityScale { get { return gravityScale; } set { gravityScale = value; } }

        public object Tag { get; set; }

        public static bool CanCollide(Body a, Body b)
        {
            // Can't collide self
            if (a == b)
                return false;

            // At least one of them must be dynamic
            if (((a.flags | b.flags) & BodyFlags.Dynamic) == 0)
                return false;

            // And in the same layer
            if ((a.layers & b.layers) == 0)
                return false;

            // And both is active
            if ((a.flags & b.flags & BodyFlags.Active) == 0)
                return false;

            return true;
        }

        public BodyFlags GetFlags() { return flags; }

        public void SetFlags(BodyType type, bool awake, bool allowSleep, bool active)
        {

            switch (type)
            {
                case BodyType.StaticBody: flags = BodyFlags.Static; break;
                case BodyType.DynamicBody: flags = BodyFlags.Dynamic; break;
                case BodyType.KinematicBody: flags = BodyFlags.Kinematic; break;
            }

            sleepTime = 0;
            flags |= awake ? BodyFlags.Awake : 0;
            flags |= allowSleep ? BodyFlags.AllowSleep : 0;
            flags |= active ? BodyFlags.Active : 0;
        } 

        public Matrix4x5 GetTransform() { return Tx; }

        public void GetTransform(Transform4 t) { t.position = P; t.rotation = Tx.rotation; }

        public void SetTransform(Vector4 position, Matrix4 rotation)
        {
            P = position;
            Tx.rotation = rotation;

            SynchronizeProxies();
        }

        public Body() { }

    }
}
