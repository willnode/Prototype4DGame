using System.Collections;
using UnityEngine;

namespace Engine4.Example
{
    public class CameraRotation4Example : MonoBehaviour4
    {


        // Update is called once per frame
        void Update()
        {
             if (Input.GetKeyDown(KeyCode.PageUp))
                StartCoroutine(Navigate(new Euler4(5, 90)));
            else if (Input.GetKeyDown(KeyCode.PageDown))
                StartCoroutine(Navigate(new Euler4(5, 270)));

        }

        IEnumerator Navigate(Euler4 target)
        {
            var cur = viewer4.transform4.eulerAngles;
            target += cur; // It's okay because we operate under only one plane rotation.
            for (int i = 0; i < 15; i++)
            {
                var p = 1 - i / 15f;
                viewer4.transform4.eulerAngles = Euler4.LerpAngle(cur, target, 1 - (p * p));
                yield return null;
            }
        }
    }
}
