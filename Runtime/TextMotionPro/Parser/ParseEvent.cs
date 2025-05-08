namespace BP.TextMotion
{
    /// <summary>
    /// Represents a single parsing event: either a tag open/close or an action marker.
    /// </summary>
    internal class ParserEvent
    {
        /// <summary>Gets the zero-based index in the original input where the event starts.</summary>
        public int Index { get; }

        /// <summary>Gets the length of the matched token (tag or action).</summary>
        public int Length { get; }

        /// <summary>True if this event is a tag; false if it is an action.</summary>
        public bool IsTag { get; }

        /// <summary>True if this tag event represents a closing tag.</summary>
        public bool IsClosing { get; }

        /// <summary>Gets the raw text of the matched token (e.g., "&lt;wave=5&gt;" or "{jump}").</summary>
        public string Raw { get; }

        /// <summary>Gets the name of the tag or action.</summary>
        public string Name { get; }

        /// <summary>Gets the optional value or parameters of the tag/action, or null if none.</summary>
        public string Value { get; }

        /// <summary>
        /// Constructs a new <see cref="ParserEvent"/>.
        /// </summary>
        public ParserEvent(int index, int length, bool isTag, bool isClosing, string raw, string name, string value)
        {
            Index = index;
            Length = length;
            IsTag = isTag;
            IsClosing = isClosing;
            Raw = raw;
            Name = name;
            Value = value;
        }

        public override string ToString() =>
            $"[{Index}:{Length}] {(IsTag ? (IsClosing ? "</" : "<") : "{")}{Name}{(Value != null ? "=" + Value : string.Empty)}{(IsTag ? ">" : "}")}";
    }
}
