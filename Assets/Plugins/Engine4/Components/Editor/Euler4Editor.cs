using UnityEngine;
using UnityEditor;

namespace Engine4.Editing
{

    [CustomPropertyDrawer(typeof(Euler4))]
    public class Euler4Editor : PropertyDrawer
    {
        static GUIContent[] labels = new GUIContent[]
                {
                new GUIContent("X"),
                new GUIContent("Y"),
                new GUIContent("Z"),
                new GUIContent("T"),
                new GUIContent("U"),
                new GUIContent("V"),
                };

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return 32;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            label = EditorGUI.BeginProperty(position, label, property);
            if (!EditorGUIUtility.wideMode)
                EditorGUIUtility.labelWidth /= 2f;
            Rect r = EditorGUI.PrefixLabel(position, label);
            r.height = 16;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 14f;

            SerializedProperty x = property.FindPropertyRelative("x");
            SerializedProperty y = property.FindPropertyRelative("y");
            SerializedProperty z = property.FindPropertyRelative("z");
            SerializedProperty t = property.FindPropertyRelative("t");
            SerializedProperty u = property.FindPropertyRelative("u");
            SerializedProperty v = property.FindPropertyRelative("v");

            Rect r2 = r;
            r2.width /= 3f;
            PropField(r2, x, labels[0]);
            r2.x += r2.width;
            PropField(r2, y, labels[1]);
            r2.x += r2.width;
            PropField(r2, z, labels[2]);
            r2 = r;
            r2.width /= 3f;
            r2.y += 16;
            PropField(r2, t, labels[3]);
            r2.x += r2.width;
            PropField(r2, u, labels[4]);
            r2.x += r2.width;
            PropField(r2, v, labels[5]);

            EditorGUI.EndProperty();
        }

        void PropField(Rect r, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.PropertyField(r, prop, label);
            //return;
            //}
            //EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            //EditorGUI.BeginChangeCheck();
            //var v = EditorGUI.FloatField(r, label, prop.floatValue * Mathf.Rad2Deg);
            //if (EditorGUI.EndChangeCheck()) {
            //    prop.floatValue =  v * Mathf.Deg2Rad;
            //}
            //EditorGUI.showMixedValue = false;

        }

    }

}