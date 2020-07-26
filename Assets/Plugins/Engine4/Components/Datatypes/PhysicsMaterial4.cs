using Engine4.Physics;
using UnityEngine;

namespace Engine4
{
    /// <summary> Additional configuration asset for 4D physics simulation </summary>
    /// <remarks> This asset is to be attached with Collider4 components </remarks>
    [CreateAssetMenu(fileName = "4D Physics Material", menuName = "4D Physics Material")]
    public class PhysicsMaterial4 : ScriptableObject
    {

        /// <summary> Density (uniform mass) of the object </summary>
        public float density = 1;
        /// <summary> Bounceness (how much energy is preserved) ratio during impact </summary>
        public float bounce = 0.2f;
        /// <summary> Friction (how much energy is lost) ratio during contact </summary>
        public float friction = 0.4f;
        
        /// <summary> Default physics material that is used for physics simulations </summary>
        public static PhysicsMaterial4 global
        {
            get
            {
                return Physics4.main.defaultMaterial;
            }
        }
    }
}
