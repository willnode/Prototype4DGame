//--------------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------------------

using Engine4.Physics.Internal;
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

namespace Engine4.Physics
{
    /// Common constant for physics simulation
    public static class Common
    {

        /// <summary>
        /// Default gravity for all simulations.
        /// This parameter can be dynamically adjusted.
        /// </summary>
        public static Vector4 GRAVITY = new Vector4(0, -9.8f, 0, 0);

        /// <summary>
        /// Simulation range helps to avoid wasted time computation of computing
        /// Objects that out of range (e.g. object keeps falling down). 
        /// The object is deactivated once becomes so.
        /// </summary>
        public static readonly Bounds4 SIM_RANGE = new Bounds4(Vector4.one * -1000f, Vector4.one * 1000f);

        /// <summary>
        /// Constant collision solving iteration.
        /// Higher values are more realistic, yet more expensive.
        /// Default is 15, can be put between 5 to 20
        /// </summary>
        public const int ITERATIONS = 15;

        /// <summary>
        /// Enables or disables rigid body sleeping.
        /// It's an important optimization on every physics engine. 
        /// It's recommended to leave this on.
        /// </summary>
        public const bool ALLOW_SLEEP = true;

        /// <summary>
        /// When two objects lay on each other should the simulator simulate friction?
        /// It's recommended to leave this on.
        /// </summary>
        public const bool ENABLE_FRICTION = true;

        /// <summary>
        /// Constant maximum multi contact count. Don't change.
        /// </summary>
        public const int MULTICONTACT_COUNT = 16;

        /// <summary>
        /// Maximum allowed linear velocity that makes a rigidbody sleeps (Default is 0.01)
        /// </summary>
        public const float SLEEP_LINEAR = 0.01f;

        /// <summary>
        ///  Maximum allowed angular velocity that makes a rigidbody sleeps (Default is 2 deg)
        /// </summary>
        public const float SLEEP_ANGULAR = (2 / 180f) * Math.PI;

        /// <summary>
        /// Simulator don't make rigidbody sleeps instantly. 
        /// They have a chance by given time to awake by itself. (Default is 0.5)
        /// </summary>
        public const float SLEEP_TIME = 0.5f;

        /// <summary>
        /// How fast the collision must be resolved? (Default is 0.2)
        /// </summary>
        public const float BAUMGARTE = 0.2f;

        /// <summary>
        /// Offset of allowed contact depth before kicked out by the solver.
        /// Allow very small number to avoid jitter. (Default is 0.05)
        /// </summary>
        public const float PENETRATION_SLOP = 0.02f;

        /// <summary>
        /// Maximum allowed delta time. This prevents objects bypassing 
        /// each other because of too large delta time. (Default is 0.02)
        /// </summary>
        public const float MAX_DT = 0.02f;

        /// <summary>
        /// Restitution (bounce) mixing. Default is max
        /// </summary>
        public static float MixRestitution(Shape A, Shape B)
        {
            return A.restitution > B.restitution ? A.restitution : B.restitution;
        }

        /// <summary>
        /// Friction (slide) mixing. Default is average
        /// </summary>
        public static float MixFriction(Shape A, Shape B)
        {
            return (A.friction + B.friction) * 0.5f;
        }        
    }
}
