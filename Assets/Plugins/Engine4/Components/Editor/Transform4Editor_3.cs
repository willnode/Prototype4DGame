using Engine4.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace Engine4.Editing
{
    [InitializeOnLoad]
    public partial class Transform4Editor
    {
        static class Styles
        {

            public static GUIContent[][] viewsGUI = new GUIContent[][] {
                new GUIContent[] { new GUIContent(" "), new GUIContent(" "), new GUIContent(" "), new GUIContent(" ")},
                new GUIContent[] { new GUIContent("W"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("X")},
                new GUIContent[] { new GUIContent("X"), new GUIContent("W"), new GUIContent("Z"), new GUIContent("Y")},
                new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("W"), new GUIContent("Z")},
                new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("W")},
            };

            public static GUIContent[] distGUI = new GUIContent[] { new GUIContent(" "), new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("W") };

            public static GUIStyle[] viewsStyle = new GUIStyle[] {
                new GUIStyle(EditorStyles.miniButtonLeft),
                new GUIStyle(EditorStyles.miniButtonMid),
                new GUIStyle(EditorStyles.miniButtonMid),
                new GUIStyle(EditorStyles.miniButtonRight),
            };

            public static Euler4[] viewModes = new Euler4[] {
                new Euler4(0, 0, 0, 90, 0, 0),
                new Euler4(0, 0, 0, 0, 90, 0),
                new Euler4(0, 0, 0, 0, 0, 90),
                new Euler4(0, 0, 0, 0, 0, 0),
                new Euler4(90, 0, 0, 0, 0, 0),
                new Euler4(0, 90, 0, 0, 0, 0),
                new Euler4(0, 0, 90, 0, 0, 0),
                new Euler4(0, 0, 0, 0, 0, 0),
            };

            public static Matrix4[] viewModesMatrix;

            public static int overlayHash = "Renderer4".GetHashCode();

            public static string[] projectionNames;

            static Styles()
            {
                var modes = Array.ConvertAll((ProjectionMode[])Enum.GetValues(typeof(ProjectionMode)), x => (int)x);
                projectionNames = new string[Mathf.Max(modes) + 1];
                for (int i = 0; i < projectionNames.Length; i++)
                {
                    projectionNames[i] = "Viewer4 " + Enum.GetName(typeof(ProjectionMode), i);
                }

                viewModesMatrix = Array.ConvertAll(viewModes, x => Matrix4.Euler(x));
            }
        }


        static Transform4Editor()
        {
            SceneView.duringSceneGui += OnStaticSceneGUI;
        }


        static void OnStaticSceneGUI(SceneView view)
        {
            var g = Selection.activeGameObject;
            if (g && g.GetComponent<Transform4>() && PrefabUtility.GetPrefabAssetType(g) == PrefabAssetType.NotAPrefab)
            {
                var t = g.GetComponent<Transform4>();
                GUIKeys(Event.current);
                OnSceneGUIHandler(t);
                OnSceneGUINavigation();
                OnPrefabDrop(t);
            }
        }

        static void OnPrefabDrop(Transform4 t)
        {
            if (Event.current.type == EventType.DragExited)
            {
                //PrefabUtility.SetPropertyModifications(t, new PropertyModification[]
                //{
                //    new PropertyModification() { propertyPath = "m_Position" },
                //    new PropertyModification() { propertyPath = "m_Rotation" }
                //});
                //PrefabUtility.RecordPrefabInstancePropertyModifications(t);
               // Undo.RecordObject(t, "Place " + t.name);

                EditorApplication.delayCall += delegate(){
                    Undo.RecordObject(t, "Place " + t.name);
                    Runtime.TransferTransform3DTo4D(t.gameObject);
                };
                //t.position = Vector4.forward;
                //Debug.Log(t.gameObject.transform.position);
                //Event.current.Use();
            }
        }
    }
}
