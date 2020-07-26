using UnityEditor;
using Engine4.Rendering;

namespace Engine4.Editing
{

    public abstract class NoScriptEditor : Editor
    {

        static string[] _excludes = { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _excludes);
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(Modeler4), true), CanEditMultipleObjects]
    public class Uploader4Editor : NoScriptEditor { }
    
}