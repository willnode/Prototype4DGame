using UnityEngine;
using Engine4.Internal;
using Engine4.Rendering;

namespace Engine4
{
    /// <summary>
    /// The camera of four-dimensional scene
    /// </summary>
    /// <remarks>
    /// A game engine needs a camera to display the scene output.
    /// Viewer is the camera. However, the output belongs to individual renderer, 
    /// Hence only one viewer is allowed in scene.
    /// </remarks>
    [ExecuteInEditMode]
    public class Viewer4 : MonoBehaviour4
    {
        /// <summary>
        /// Projection mode used by the viewer
        /// </summary>
        /// <remarks>
        /// Changes to this variable requires a call to <see cref="Validate"/> to take effect.
        /// </remarks>
        public ProjectionMode projection;

        /// <summary>
        /// Should renderers do the computation in separate thread? (experimental)
        /// </summary>
        /// <remarks>
        /// Changes to this variable requires a call to <see cref="Validate"/> to take effect.
        /// </remarks>
        public bool background = false;

        Projector4 _projector;

        ProjectorJob _job;

        /// <summary>
        /// Get access to internal projector
        /// </summary>
        public Projector4 projector { get { if (_projector == null) Validate(); return _projector; } }

        /// <summary>
        /// Get access to internal multithreaded projector
        /// </summary>
        public ProjectorJob projectorJob { get { if (_job == null && background) Validate(); return _job; } }

        static Viewer4 _main;

        void Start()
        {
            if (Application.isPlaying && main != this) Destroy(this); // multiscenario fix
        }

        void OnEnable()
        {
            Validate();
            transform4.update += OnTransformChanged;
        }

        void OnDisable()
        {
            transform4.update -= OnTransformChanged;
        }

        public void Validate()
        {
            switch (projection)
            {
                case ProjectionMode.CrossSection:
                    _projector = Runtime.GetOrAddComponent<CrossSection4>(gameObject);
                    break;
                case ProjectionMode.Frustum:
                    _projector = Runtime.GetOrAddComponent<Frustum4>(gameObject);
                    break;
            }

            if (!_job && background)
            {
                _job = GetComponentInChildren<ProjectorJob>();
                if (!_job)
                {
                    var g = new GameObject("Viewer4 Jobs", typeof(ProjectorJob));
                    g.transform.SetParent(transform, false);
                    _job = g.GetComponent<ProjectorJob>();
                }
            }

            if (_job)
            {
                _job.enabled = background;

                switch (projection)
                {
                    case ProjectionMode.CrossSection:
                        _job.SyncProjector((CrossSection4)_projector);
                        break;
                    case ProjectionMode.Frustum:
                        _job.SyncProjector((Frustum4)_projector);
                        break;
                }
            }

            SetDirty(DirtyLevel.Model);
        }

        void OnTransformChanged()
        {
            SetDirty(DirtyLevel.Transform);
        }

        /// <summary>
        /// Main access to the Viewer4
        /// </summary>
        public static Viewer4 main
        {
            get
            {
                return _main ? _main : ((_main = FindObjectOfType<Viewer4>())
                  ? _main : (_main = Runtime.CreateGameObject<Viewer4>("Main Viewer")));
            }
        }

        /// <summary>
        /// Force a global invalidation
        /// </summary>
        public void SetDirty(DirtyLevel level)
        {
            projector.Setup(viewerToWorldMatrix);
            if (_job)
                _job.SyncProjector(_projector);

            if (update != null)
                update(level);
        }

        /// <summary>
        /// Event to be called when something updated in viewer.
        /// By default it is filled by Viewer4
        /// </summary>
        public static ViewerDispatch update = null;

        /// <summary>
        /// Camera to world matrix transformation
        /// </summary>
        public Matrix4x5 viewerToWorldMatrix { get { return transform4.localToWorldMatrix; } }
        /// <summary>
        /// World to camera matrix transformation
        /// </summary>
        public Matrix4x5 worldToViewerMatrix { get { return transform4.worldToLocalMatrix; } }

    }


}
