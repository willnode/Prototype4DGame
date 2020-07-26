//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System;

//namespace Engine4.Editing {

//    //[CustomPropertyDrawer(typeof(Quaternion4))]
//    public class Quaternion4Editor : PropertyDrawer 
//    {
//        private Euler4 _euler = Euler4.zero;
//        private Quaternion4 _quaternion = Quaternion4.identity;

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//              var tr = property.GetValue<Quaternion4>();

//              if (_quaternion != tr)
//              {
//                    _quaternion = tr;
//                    _euler = tr.ToEuler();
//              }

//              if (!EditorGUIUtility.wideMode)
//                  EditorGUIUtility.labelWidth /= 2f;
//              position = EditorGUI.PrefixLabel(position, label);

//              EditorGUI.BeginProperty(position, label, property);
//              EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
//              EditorGUI.BeginChangeCheck();

//              PropertyEuler(position);

//            if (EditorGUI.EndChangeCheck())
//            {
//                _quaternion = Quaternion4.Euler(_euler);
////                Undo.RecordObjects(property.serializedObject.targetObjects, "Quaternion4 Change");
//                //property.SetStructValueAll(_quaternion);
//                //property.FindPropertyRelative("xyz").quaternionValue = _quaternion.x;
//                //property.FindPropertyRelative("tuv").floatValue = _quaternion.y;
//                //property.FindPropertyRelative("z").floatValue = _quaternion.z;
//                //property.FindPropertyRelative("t").floatValue = _quaternion.t;
//                //property.FindPropertyRelative("u").floatValue = _quaternion.u;
//                //property.FindPropertyRelative("v").floatValue = _quaternion.v;
//                //property.FindPropertyRelative("w").floatValue = _quaternion.w;
//                //property.FindPropertyRelative("s").floatValue = _quaternion.s;
//            }

//            EditorGUI.showMixedValue = false;
//             EditorGUI.EndProperty();
//            if (!EditorGUIUtility.wideMode)
//                  EditorGUIUtility.labelWidth *= 2f;
              
//        }

//        static float[] _values3 = new float[3];
//        static GUIContent[] _GUIxyz = new GUIContent[] {
//            new GUIContent("X", "YZ Plane"), new GUIContent("Y", "XZ Plane"), new GUIContent("Z", "XY Plane"),
//        };
//        static GUIContent[] _GUItuv = new GUIContent[] {
//            new GUIContent("T", "XW Plane"), new GUIContent("U", "YW Plane"), new GUIContent("V", "ZW Plane"),
//        };

//        void TidyDigits ()
//        {
//            _values3[0] = (float)Math.Round(_values3[0], 2);
//            _values3[1] = (float)Math.Round(_values3[1], 2);
//            _values3[2] = (float)Math.Round(_values3[2], 2);
//        }
//        void PropertyEuler (Rect pos)
//        {
//            pos.height = EditorGUIUtility.singleLineHeight;
//            {
//                _values3[0] = _euler[0];
//                _values3[1] = _euler[1];
//                _values3[2] = _euler[2];
//                TidyDigits();
//            }
//            EditorGUI.MultiFloatField(pos, _GUIxyz, _values3);
//            {
//                _euler[0] = _values3[0];
//                _euler[1] = _values3[1];
//                _euler[2] = _values3[2];
//            }
//            pos.y += pos.height;
//            {
//                _values3[0] = _euler[3];
//                _values3[1] = _euler[4];
//                _values3[2] = _euler[5];
//                TidyDigits();
//            }
//            EditorGUI.MultiFloatField(pos, _GUItuv, _values3);
//            {
//                _euler[3] = _values3[0];
//                _euler[4] = _values3[1];
//                _euler[5] = _values3[2];
//            }

//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return (2) * EditorGUIUtility.singleLineHeight;
//        }

//    }

//}