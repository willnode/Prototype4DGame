using UnityEngine;
using UnityEditor;
using Engine4.Internal;
using System.Linq;

namespace Engine4.Editing
{
    [CustomEditor(typeof(Transform4)), CanEditMultipleObjects]
    public partial class Transform4Editor : Editor
    {
        public enum Tool4
        {
            None = -1,
            Move = 0,
            Rotate = 1,
            Scale = 2,
        }

        static Color _themeSolid = new Color(0.4f, 0.1f, 0.6f, 0.2f);
        static Color _themeWire = new Color(0.4f, 0.1f, 0.6f, 1f);
        static Tool4 _tool = Tool4.None;
        static GUIStyle _toolStyle;
        static GUIContent[] _toolIcons;
        static GUIContent[] _toolIcons_2 = new GUIContent[2];
        static GUIContent[] _toolIcons_3 = new GUIContent[3];
        static bool _initStyle = false;

        void InitStyle()
        {
            _toolIcons = _toolIcons ?? new GUIContent[]
            {
                EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects."),
                EditorGUIUtility.IconContent("RotateTool", "|Rotate the selected objects."),
                EditorGUIUtility.IconContent("ScaleTool", "|Scale the selected objects."),
                EditorGUIUtility.IconContent("MoveTool On"),
                EditorGUIUtility.IconContent("RotateTool On"),
                EditorGUIUtility.IconContent("ScaleTool On"),
            };
            _toolStyle = new GUIStyle("Commandmid");
            _toolStyle.margin = new RectOffset();
            _toolStyle.padding = new RectOffset();
            _toolStyle.stretchHeight = _toolStyle.stretchWidth = true;
            _toolStyle.fixedWidth = _toolStyle.fixedHeight = 0;
            _toolStyle.overflow = new RectOffset();
            _initStyle = true;
        }

        bool CanScaling()
        {
            return m_CanScale;
        }

        void GUITools(Rect r, bool scale)
        {
            var n = (int)(Tools.current == Tool.None ? _tool : Tool4.None);
            var c = scale ? 3 : 2;
            var o = scale ? _toolIcons_3 : _toolIcons_2;
            for (int i = 0; i < c; i++)
            {
                o[i] = _toolIcons[n == i ? i + 3 : i];
            }
            n = GUI.SelectionGrid(r, n, o, 1, _toolStyle);
            if (n >= 0)
            {
                Tools.current = Tool.None;
                _tool = (Tool4)n;
            }
        }

        static void GUIKeys(Event ev)
        {
            if (!ev.isKey || !ev.shift)
                return;
            if (ev.keyCode == KeyCode.Q)
            {
                Tools.current = Tool.None;
                _tool = Tool4.None;
            }
            else if (ev.keyCode == KeyCode.W)
            {
                Tools.current = Tool.None;
                _tool = Tool4.Move;
            }
            else if (ev.keyCode == KeyCode.E)
            {
                Tools.current = Tool.None;
                _tool = Tool4.Rotate;
            }
            else if (ev.keyCode == KeyCode.R)
            {
                Tools.current = Tool.None;
                _tool = Tool4.Scale;
            }
            EditorPrefs.SetInt("Engine4_Trans4Tool", (int)_tool);
        }

        SerializedProperty m_Position;
        SerializedProperty m_Rotation;
        SerializedProperty m_Scale;
        bool m_CanScale;

        void OnEnable()
        {
            m_Position = serializedObject.FindProperty("m_Position");
            m_Rotation = serializedObject.FindProperty("m_Rotation");
            m_Scale = serializedObject.FindProperty("m_Scale");
            m_CanScale = (target as Transform4).GetComponents<MonoBehaviour>().Any((x) => x is INeedTransform4Scale);
            _tool = (Tool4)EditorPrefs.GetInt("Engine4_Trans4Tool", (int)_tool);
        }

        public override void OnInspectorGUI()
        {
            if (!_initStyle)
                InitStyle();

            var r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_Position);

                EditorGUILayout.PropertyField(m_Rotation);
                if (CanScaling())
                    EditorGUILayout.PropertyField(m_Scale);

                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.GetControlRect(GUILayout.Height(2));
            }
            EditorGUILayout.EndVertical();
            var r2 = EditorGUILayout.BeginVertical(GUILayout.MinWidth(40));
            {
                r2.height = r.height;
                GUITools(r2, CanScaling());
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        static Quaternion GetRotation3D(Matrix4 v)
        {
            return Quaternion.Euler(v.ToEuler().xyz);
        }

        static void OnSceneGUIHandler(Transform4 target)
        {
            if (Tools.current != Tool.None)
            {
                Handles.BeginGUI();

                var r = new Rect(10, Screen.height - 68, 400, 42);
                GUI.Label(r, new GUIContent("Using 3D Transformation tools for 4D objects. Be careful."), "SC ViewLabel");//.centeredGreyMiniLabel);

                Handles.EndGUI();
                return;
            }
            var t = target;
            var v = Viewer4.main;

            var p = v.worldToViewerMatrix * t.position;
            var n = Vector3.Normalize(Camera.current.transform.position - p);
            var s = HandleUtility.GetHandleSize(p) * 0.2f;
            var orient = GetRotation3D(v.worldToViewerMatrix.rotation * t.rotation * v.viewerToWorldMatrix.rotation);

            Handles.matrix = t.transform.localToWorldMatrix;

            // Give unique indicator
            Handles.color = _themeSolid;
            Handles.DrawSolidDisc(p, n, s);

            Handles.color = _themeWire;
            Handles.DrawWireDisc(p, n, s);

            switch (_tool)
            {
                case Tool4.Move:
                    HandlePosition(t, v, p, orient);
                    break;
                case Tool4.Rotate:
                    HandleRotation(t, v, p, orient);
                    break;
                case Tool4.Scale:
                    HandleScale(t, v, p, orient);
                    break;
            }
        }



        static void HandlePosition(Transform4 t, Viewer4 v, Vector3 p, Quaternion orient)
        {
            var v_ = Handles.PositionHandle(p, Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : orient);
            if (p != v_)
            {
                Undo.RecordObject(t, "Transform4 Position");
                t.Translate((UnityEngine.Vector4)(v_ - p), Space4.View);
            }
        }

        static void HandleRotation(Transform4 t, Viewer4 v, Vector3 p, Quaternion orient)
        {
            var v_ = Handles.RotationHandle(orient, p);
            if (orient != v_)
            {
                Undo.RecordObject(t, "Transform4 Rotation");
                t.Rotate(v_ * Quaternion.Inverse(orient), Space4.View);
            }
        }

        static bool IsScaledUniformly(Vector3 a, Vector3 b)
        {
            return a.x != b.x && a.y != b.y && a.z != b.z;
        }

        static void HandleScale(Transform4 t, Viewer4 v, Vector3 p, Quaternion orient)
        {
            var r = v.worldToViewerMatrix.rotation * t.localScale;
            var r_ = Handles.ScaleHandle(r, p, orient, HandleUtility.GetHandleSize(p));
            if (r != r_)
            {
                if (IsScaledUniformly(r, r_))
                    r = new Vector4(r_.x, r_.y, r_.z, r.w * r_.x / r.x);
                else
                    r = new Vector4(r_.x, r_.y, r_.z, r.w);
                Undo.RecordObject(t, "Transform4 Scale");
                t.localScale = v.viewerToWorldMatrix * r;
            }
        }
    }

}