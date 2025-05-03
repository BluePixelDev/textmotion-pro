using System;

namespace BP.TextMotionPro
{
    public class TextEffectDescriptor
    {
        public Type Type { get; }
        public string Name { get; }
        public string Description { get; }

        public TextEffectDescriptor(Type type, string name, string description)
        {
            Type = type;
            Name = name;
            Description = description;
        }
    }
}