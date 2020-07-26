using System;
using UnityEditor;
using UnityEngine;
using Engine4.Internal;

namespace Engine4.Editing
{
    [CustomPropertyDrawer(typeof(Matrix4))]
    public class Matrix4Editor : PropertyDrawer
    {
        private Euler4 _euler = Euler4.zero;
        private Matrix4 _matrix = Matrix4.identity;

        bool IsToEuler ()
        {
            return fieldInfo.GetCustomAttributes(typeof(Matrix4AsEulerAttribute), false).Length > 0;

        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsToEuler())
            {
                EditorGUI.BeginProperty(position, label, property);
                position.height = 16;

                var child = property.FindPropertyRelative("ex");
                do
                {
                    EditorGUI.PropertyField(position, child, child.name == "ex" ? label : new GUIContent(" "));
                    position.y += 16;

                    if (child.name == "ew") break;
                } while (child.Next(false));

                EditorGUI.EndProperty();
                return;
            }

            var tr = property.GetValue<Matrix4>();

            if (_matrix != tr)
            {
                _matrix = tr;
                _euler = tr.ToEuler();
            }

            if (!EditorGUIUtility.wideMode)
                EditorGUIUtility.labelWidth /= 2f;
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();

            PropertyEuler(position);

            if (EditorGUI.EndChangeCheck())
            {
                var m = Matrix4.Euler(_euler);
                // Should there be a better way?
                property.FindPropertyRelative("ex.x").floatValue = m.ex.x;    
                property.FindPropertyRelative("ex.y").floatValue = m.ex.y;    
                property.FindPropertyRelative("ex.z").floatValue = m.ex.z;    
                property.FindPropertyRelative("ex.w").floatValue = m.ex.w;
                property.FindPropertyRelative("ey.x").floatValue = m.ey.x;
                property.FindPropertyRelative("ey.y").floatValue = m.ey.y;
                property.FindPropertyRelative("ey.z").floatValue = m.ey.z;
                property.FindPropertyRelative("ey.w").floatValue = m.ey.w;
                property.FindPropertyRelative("ez.x").floatValue = m.ez.x;
                property.FindPropertyRelative("ez.y").floatValue = m.ez.y;
                property.FindPropertyRelative("ez.z").floatValue = m.ez.z;
                property.FindPropertyRelative("ez.w").floatValue = m.ez.w;
                property.FindPropertyRelative("ew.x").floatValue = m.ew.x;
                property.FindPropertyRelative("ew.y").floatValue = m.ew.y;
                property.FindPropertyRelative("ew.z").floatValue = m.ew.z;
                property.FindPropertyRelative("ew.w").floatValue = m.ew.w;

            }

            EditorGUI.showMixedValue = false;
            EditorGUI.EndProperty();
            if (!EditorGUIUtility.wideMode)
                EditorGUIUtility.labelWidth *= 2f;

        }

        static float[] _values3 = new float[3];
        static GUIContent[] _GUIxyz = new GUIContent[] {
                    new GUIContent("X", "YZ Plane"), new GUIContent("Y", "XZ Plane"), new GUIContent("Z", "XY Plane"),
                };
        static GUIContent[] _GUItuv = new GUIContent[] {
                    new GUIContent("T", "XW Plane"), new GUIContent("U", "YW Plane"), new GUIContent("V", "ZW Plane"),
                };

        void TidyDigits()
        {
            _values3[0] = (float)Math.Round(_values3[0], 2);
            _values3[1] = (float)Math.Round(_values3[1], 2);
            _values3[2] = (float)Math.Round(_values3[2], 2);
        }
        void PropertyEuler(Rect pos)
        {
            pos.height = EditorGUIUtility.singleLineHeight;
            {
                _values3[0] = _euler[0];
                _values3[1] = _euler[1];
                _values3[2] = _euler[2];
                TidyDigits();
            }
            EditorGUI.MultiFloatField(pos, _GUIxyz, _values3);
            {
                _euler[0] = _values3[0];
                _euler[1] = _values3[1];
                _euler[2] = _values3[2];
            }
            pos.y += pos.height;
            {
                _values3[0] = _euler[3];
                _values3[1] = _euler[4];
                _values3[2] = _euler[5];
                TidyDigits();
            }
            EditorGUI.MultiFloatField(pos, _GUItuv, _values3);
            {
                _euler[3] = _values3[0];
                _euler[4] = _values3[1];
                _euler[5] = _values3[2];
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (IsToEuler() ? 2 : 4) * EditorGUIUtility.singleLineHeight;
        }

    }
}
