using Engine4.Internal;
using UnityEngine;

namespace Engine4.Rendering
{
    /// <summary>
    /// Base class from custom 4D model modifier.
    /// </summary>
    [RequireComponent(typeof(Renderer4)), ExecuteInEditMode]
    public abstract class Modifier4 : MonoBehaviour4
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract void ModifyModel(Buffer4 buffer);

        ///
        [HideInDocs]
        protected virtual void OnEnable()
        {
            renderer4.ReacquireModifiers();
        }


        ///
        [HideInDocs]
        protected virtual void OnDisable()
        {
            if (renderer4)
                renderer4.ReacquireModifiers();
        }

        ///
        [HideInDocs]
        protected virtual void OnDestroy()
        {
            if (renderer4)
                renderer4.ReacquireModifiers();
        }

        ///
        [HideInDocs]
        protected virtual void OnValidate()
        {
            renderer4.SetDirty(DirtyLevel.Modifier);
        }
    }
}
