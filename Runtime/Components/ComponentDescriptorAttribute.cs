using System;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Attribute used to mark and describe a text motion component (effect or transition)
    /// for editor tooling and runtime discovery.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComponentDescriptorAttribute : Attribute
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

        public ComponentDescriptorAttribute(
            string displayName,
            string category = "Default",
            string description = "")
        {
            DisplayName = displayName;
            Category = category;
            Description = description;
        }
    }
}
