using Engine4;
using UnityEngine;
using UnityEditor;
using Engine4.Editing;
using Engine4.Physics;

[CustomEditor(typeof(Rigidbody4))]
public class Rigidbody4Editor : NoScriptEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying && !serializedObject.isEditingMultipleObjects)
        {
            EditorGUILayout.HelpBox(GetInfoString(target as Rigidbody4), MessageType.None);
        }

    }


    void OnEnable ()
    {
        if (Application.isPlaying)
            EditorApplication.update += InspectorUpdate;
    }

    void OnDisable()
    {
        if (Application.isPlaying)
            EditorApplication.update -= InspectorUpdate;
    }

    void InspectorUpdate()
    {
        if (Time.frameCount % 10 == 0)
        {
            Repaint();
        }
    }


    string GetInfoString (Rigidbody4 r)
    {
        if (r.type == BodyType.StaticBody) return "Static objects don't move";
        return r.ToString(true);
    }
}
