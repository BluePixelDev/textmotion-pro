namespace BP.TextMotion
{

    /// <summary>
    /// Abstract base class for defining named text transitions in the TextMotion system.
    /// A text transition describes how characters animate into or out of view,
    /// and can be identified by a unique name (e.g., "pop", "cut", "fade").
    /// </summary>
    public abstract class TextTransition : MotionComponent
    {
        public abstract void Apply(TransitionContext context);
    }
}
