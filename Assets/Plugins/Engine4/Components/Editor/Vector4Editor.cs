using UnityEngine;
using UnityEditor;

namespace Engine4.Editing
{

    [CustomPropertyDrawer(typeof(Vector4))]
    public class Vector4Editor : PropertyDrawer
    {
        GUIContent[] labels;
        float[] values;
        bool hasInit = false;

        void OnEnable()
        {
            labels = new GUIContent[]
                {
                new GUIContent("X"),
                new GUIContent("Y"),
                new GUIContent("Z"),
                new GUIContent("W")
                };
            values = new float[4];
            hasInit = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!hasInit)
                OnEnable();
            label = EditorGUI.BeginProperty(position, label, property);
            if (!EditorGUIUtility.wideMode)
                EditorGUIUtility.labelWidth /= 2f;
            Rect r = EditorGUI.PrefixLabel(position, label);
            EditorGUIUtility.labelWidth = 14f;

            SerializedProperty x = property.FindPropertyRelative("x");
            SerializedProperty y = property.FindPropertyRelative("y");
            SerializedProperty z = property.FindPropertyRelative("z");
            SerializedProperty w = property.FindPropertyRelative("w");

            values[0] = x.floatValue;
            values[1] = y.floatValue;
            values[2] = z.floatValue;
            values[3] = w.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(r, labels, values);
            if (EditorGUI.EndChangeCheck())
            {
                //Undo.RecordObject(property, "Vector4 Edit");
                x.floatValue = values[0];
                y.floatValue = values[1];
                z.floatValue = values[2];
                w.floatValue = values[3];
            }
            EditorGUI.EndProperty();
        }

    }

}