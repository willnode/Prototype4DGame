using Engine4.Physics;
using UnityEngine;

namespace Engine4.Example
{
    public class BoxCannon4Example : MonoBehaviour4
    {
        public GameObject prefab;
        public float impulse = 20;
   
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var g = Instantiate(prefab, transform4.position, transform4.rotation);
                g.GetComponent<Rigidbody4>().AddImpulse(transform4.overward * impulse);
            }
        }
    }
}
