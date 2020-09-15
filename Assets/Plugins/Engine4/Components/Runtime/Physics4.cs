using System;
using System.Collections.Generic;
using UnityEngine;
using Engine4.Physics.Internal;
using Engine4.Internal;
using System.ComponentModel;

namespace Engine4.Physics
{
    /// <summary>
    /// The head of the physics simulation.
    /// </summary>
    public class Physics4 : MonoBehaviour4, IContactListener
    {
        [NonSerialized]
        internal Scene scene = new Scene();

        /// <summary>
        /// Default material fallback for the scene.
        /// </summary>
        public PhysicsMaterial4 defaultMaterial;

        public float timeScale = 1f;

        void Start()
        {
            if (Application.isPlaying && main != this) Destroy(this); // multiscenario fix
        }

        void Reset() 
        {
            defaultMaterial = Resources.Load<PhysicsMaterial4>("Default-PhysicsMaterial4");
        }

        void OnEnable()
        {
            if (defaultMaterial == null)
                (defaultMaterial = ScriptableObject.CreateInstance<PhysicsMaterial4>()).hideFlags = HideFlags.DontSave;
            scene.ContactManager.ContactListener = this;
        }

        void FixedUpdate()
        {
            if (Time.deltaTime > 1e-4)
                scene.Step(Time.deltaTime * timeScale);

            if (onStayCallbacks.Count > 0)
            {
                var hit = new CollisionHit4(CollisionState.Stay);
                for (int i = onStayCallbacks.Count; i-- > 0;)
                {
                    collisionCallbacks[onStayCallbacks[i]](hit);
                }
            }
        }

        static Physics4 _main;

        /// <summary>
        /// Main physics manager
        /// </summary>
        public static Physics4 main
        {
            get
            {
                return _main ? _main : ((_main = Viewer4.main.GetComponent<Physics4>())
                  ? _main : (_main = Viewer4.main.gameObject.AddComponent<Physics4>()));
            }
        }

        /// <summary>
        /// Database of delegates to collision callbacks
        /// </summary>
        /// <remarks>
        /// Attach at Awake() or OnEnable()/OnDisable() 
        /// </remarks>
        [NonSerialized]
        public Dictionary<int, CollisionCallback> collisionCallbacks = new Dictionary<int, CollisionCallback>();
        [NonSerialized]
        Set<int> onStayCallbacks = new Set<int>();

        /// <summary>
        /// Globally cast raycast and return report
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static RaycastHit4 Raycast(Ray4 ray)
        {
            RaycastHit4 hit = new RaycastHit4(ray);
            return main.scene.QueryRaycast(hit);
        }

        /// 
        [HideInDocs, EditorBrowsable(EditorBrowsableState.Never)]
        public void BeginContact(Contact contact)
        {
            int a = contact.A.hash, b = contact.B.hash; CollisionCallback CA, CB;
            if (collisionCallbacks.ContainsKey(a) && (CA = collisionCallbacks[a]) != null)
            {
                CA(new CollisionHit4(CollisionState.Enter, contact, true));
                onStayCallbacks.Add(a);
            }
            if (collisionCallbacks.ContainsKey(b) && (CB = collisionCallbacks[b]) != null)
            {
                CB(new CollisionHit4(CollisionState.Enter, contact, false));
                onStayCallbacks.Add(b);
            }
        }

        /// 
        [HideInDocs, EditorBrowsable(EditorBrowsableState.Never)]
        public void EndContact(Contact contact)
        {
            int a = contact.A.hash, b = contact.B.hash; CollisionCallback CA, CB;
            if (collisionCallbacks.ContainsKey(a) && (CA = collisionCallbacks[a]) != null)
            {
                CA(new CollisionHit4(CollisionState.Exit, contact, true));
                onStayCallbacks.Remove(a);
            }
            if (collisionCallbacks.ContainsKey(b) && (CB = collisionCallbacks[b]) != null)
            {
                CB(new CollisionHit4(CollisionState.Exit, contact, false));
                onStayCallbacks.Remove(b);
            }
        }

        public bool CheckStaying(Contact contact)
        {
            return onStayCallbacks.Contains(contact.A.hash) || onStayCallbacks.Contains(contact.B.hash);
        }
    }

}
