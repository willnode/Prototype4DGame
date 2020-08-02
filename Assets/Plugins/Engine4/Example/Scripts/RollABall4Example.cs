using UnityEngine;

namespace Engine4.Example
{
    public class RollABall4Example : MonoBehaviour4
    {
        public float strength = 1;
        public float looking = 1;
        Euler4 cameraAngles; 
        // Update is called once per frame
        void FixedUpdate()
        {
            rigidbody4.AddForce(viewer4.viewerToWorldMatrix.rotation *  new Vector4(
                Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"), 0
                ) * strength);
        }

        private void Update()
        {
            cameraAngles += new Euler4(0, Input.GetAxis("Mouse X") * 2, 0, 0, 0, Input.GetAxis("Mouse Y")) * looking;
            viewer4.transform4.position = transform4.position;
            viewer4.transform4.eulerAngles = cameraAngles;
        }
    }
}