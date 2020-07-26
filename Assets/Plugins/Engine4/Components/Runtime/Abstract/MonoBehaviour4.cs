using Engine4.Internal;
using Engine4.Physics;
using System;
using UnityEngine;

namespace Engine4
{
    /// <summary>
    /// Base class to get access with main Engine4 components
    /// </summary>
    [RequireComponent(typeof(Transform4))]
    public class MonoBehaviour4 : MonoBehaviour
    {

        [NonSerialized]
        Transform4 _t4;

        [NonSerialized]
        Renderer4 _r4;
        
        [NonSerialized]
        Rigidbody4 _g4;

        /// <summary>
        /// Direct access to Transform4
        /// </summary>
        public Transform4 transform4
        {
            get { return _t4 ? _t4 : (_t4 = GetComponent<Transform4>()); }
        }

        /// <summary>
        /// Direct access to Renderer4
        /// </summary>
        public Renderer4 renderer4
        {
            get { return _r4 ? _r4 : (_r4 = GetComponent<Renderer4>()); }
        }

        /// <summary>
        /// Direct access to Rigidbody4
        /// </summary>
        public Rigidbody4 rigidbody4
        {
            get { return _g4 ? _g4 : (_g4 = GetComponent<Rigidbody4>() ?? GetComponentInParent<Rigidbody4>()); }
        }

        /// <summary>
        /// Direct access to Viewer4
        /// </summary>
        public static Viewer4 viewer4
        {
            get { return Viewer4.main; }
        }

        /// <summary>
        /// Direct access to Physics4
        /// </summary>
        public static Physics4 physics4
        {
            get { return Physics4.main; }
        }

        /// 
        [HideInDocs]
        protected virtual void OnTransformParentChanged() { _g4 = null; } // Invalidate cache
        
        /// <summary>
        /// Instantiate with given four-dimensional position and rotation
        /// </summary>
        public static GameObject Instantiate (GameObject g, Vector4 position, Matrix4 rotation)
        {
            var obj = Instantiate(g);
            Transform4 t;
            if (t = Runtime.GetOrAddComponent<Transform4>(g))
            {
                t.position = position;
                t.rotation = rotation;
            }
            return obj;
        }
    }

}
