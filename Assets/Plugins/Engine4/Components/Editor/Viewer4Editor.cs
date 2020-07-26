using UnityEditor;

namespace Engine4.Editing
{

    [CustomEditor(typeof(Viewer4)), CanEditMultipleObjects]
    public class Viewer4Editor : Editor {

        SerializedProperty projection;
        SerializedProperty background;

        void OnEnable()
        {
            projection = serializedObject.FindProperty("projection");
            background = serializedObject.FindProperty("background");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(projection);
            EditorGUILayout.PropertyField(background);
            if (background.boolValue)
            {
#if UNITY_WEBGL
                EditorGUILayout.HelpBox("WebGL does not support multithreading", MessageType.Warning);
#else
                EditorGUILayout.HelpBox("Background threads helps improve performance but still considered at experimental state.", MessageType.Info);
#endif
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                foreach (var item in targets)
                    ((Viewer4)item).Validate();
            }
        }
    }

}
