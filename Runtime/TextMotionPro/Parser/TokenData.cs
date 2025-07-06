namespace BP.TextMotionPro
{
    /// <summary>
    /// Class for parsed tokens (tags and actions) in motion text.
    /// </summary>
    public class TokenData
    {
        /// <summary>Raw text of the token, e.g. "<wave=5>" or "{jump}".</summary>
        public string Raw { get; }

        /// <summary>Name of the tag or action.</summary>
        public string Name { get; }

        /// <summary>Value or parameters of the token, e.g. "5".</summary>
        public string Value { get; }

        /// <summary>Position in the cleaned text where this token occurs.</summary>
        public int Position { get; }

        public TokenData(string raw, string name, string value, int position)
        {
            Raw = raw;
            Name = name;
            Value = value;
            Position = position;
        }
    }
}
