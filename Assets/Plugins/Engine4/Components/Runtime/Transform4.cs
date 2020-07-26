using UnityEngine;
using System.Collections.Generic;
using System;
using Engine4.Internal;

namespace Engine4
{
    /// <summary>
    /// Transformation component in 4D
    /// </summary>
    [DisallowMultipleComponent]
    public class Transform4 : MonoBehaviour
    {
        [SerializeField]
        Vector4 m_Position = Vector4.zero;
        [SerializeField]
        [Matrix4AsEuler]
        Matrix4 m_Rotation = Matrix4.identity;
        [SerializeField]
        Vector4 m_Scale = Vector4.one;
        [NonSerialized]
        List<Transform4> _childs = new List<Transform4>();
        [NonSerialized]
        TransformCallback _callback;
        [NonSerialized]
        Transform4 _parent;

        [NonSerialized]
        Matrix4x5 _local;
        [NonSerialized]
        Matrix4x5 _localToWorld;
        [NonSerialized]
        Matrix4x5 _worldToLocal;
        [NonSerialized]
        bool _matrixDirty = true;

        /// <summary>
        /// Position relative to parent
        /// </summary>
        public Vector4 localPosition
        {
            get
            {
                return m_Position;
            }
            set
            {
                if (m_Position != value)
                {
                    m_Position = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Rotation (in euler) relative to parent
        /// </summary>
        public Euler4 localEulerAngles
        {
            get
            {
                return m_Rotation.ToEuler();
            }
            set
            {
                m_Rotation = Matrix4.Euler(value);
                SetDirty();
            }
        }

        /// <summary>
        /// Rotation (in matrix) relative to parent
        /// </summary>
        public Matrix4 localRotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                if (m_Rotation != value)
                {
                    m_Rotation = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Scaling value.
        /// </summary>
        /// <remarks>
        /// Scaling won't affect to its children.
        /// </remarks>
        public Vector4 localScale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                if (m_Scale != value)
                {
                    m_Scale = value;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Position relative to world space
        /// </summary>
        public Vector4 position
        {
            get
            {
                return localToWorldMatrix.position;
            }
            set
            {
                localPosition = parent ? parent.worldToLocalMatrix * value : value;
            }
        }

        /// <summary>
        /// Rotation (in euler) to world space
        /// </summary>
        public Euler4 eulerAngles
        {
            get
            {
                return rotation.ToEuler();
            }
            set
            {
                rotation = Matrix4.Euler(value);
                SetDirty();
            }
        }
        
        /// <summary>
        /// Rotation relative to world space
        /// </summary>
        public Matrix4 rotation
        {
            get
            {
                return parent ? localToWorldMatrix.rotation : localRotation;
            }
            set
            {
                localRotation = parent ? parent.worldToLocalMatrix.rotation * value : value;
            }
        }

        void ProcessMatrix()
        {
            _local = new Matrix4x5(localPosition, localRotation);
            _localToWorld = parent ? _parent.localToWorldMatrix * _local : _local;
            _worldToLocal = Matrix4x5.Inverse(_localToWorld);
            _matrixDirty = false;
        }

        /// <summary>
        /// The analogous localToWorldMatrix just for 4 manipulations
        /// </summary>
        /// <remarks>
        /// Scale doesn't included in the matrix
        /// </remarks>
        public Matrix4x5 localToWorldMatrix
        {
            get
            {
                if (_matrixDirty) ProcessMatrix();
                return _localToWorld;
            }
        }

        /// <summary>
        /// The analogous worldToLocalMatrix just for 4 manipulations
        /// </summary>
        /// <remarks>
        /// Scale doesn't included in the matrix
        /// </remarks>
        public Matrix4x5 worldToLocalMatrix
        {
            get
            {
                if (_matrixDirty) ProcessMatrix();
                return _worldToLocal;
            }
        }

        /// <summary>
        /// Get Transform4 in ancestor. Note that not every objects have Transform4.
        /// </summary>
        public Transform4 parent
        {
            get
            {
                return _parent ? _parent : (_parent = Runtime.GetComponentFromParent<Transform4>(transform));
            }
        }

        /// <summary>
        /// Get Transform4 in childrens. note this return NOT just a depth
        /// </summary>
        List<Transform4> childrens
        {
            get
            {
                if (_childs.Count == 0) GetComponentsInChildren(true, _childs);
                return _childs;
            }
        }

        void OnTransformParentChanged()
        {
            _parent = null;
            SetDirty();
        }

        void OnTransformChildrenChanged()
        {
            _childs.Clear();
        }

        void OnTransform4Dirty()
        {
            _matrixDirty = true;

            if (_callback != null)
                _callback();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }

        void OnValidate()
        {
            SetDirty();
        }

        internal void SetDirty()
        {
            if (_matrixDirty)
            {
                //  ProcessMatrix();
                return; // Alert later
            }

            for (int i = childrens.Count; i-- > 0;)
            {
                // This component got included too
                if (_childs[i])
                    _childs[i].OnTransform4Dirty();
            }
        }

        /// <summary>
        /// Event to be fired when this transform changed.
        /// </summary>
        public TransformCallback update
        {
            get { return _callback; }
            set { _callback = value; }
        }

        /// <summary>
        /// Rotate this transform (in matrix) in given space orientation.
        /// </summary>
        public void Rotate(Matrix4 value, Space4 space)
        {
            switch (space)
            {
                case Space4.Self:
                    localRotation *= value;
                    break;
                case Space4.World:
                    rotation = value * rotation; // order matter!
                    break;
                case Space4.View:
                    rotation = Matrix4.Transform(Viewer4.main.worldToViewerMatrix.rotation, value) * rotation; // order matter!
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Rotate this transform (in 3D quaternion) in given space orientation.
        /// </summary>
        public void Rotate(Quaternion value, Space4 space)
        {
            Rotate(Matrix4x4.TRS(Vector3.zero, value, Vector3.one), space);
        }

        /// <summary>
        /// Rotate this transform (in euler) in given space orientation.
        /// </summary>
        public void Rotate(Euler4 value, Space4 space)
        {
            Rotate(Matrix4.Euler(value), space);
        }

        /// <summary>
        /// Translate this transform in given space orientation.
        /// </summary>
        public void Translate(Vector4 value, Space4 space)
        {
            switch (space)
            {
                case Space4.Self:
                    localPosition += value;
                    break;
                case Space4.World:
                    position += value;
                    break;
                case Space4.View:
                    position += Viewer4.main.viewerToWorldMatrix.rotation * value;
                    break;
                default:
                    break;
            }
        }


        /// <summary> Get the right (X+) world axis of the transform </summary>
        public Vector4 rightward { get { return rotation.Column0; } }

        /// <summary> Get the up (Y+) world axis of the transform </summary>
        public Vector4 upward { get { return rotation.Column1; } }

        /// <summary> Get the forward (Z+) world axis of the transform </summary>
        public Vector4 forward { get { return rotation.Column2; } }

        /// <summary> Get the overward (W+) world axis of the transform </summary>
        public Vector4 overward { get { return rotation.Column3; } }

#if UNITY_EDITOR

        [ContextMenu("Reset Position")]
        void _ResetPosition()
        {
            UnityEditor.Undo.RecordObject(this, "Reset Position4");
            localPosition = Vector4.zero;
        }

        [ContextMenu("Reset Rotation")]
        void _ResetRotation()
        {
            UnityEditor.Undo.RecordObject(this, "Reset Rotation4");
            localRotation = Matrix4.identity;
        }

        [ContextMenu("Reset Scale")]
        void _ResetScale()
        {
            UnityEditor.Undo.RecordObject(this, "Reset Scale4");
            localScale = Vector4.one;
        }

        void Reset ()
        {
            SetDirty();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

       

#endif

    }
}