using UnityEngine;

namespace Engine4.Physics
{
    /// <summary>
    /// Constant Force
    /// </summary>
    [RequireComponent(typeof(Rigidbody4))]
    public class ConstantForce4 : MonoBehaviour4
    {
        /// <summary>
        /// Linear force to be applied each frame
        /// </summary>
        public Vector4 linearForce;
        /// <summary>
        /// Angular force to be applied each frame
        /// </summary>
        public Euler4 angularForce;

        void FixedUpdate()
        {
            
            if (rigidbody4.enabled)
            {
                rigidbody4.body.ApplyForce(linearForce);
                rigidbody4.body.ApplyTorque(angularForce);
            }
        }
    }
}
