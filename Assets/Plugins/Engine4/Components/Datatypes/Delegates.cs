namespace Engine4
{
    /// <summary>
    /// Delegate to notice transform changes
    /// </summary>
    public delegate void TransformCallback();

    /// <summary>
    /// Delegate to notice viewer update
    /// </summary>
    public delegate void ViewerDispatch(DirtyLevel level);
}

namespace Engine4.Physics
{
    /// <summary>
    /// Delegate to notice collision impacts
    /// </summary>
    public delegate void CollisionCallback(CollisionHit4 collision);
}
