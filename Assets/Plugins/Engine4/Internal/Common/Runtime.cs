using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Engine4.Internal
{
    /// <summary>
    /// Utilities for handling Scene, Game objects, Unity-related task.
    /// </summary>
    public static class Runtime
    {

        /// <summary>
        /// Get or add a component
        /// </summary>
        public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T obj = gameObject.GetComponent<T>();
            if (obj)
                return obj;
            else
                return gameObject.AddComponent<T>();
        }


        /// <summary>
        /// Find a component starting from its parent
        /// </summary>
        public static T GetComponentFromParent<T>(Transform start) where T : class
        {
            while (start = start.parent)
            {
                if (start.GetComponent<T>() == null)
                    continue;
                return start.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// Copy component values to another
        /// </summary>
        public static void CopyComponent<T>(T source, T dest, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
        {
            var type = typeof(T);
            var fields = type.GetFields(flags);
            foreach (var f in fields)
            {
                f.SetValue(dest, f.GetValue(source));
            }
        }

        /// <summary>
        /// Safer way to destroy an object
        /// </summary>
        public static void Destroy(UnityEngine.Object obj)
        {
            if (!obj) return;
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(obj);
            else
                UnityEngine.Object.DestroyImmediate(obj);
        }

        /// <summary>
        /// Get either main camera or scene camera.
        /// </summary>
        /// <returns></returns>
        public static Transform GetCurrentCamera()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var scene = UnityEditor.SceneView.lastActiveSceneView;
                if (scene != null) return scene.camera.transform; else return null;
            }
            else
#endif
            {
                var camera = Camera.main;
                if (camera != null) return camera.transform; else return null;
            }
        }

        /// <summary>
        /// Create a new GameObject with hideFlags
        /// </summary>
        public static T CreateGameObject<T>(string name, HideFlags flags = HideFlags.None)
        {
            var obj = new GameObject(name);
            obj.hideFlags = flags;
            obj.AddComponent(typeof(T));

            return obj.GetComponent<T>();
        }

        /// <summary>
        /// Dump Mesh to Readable CSV format
        /// </summary>
        public static string Dump (Mesh m)
        {
            var l = new Buffer3(m);
            var s = new StringBuilder();
            s.AppendLine("Position\t\t\tColor\tUV0\t\t\t\tUV2\t\t\t\tUV3\t\t\t\t");
            s.AppendLine("X\tY\tZ\tHex\tX\tY\tZ\tW\tX\tY\tZ\tW\tX\tY\tZ\tW");
            for (int i = 0; i < l.m_Verts.Count; i++)
            {
                s.AppendLine(Dump(l.m_Verts, i) + "\t" + Dump(l.m_Colors, i) + "\t" + Dump(l.m_Uv0, i) + "\t" + Dump(l.m_Uv2, i) + "\t" + Dump(l.m_Uv3, i));
            }
            return s.ToString();
        }

        static string Dump(List<Color> arr, int i)
        {
            if (i < arr.Count)
            {
                return ColorUtility.ToHtmlStringRGBA(arr[i]);
            }
            return string.Empty;
        }

        static string Dump(List<Vector3> arr, int i)
        {
            if (i < arr.Count)
            {
                var v = arr[i];
                return v.x + "\t" + v.y + "\t" + v.z;
            }
            return "\t\t";
        }

        static string Dump (List<UnityEngine.Vector4> arr, int i)
        {
            if (i < arr.Count )
            {
                var v = arr[i];
                return v.x + "\t" + v.y + "\t" + v.z + "\t" + v.w;
            }
            return "\t\t\t";
        }

        /// <summary>
        /// Push 3D data to 4D
        /// </summary>
        static public void TransferTransform3DTo4D (GameObject g)
        {
            var t3 = g.transform;
            var t4 = GetOrAddComponent<Transform4>(g);
            t4.Translate((UnityEngine.Vector4)t3.position, Space4.View);
            t4.Rotate(t3.rotation, Space4.View);
            t3.position = Vector3.zero;
            t3.rotation = Quaternion.identity;
        }

    }
}
