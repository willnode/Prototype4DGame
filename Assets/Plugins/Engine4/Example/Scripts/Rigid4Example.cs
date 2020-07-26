using UnityEngine;

namespace Engine4.Example
{
    public class Rigid4Example : MonoBehaviour4
    {
        public GameObject prefab;
        public Transform4 spawn;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(prefab, spawn.position, spawn.rotation);
            }
        }
    }
}
