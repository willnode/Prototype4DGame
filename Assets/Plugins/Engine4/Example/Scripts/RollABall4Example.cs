using UnityEngine;

namespace Engine4.Example
{
    public class RollABall4Example : MonoBehaviour4
    {
        public float strength = 1;
        // Update is called once per frame
        void FixedUpdate()
        {
            rigidbody4.AddForce(viewer4.viewerToWorldMatrix.rotation *  new Vector4(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical"),
                Input.GetAxis("Mouse ScrollWheel")
                ) * strength) ;
        }
    }
}