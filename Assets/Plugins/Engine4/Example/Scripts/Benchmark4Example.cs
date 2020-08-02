using Engine4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Engine4.Example
{
    public class Benchmark4Example : MonoBehaviour4
    {

        public GameObject obj;
        public int count;
        public bool variable;
        public Text fps;

        int frames;
        float captured;

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(obj, Vector4.forward * i, Matrix4.identity);
            }
        }

        // Update is called once per frame
        void Update()
        {
            frames++;
            if (frames % 5 == 0)
            {
               var delta = (Time.time - captured) / 5f;
                fps.text = "MS: " + (delta * 1000).ToString("0.00") + " FPS: " + (1f / delta).ToString("0.0");
                captured = Time.time;
            }
            viewer4.transform4.Rotate(Matrix4.Euler(0, variable ? 90 * Time.deltaTime : 2), Space4.Self);
        }
    }
}