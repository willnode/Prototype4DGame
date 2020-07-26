using UnityEngine;
using Engine4.Internal;

namespace Engine4.Rendering
{
    /// <summary>
    /// Base class from custom 4D model creation.
    /// </summary>
    [RequireComponent(typeof(Renderer4)), ExecuteInEditMode]
    public abstract class Modeler4 : MonoBehaviour4
    {
        /// <summary>
        /// The starting method for generating model 
        /// </summary>
        public abstract void CreateModel(Buffer4 buffer);

        ///
        [HideInDocs] 
        protected virtual void OnEnable()
        {
            renderer4.ReacquireModelers();
        }


        ///
        [HideInDocs]
        protected virtual void OnDisable()
        {
            if (renderer4)
                renderer4.ReacquireModelers();
        }
        
        ///
        [HideInDocs]
        protected virtual void OnDestroy()
        {
            if (renderer4)
                renderer4.ReacquireModelers();
        }

        ///
        [HideInDocs]
        protected virtual void OnValidate()
        {
            renderer4.SetDirty(DirtyLevel.Model);
        }
    }

}