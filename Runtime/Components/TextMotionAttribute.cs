using System;

namespace BP.TextMotion
{
    public enum TextMotionRole
    {
        Effect,
        Transition
    }

    /// <summary>
    /// Attribute used to mark and describe a text motion component (effect or transition)
    /// for editor tooling and runtime discovery.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TextMotionAttribute : Attribute
    {
        /// <summary>
        /// The display name used in the editor or MotionProfile system.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Optional category used for grouping components in UI.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Optional description of the component's behavior.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Optional type of the motion component (Effect or Transition).
        /// </summary>
        public TextMotionRole Role { get; }

        public TextMotionAttribute(
            string displayName,
            TextMotionRole role,
            string category = "Default",
            string description = "")
        {
            DisplayName = displayName;
            Role = role;
            Category = category;
            Description = description;
        }
    }
}
