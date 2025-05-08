using System;

namespace BP.TextMotion
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TextEffectAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public TextEffectAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}