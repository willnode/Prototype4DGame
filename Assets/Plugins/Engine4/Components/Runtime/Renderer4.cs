using Engine4.Rendering;
using System;
using System.Collections.Generic;
using UnityEngine;
using Engine4.Internal;

namespace Engine4
{
    /// <summary>
    /// Component to render and manage outputs from modelers
    /// </summary>
    [RequireComponent(typeof(Transform4), typeof(MeshFilter), typeof(MeshRenderer)),
        ExecuteInEditMode, DisallowMultipleComponent]
    public class Renderer4 : MonoBehaviour4, INeedTransform4Scale
    {

        /// <summary>
        /// Simplex type that'll be generated
        /// </summary>
        public VisualizeMode visualization = VisualizeMode.Solid;

        /// <summary>
        /// Additional vertex properties to be included in this renderer
        /// </summary>
        public VertexProfiles profile = VertexProfiles.None;

        internal Buffer4 _buffer = new Buffer4();
        internal Buffer4 _origin = new Buffer4();
        internal Buffer3 _helper = new Buffer3();
        internal SphereBounds4 _bounds = new SphereBounds4();
        internal Mesh _mesh;
        [NonSerialized]
        DirtyLevel _dirty = DirtyLevel.Model;
        [NonSerialized]
        List<Modeler4> _modelers = new List<Modeler4>();
        [NonSerialized]
        List<Modifier4> _modifiers = new List<Modifier4>();
        [NonSerialized]
        IVisualizer _visualizer;
        /// <summary>
        /// 0 = off or idle, 1 = sent, 2 = sent, but data outdated, 3 = done
        /// </summary>
        //[NonSerialized]
        int _hyperState = 0;
        bool _hyperSafe;

        void LateUpdate()
        {
            switch (_dirty)
            {
                case DirtyLevel.None:
                    return;
                case DirtyLevel.Visualizer:
                    ExecuteVisualizer();
                    break;
                case DirtyLevel.Transform:
                    ExecuteTransform();
                    ExecuteVisualizer();
                    break;
                case DirtyLevel.Modifier:
                    ExecuteModifier();
                    ExecuteTransform();
                    ExecuteVisualizer();
                    break;
                case DirtyLevel.Model:
                    ExecuteModel();
                    ExecuteModifier();
                    ExecuteTransform();
                    ExecuteVisualizer();
                    break;
            }
            _dirty = DirtyLevel.None;
        }

        void ExecuteModel()
        {
            ValidateBuffer();
            for (int i = modelers.Count; i-- > 0;)
            {
                if (_modelers[i].enabled)
                    _modelers[i].CreateModel(_origin);
                _buffer.Align();
            }
        }

        void ExecuteModifier()
        {
            _origin.CopyTo(_buffer, true);
            for (int i = modifiers.Count; i-- > 0;)
            {
                if (_modifiers[i].enabled)
                    _modifiers[i].ModifyModel(_buffer);
            }
            _bounds = new SphereBounds4(_buffer.m_Vertices.GetBounding(_buffer.m_VerticesCount));
        }


        void OnTransformChanged()
        {
            SetDirty(DirtyLevel.Transform);
        }

        /// <summary>
        /// Calculate and upload the final projection
        /// </summary>
        void ExecuteTransform()
        {

            if (_visualizer == null)
                return;


            var bound = _bounds;
            bound.radius *= Vector4.MaxPerElem(transform4.localScale);

            if (viewer4.culling && viewer4.projector.IsCullable(bound))
            {
                _visualizer.Clear(_mesh);
                return;
            }

            if (viewer4.background)
            {
                if (_hyperState == 3)
                    ExecuteVisualizer();

                if (_hyperState == 0)
                {
                    _hyperState = 1;
                    _visualizer.Initialize(_helper);
                    viewer4.projectorJob.AddJob(new ProjectUnit(_buffer, transform4.localToWorldMatrix.ToTRS(transform4.localScale), _visualizer, this));
                }
                else
                    _hyperState = 2;
            }
            else
            {
                _visualizer.Initialize(_helper);
                viewer4.projector.Project(_buffer, transform4.localToWorldMatrix.ToTRS(transform4.localScale), _visualizer);
            }
        }

        void ExecuteVisualizer()
        {
            if (!viewer4.background || _hyperState == 0 || _hyperState == 3)
            {
                _visualizer.End(_mesh, profile);
                _hyperState = 0;
            }
        }

        internal void SignalHyperExecution()
        {
            var old = _hyperState;
            SetDirty(old == 1 ? DirtyLevel.Visualizer : DirtyLevel.Transform);
            _hyperState = 3;


        #if UNITY_EDITOR
            if (!_hyperSafe)
            {
                UnityEditor.EditorApplication.delayCall += UnityEditor.SceneView.RepaintAll;
                UnityEditor.EditorApplication.delayCall += LateUpdate;
            }
        #endif
        }

        void OnEnable()
        {
            _modelers.Clear();
            _modifiers.Clear();
            _buffer.Clean();
            _origin.Clean();
            _dirty = DirtyLevel.Model;
            Viewer4.update += SetDirty;
            transform4.update += OnTransformChanged;
            _hyperState = 0;
            _hyperSafe = Application.isPlaying;
        }

        void OnDisable()
        {
            Viewer4.update -= SetDirty;
            transform4.update -= OnTransformChanged;
            if (_mesh)
                _mesh.Clear();
        }

        void OnValidate()
        {
            SetDirty(DirtyLevel.Model);
        }

        void Reset()
        {
            if (GetComponent<MeshRenderer>().sharedMaterial == null)
                GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Default-DoubleSided");
        }

        /// <summary>
        /// Set this renderer to be validated later. Set rebake if mesh reconstruction is needed
        /// </summary>
        public void SetDirty(DirtyLevel level)
        {
            _dirty = (DirtyLevel)(Math.Max((int)level, (int)_dirty));
        }

        /// <summary>
        /// List of modeler
        /// </summary>
        public List<Modeler4> modelers
        {
            get
            {
                if (_modelers.Count == 0) { GetComponents(_modelers); _modelers.Reverse(); }
                return _modelers;
            }
        }

        /// <summary>
        /// List of modeler
        /// </summary>
        public List<Modifier4> modifiers
        {
            get
            {
                if (_modifiers.Count == 0) { GetComponents(_modifiers); _modifiers.Reverse(); };
                return _modifiers;
            }
        }

        /// <summary>
        /// Invalidate list of modeler
        /// </summary>
        public void ReacquireModelers()
        {
            _modelers.Clear();
            SetDirty(DirtyLevel.Model);
        }

        /// <summary>
        /// Invalidate list of modifier
        /// </summary>
        public void ReacquireModifiers()
        {
            _modifiers.Clear();
            SetDirty(DirtyLevel.Modifier);
        }

        void ValidateBuffer()
        {
            // Setup mesh
            if (!_mesh)
            {
                var m = _mesh = new Mesh();
                m.name = "S4 Renderer Buffer";
                m.hideFlags = HideFlags.DontSave;
            }
            //else
            //    _mesh.Clear();
            GetComponent<MeshFilter>().mesh = _mesh;

            // Setup buffer
            _origin.Clear();

            // Setup visualizer
            if (_visualizer != null && _mesh)
                _visualizer.Clear(_mesh);

            switch (visualization)
            {
                case VisualizeMode.Particle: _visualizer = (_visualizer as ParticleVisualizer4) ?? new ParticleVisualizer4(); break;
                case VisualizeMode.Wire: _visualizer = (_visualizer as WireVisualizer4) ?? new WireVisualizer4(); break;
                case VisualizeMode.Solid: _visualizer = (_visualizer as SolidVisualizer4) ?? new SolidVisualizer4(); break;
                case VisualizeMode.Custom: _visualizer = GetComponent<Visualizer4>(); break;
            }

            if (_visualizer != null)
                _origin.simplex = viewer4.projector.SimplexModeForVisualizing(_visualizer.WorkingSimplexCase);

        }

    }

}
