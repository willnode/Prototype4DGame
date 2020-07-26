using System;
using UnityEditor;
using UnityEngine;

namespace Engine4.Editing
{
    public partial class Transform4Editor
    {

        static int activeView
        {
            get
            {
                var r = Viewer4.main.transform4.rotation;
                var v = Array.IndexOf(Styles.viewModesMatrix, r);
                return v;
            }
            set
            {
                if (value >= 0)
                {
                    SetAnimation(Viewer4.main, Styles.viewModes[value]);
                }
            }
        }

        static void CenterView()
        {
            SetAnimation(Viewer4.main, Vector4.zero);
        }

        static Viewer4 _viewer;
        static Euler4 _fromR;
        static Euler4 _targetR;
        static Vector4 _fromP;
        static Vector4 _targetP;
        static double _time;

        static void SetAnimation(Viewer4 v, Euler4 t)
        {
            _fromR = v.transform4.rotation.ToEuler();
            _time = EditorApplication.timeSinceStartup;
            _targetR = t;
            _viewer = v;
            EditorApplication.update += TransformViewerR;
        }

        static void SetAnimation(Viewer4 v, Vector4 t)
        {
            _fromP = v.transform4.position;
            _time = EditorApplication.timeSinceStartup;
            _targetP = t;
            _viewer = v;
            EditorApplication.update += TransformViewerP;
        }

        static float InverseLerp(double a, double b, double t)
        {
            return (float)((t - a) / (b - a));
        }



        static void TransformViewerR()
        {
            var d = 1 - InverseLerp(_time, _time + 0.4, EditorApplication.timeSinceStartup);

            _viewer.transform4.rotation = Matrix4.Euler(Euler4.LerpAngle(_fromR, _targetR, 1 - d * d));

            if (d <= 0)
            {
                // Accuracy issue
                _viewer.transform4.rotation = Matrix4.Euler(_fromR);
                _viewer.transform4.rotation = Matrix4.Euler(_targetR);
                EditorApplication.update -= TransformViewerR;
            }
        }

        static void TransformViewerP()
        {
            var d = 1 - InverseLerp(_time, _time + 0.4, EditorApplication.timeSinceStartup);
            _viewer.transform4.position = Vector4.Lerp(_fromP, _targetP, 1 - d * d);

            if (d <= 0)
            {
                // Accuracy issue
                _viewer.transform4.position = _fromP;
                _viewer.transform4.position = _targetP;
                EditorApplication.update -= TransformViewerP;
            }
        }

        static float activeDistance
        {
            get
            {
                var r = Viewer4.main.transform4.rotation;
                var p = Viewer4.main.transform4.position;

                var w = (r * p).w;
                return w * w <= 1e-6f ? 0 : w;
            }
            set
            {
                var r = Viewer4.main.transform4.rotation;
                var p = Viewer4.main.transform4.position;
                p = (r * p);
                p.w = value;
                p = Matrix4.Transpose(r) * p;
                Viewer4.main.transform4.position = p;
            }
        }


        static void OnSceneGUINavigation()
        {           
            Handles.BeginGUI();
            var r = new Rect(Screen.width * 96 / Screen.dpi - 210, Screen.height * 96 / Screen.dpi - 68, 200, 42);
            GUI.Window(Styles.overlayHash, r, new GUI.WindowFunction(DrawOverlay), Styles.projectionNames[(int)Viewer4.main.projection]);
            Handles.EndGUI();
        }

        static void DrawOverlay(int id)
        {
            EditorGUILayout.BeginHorizontal();
            {
                var act = activeView % 4;
                var gui = Styles.viewsGUI[act + 1];
                var stl = Styles.viewsStyle;

                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < 4; i++)
                {
                    if (GUILayout.Toggle(act == i, gui[i], stl[i]))
                        act = i;
                }
                if (EditorGUI.EndChangeCheck())
                    activeView = act + (Event.current.shift ? 4 : 0);
            }

            {
                var dist = activeDistance;
                EditorGUIUtility.labelWidth = 16f;
                EditorGUI.BeginChangeCheck();
                dist = EditorGUILayout.FloatField(Styles.distGUI[activeView % 4 + 1], dist);
                if (EditorGUI.EndChangeCheck())
                    activeDistance = dist;
            }
            {
                EditorGUI.BeginDisabledGroup(Vector4.LengthSq(Viewer4.main.transform4.position) < 1e-4);
                if (GUILayout.Button("R", EditorStyles.miniButton))
                    CenterView();
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
