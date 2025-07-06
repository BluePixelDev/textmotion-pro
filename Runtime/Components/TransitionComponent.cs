namespace BP.TextMotionPro
{

    /// <summary>
    /// Abstract base class for defining named text transitions in the TextMotionPro system.
    /// A text transition describes how characters animate into or out of view,
    /// and can be identified by a unique name (e.g., "pop", "cut", "fade").
    /// </summary>
    public abstract class TransitionComponent : MotionComponent
    {
        public abstract void Apply(MotionContext context);
    }
}
