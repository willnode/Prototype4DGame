using System;

namespace Engine4.Internal
{

    /// <summary>
    /// Attach this attribute to a Matrix4 field so it will be shown as Euler rotation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class Matrix4AsEulerAttribute : Attribute
    {
        /// 
        public Matrix4AsEulerAttribute()
        {
        }
    }

    /// <summary>
    /// Used internally by the documentation engine to exclude certain codes.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple =false, Inherited = true)]
    internal sealed class HideInDocs : Attribute
    {
    }

    /// <summary>
    /// An empty interface to mark a component that it needs transform4 scale so it can be shown.
    /// </summary>
    public interface INeedTransform4Scale { }
}
