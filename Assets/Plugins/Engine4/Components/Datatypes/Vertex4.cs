using Engine4.Internal;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Engine4
{
    /// <summary>
    /// Additional vertex channel
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexProfile
    {
        /// <summary>
        /// Color of the vertex
        /// </summary>
        public Color color;

        /// <summary>
        /// First UV channel
        /// </summary>
        public UnityEngine.Vector4 uv;

        /// <summary>
        /// Second UV channel
        /// </summary>
        public UnityEngine.Vector4 uv2;

        /// <summary>
        /// Third UV channel
        /// </summary>
        public UnityEngine.Vector4 uv3;

        /// <summary>
        /// Default state for VertexProfile
        /// </summary>
        public static VertexProfile initial
        {
            get { return new VertexProfile(Color.white); }
        }

        /// 
        [HideInDocs]
        public VertexProfile(Color color) : this(color, UnityEngine.Vector4.zero, UnityEngine.Vector4.zero, UnityEngine.Vector4.zero) { }
        /// 
        [HideInDocs]
        public VertexProfile(Color color, UnityEngine.Vector4 uv) : this(color, uv, UnityEngine.Vector4.zero, UnityEngine.Vector4.zero) { }
        /// 
        [HideInDocs]
        public VertexProfile(Color color, UnityEngine.Vector4 uv, UnityEngine.Vector4 uv2) : this(color, uv, uv2, UnityEngine.Vector4.zero) { }
        /// 
        [HideInDocs]
        public VertexProfile(UnityEngine.Vector4 uv) : this(Color.white, uv, UnityEngine.Vector4.zero, UnityEngine.Vector4.zero) { }
        /// 
        [HideInDocs]
        public VertexProfile(UnityEngine.Vector4 uv, UnityEngine.Vector4 uv2) : this(Color.white, uv, uv2, UnityEngine.Vector4.zero) { }
        /// 
        [HideInDocs]
        public VertexProfile(UnityEngine.Vector4 uv, UnityEngine.Vector4 uv2, UnityEngine.Vector4 uv3) : this(Color.white, uv, uv2, uv3) { }

        /// <summary>
        /// Create new vertex profile
        /// </summary>
        public VertexProfile(Color color, UnityEngine.Vector4 uv, UnityEngine.Vector4 uv2, UnityEngine.Vector4 uv3)
        {
            this.color = color;
            this.uv = uv;
            this.uv2 = uv2;
            this.uv3 = uv3;
        }

        /// <summary>
        /// Interpolate between two profile
        /// </summary>
        public static VertexProfile Lerp(VertexProfile a, VertexProfile b, float t)
        {
            return LerpUnclamped(a, b, Mathf.Clamp01(t));
        }

        /// <summary>
        /// Interpolate between two profile (without clamping)
        /// </summary>
        public static VertexProfile LerpUnclamped(VertexProfile a, VertexProfile b, float t)
        {
            return new VertexProfile
            {
                color = Color.LerpUnclamped(a.color, b.color, t),
                uv = UnityEngine.Vector4.LerpUnclamped(a.uv, b.uv, t),
                uv2 = UnityEngine.Vector4.LerpUnclamped(a.uv2, b.uv2, t),
                uv3 = UnityEngine.Vector4.LerpUnclamped(a.uv3, b.uv3, t),
            };
        }

        /// <summary>
        /// Interpolate between two profile (without clamping)
        /// </summary>
        public static VertexProfile LerpUnclamped(VertexProfile a, VertexProfile b, float t, Matrix4x5 m)
        {
            return new VertexProfile
            {
                color = Color.LerpUnclamped(a.color, b.color, t),
                uv = m * UnityEngine.Vector4.LerpUnclamped(a.uv, b.uv, t),
                uv2 = m * UnityEngine.Vector4.LerpUnclamped(a.uv2, b.uv2, t),
                uv3 = m * UnityEngine.Vector4.LerpUnclamped(a.uv3, b.uv3, t),
            };
        }

        /// <summary>
        /// Transform UV Profile
        /// </summary>
        public static VertexProfile Transform (Matrix4x4 t, VertexProfile profile)
        {
            return new VertexProfile()
            {
                color = profile.color,
                uv = t * profile.uv,
                uv2 = t * profile.uv2,
                uv3 = t * profile.uv3,
            };
        }

        /// <summary>
        /// Transform UV Profile
        /// </summary>
        public static VertexProfile Transform (Matrix4x4 t, VertexProfile profile, UnityEngine.Vector4 center)
        {
            return new VertexProfile()
            {
                color = profile.color,
                uv = t * profile.uv + center,
                uv2 = t * profile.uv2 + center,
                uv3 = t * profile.uv3 + center,
            };
        }
    }

}
