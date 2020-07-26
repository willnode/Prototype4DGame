using UnityEditor;

namespace Engine4.Editing
{

    [CustomEditor(typeof(Renderer4)), CanEditMultipleObjects]
    public class Renderer4Editor : NoScriptEditor
    {

        //void OnEnable ()
        //{
        //    (target as Renderer4).GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
        //}

        //void OnDisable ()
        //{
        //    (target as Renderer4).GetComponent<MeshRenderer>().hideFlags = 0;
        //}
    }

}