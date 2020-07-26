using Engine4.Physics.Internal;
using System;
using UnityEngine;

namespace Engine4.Physics
{
    /// <summary>
    /// Rigidbody makes the object is participating in the physics simulations.
    /// </summary>
    /// <remarks>
    /// It's generally not a good idea if you put the same rigidbody in the same object,
    /// or in the parent, as both will change transformation together and causing deadlocks.
    /// </remarks>
    public class Rigidbody4 : MonoBehaviour4
    {

        [SerializeField]
        BodyType m_type = BodyType.DynamicBody;
        [SerializeField]
        float m_linearDamping = 0.1f;
        [SerializeField]
        float m_angularDamping = 0.1f;
        [SerializeField]
        float m_gravityScale = 1;
        [NonSerialized]
        internal Body _body;
        [NonSerialized]
        internal bool _suppressTransform = false;

        /// <summary> Rigidbody type during simulation </summary>
        public BodyType type { get { return m_type; } set { if (value != m_type) { m_type = value; Synchronize(); } } }

        /// <summary> Angular damping of the body </summary>
        public float angularDamping { get { return m_angularDamping; } set { if (value != m_angularDamping) { body.angularDamping = m_angularDamping = value; } } }

        /// <summary> Linear damping of the body </summary>
        public float linearDamping { get { return m_linearDamping; } set { if (value != m_linearDamping) { body.linearDamping = m_linearDamping = value; } } }

        /// <summary> Amount of gravity that affects the body </summary>
        public float gravityScale { get { return m_gravityScale; } set { if (value != m_gravityScale) { body.gravityScale = m_gravityScale = value; } } }

        internal Body body
        {
            get { if (_body == null) Synchronize(); return _body; }
        }

        void OnEnable()
        {
            Synchronize();
            transform4.update += OnTransformChanged;
        }


        void OnDisable()
        {
            transform4.update -= OnTransformChanged;
        }

        /// <summary>
        /// Internally update parameters to the physics engine.
        /// </summary>
        internal void Synchronize()
        {
            if (_body == null)
            {
                Physics4.main.scene.CreateBody(_body = new Body());
            }
            {
                _body.LinearDamping = m_linearDamping;
                _body.AngularDamping = m_angularDamping;
                _body.GravityScale = m_gravityScale;
                _body.SetTransform(transform4.position, transform4.rotation);
                _body.SetFlags(type, true, true, enabled);
                _body.Tag = this;
            }
            {
                // Get colliders now
                foreach (var c in GetComponentsInChildren<Collider4>())
                {
                    c.Synchronize();
                }
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Physics4.main.enabled)
            {
                _suppressTransform = true;
                _body.GetTransform(transform4);
                _suppressTransform = false;

                if ((_body.flags & BodyFlags.Active) == 0)
                    enabled = false;
            }
        }

        void OnTransformChanged()
        {
            if (!_suppressTransform) Synchronize();
        }

        /// <summary> Add a force </summary>
        /// <remarks> A force is affected by delta time and mass and it is generally accumulated over time. </remarks>
        public void AddForce(Vector4 force)
        {
            body.ApplyForce(force);
        }

        /// <summary> Add an impulse </summary>
        /// <remarks> An impulse is not affected by delta time but mass and it is generally applied once. </remarks>
        public void AddImpulse(Vector4 impulse)
        {
            body.ApplyLinearImpulse(impulse);
        }

        /// <summary>
        /// Add force at position
        /// </summary>
        public void AddForceAtPosition(Vector4 force, Vector4 point)
        {
            if (isActiveAndEnabled) body.ApplyForceAtWorldPoint(force, point);
        }

        /// <summary>
        /// Add torque (force in terms of rotation)
        /// </summary>
        public void AddTorque(Euler4 torque)
        {
            if (isActiveAndEnabled) body.ApplyTorque(torque);
        }

        /// <summary>
        /// Linear velocity of this rigidbody (world unit/sec)
        /// </summary>
        public Vector4 linearVelocity { get { return body.LinearVelocity; } set { body.LinearVelocity = value; } }

        /// <summary>
        /// Angular velocity of this rigidbody (radian/sec)
        /// </summary>
        public Euler4 angularVelocity { get { return body.AngularVelocity; } set { body.AngularVelocity = value; } }

        /// <summary>
        /// Set rigidbody to sleep
        /// </summary>
        public void Sleep() { body.SetToSleep(); }

        /// <summary>
        /// Wake up rigidbody if it sleeping
        /// </summary>
        public void WakeUp() { body.SetToAwake(); }

        /// <summary>
        /// Dump state to string for debugging ease.
        /// </summary>
        public string ToString(bool report)
        {
            if (!report) return ToString();

            return string.Format("\nLinear\t: {0}\nAngular\t: {1}\nState\t: {2}\nCumulated Mass: {3}"
            , body.linearVelocity, body.angularVelocity, body.GetFlags(), body.mass);
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            // view contacts realtime
            var contacts = body.contactList;
            for (int i = 0; i < contacts.Count; i++)
            {
                var c = contacts[i].contact.manifold;
                var inv = c.B == body.shapes[0];
                for (int j = 0; j < c.contacts; j++)
                {
                    Gizmos.DrawRay(viewer4.projector.Project(c.position[j]), (inv ? -c.normal : c.normal) * 0.4f);
                }
            }
        }

    }

}
