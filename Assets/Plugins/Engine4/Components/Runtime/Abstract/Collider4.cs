using System;
using UnityEngine;
using Engine4.Rendering;
using Engine4.Physics.Internal;
using Engine4.Internal;

namespace Engine4.Physics
{
    /// <summary>
    /// Base class for all collider in Engine4
    /// </summary>
    /// <remarks>
    /// Unlike Unity, all colliders must have a rigidbody in the same gameobject or its parent.
    /// Otherwise, a static rigidbody is automatically generated when the game start.
    /// </remarks>
    public abstract class Collider4 : MonoBehaviour4, INeedTransform4Scale
    {
        [SerializeField]
        PhysicsMaterial4 m_material;
        [SerializeField]
        bool m_sensor = false;

        /// <summary>
        /// Physics material used during simulation
        /// </summary>
        public PhysicsMaterial4 material
        {
            get { return m_material; }
            set { if (m_material != value) { m_material = value; Synchronize(); } }
        }

        /// <summary>
        /// Determine if this collider ignores collision impacts (aka. trigger collider)
        /// </summary>
        public bool sensor
        {
            get { return m_sensor; }
            set { if (m_sensor != value) { m_sensor = value; Synchronize(); } }
        }

        /// <summary>
        /// Register collision callback to this shape
        /// </summary>
        public CollisionCallback callback
        {
            get { return physics4.collisionCallbacks.GetValue(shape.hash, null); }
            set { physics4.collisionCallbacks[shape.hash] = value; }
        }

        [NonSerialized]
        Shape _shape;

        internal Shape shape { get { if (_shape == null) Synchronize(); return _shape; } }

        // Use this for initialization

        internal virtual void Synchronize()
        {
            if (!rigidbody4)
            {
                // Add a rigidbody (note that Rigidbody4 can be in the parent object)
                _shape = Shape.CreateShape(type);
                var g = gameObject.AddComponent<Rigidbody4>();
                g.body.AddShape(_shape);
                g.type = BodyType.StaticBody;
            }

            if (_shape == null)
            {
                _shape = Shape.CreateShape(type);
                rigidbody4.body.AddShape(_shape);
            }
            {
                var material = this.m_material ? this.m_material : Physics4.main.defaultMaterial;
                _shape.Local = rigidbody4.transform4.worldToLocalMatrix * transform4.localToWorldMatrix;
                _shape.Restitution = material.bounce;
                _shape.Friction = material.friction;
                _shape.Density = material.density;
                _shape.Sensor = m_sensor;
                _shape.Tag = this;
            }
        }

        void Start() 
        { 
            Synchronize(); 
        }

        void OnValidate() 
        { 
            if (Application.isPlaying && rigidbody4 && physics4) 
                Synchronize(); 
        }

        void OnEnable ()
        {
            transform4.update += OnTransformChanged;
        }

        void OnDisable()
        {
            transform4.update -= OnTransformChanged;
        }

        void OnTransformChanged()
        {
            if (rigidbody4 && !rigidbody4._suppressTransform)
                Synchronize(); // Transform has been changed manually
        }

        internal abstract ShapeType type { get; }

        /// 
        [HideInDocs]
        protected virtual void OnDrawGizmosSelected ()
        {
            // Initialize
            Gizmos4.color = isActiveAndEnabled ? (rigidbody4 && rigidbody4.type != BodyType.StaticBody ? dynamicGizmoColor : colliderGizmoColor) : colliderGizmoColorDisabled;
            Gizmos4.matrix = transform4.localToWorldMatrix;
        }

        /// 
        [HideInDocs]
        protected static readonly Color colliderGizmoColor = new Color(0.5f, 0.8f, 0.475f, 1);
        /// 
        [HideInDocs]
        protected static readonly Color dynamicGizmoColor = new Color(0.2f, 0.475f, 0.9f, 1);
        /// 
        [HideInDocs]
        protected static readonly Color colliderGizmoColorDisabled = new Color(0.5f, 0.8f, 0.475f, 0.4f);

    }





}
