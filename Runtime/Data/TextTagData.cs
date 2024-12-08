using System;

namespace BP.TMPA
{
    /// <summary>
    /// Represents data associated with a text tag, including its raw content, name, attributes, and location.
    /// </summary>
    public class TextTagData
    {
        /// <summary>
        /// Indicates whether the tag data is valid and fully populated.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// The full, unmodified tag string.
        /// </summary>
        public string RawTag { get; private set; }

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The attributes associated with the tag.
        /// </summary>
        public string Attributes { get; private set; }

        /// <summary>
        /// The starting index of the tag in the original text.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// The ending index of the tag in the original text.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TextTagData class.
        /// </summary>
        public TextTagData() => Reset();

        /// <summary>
        /// Initializes the tag data with new data.
        /// </summary>
        /// <param name="rawTag">The full tag string</param>
        /// <param name="name">The name of the tag</param>
        /// <param name="attributes">The attributes of the tag</param>
        /// <param name="startIndex">The starting index of the tag</param>
        /// <param name="endIndex">The ending index of the tag</param>
        public void Initialize(string rawTag, string name, string attributes, int startIndex, int endIndex)
        {
            IsValid = true;
            RawTag = rawTag;
            Name = name;
            Attributes = attributes;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        /// <summary>
        /// Updates the end index of the tag.
        /// </summary>
        /// <param name="endIndex">The new ending index</param>
        public void Close(int endIndex)
        {
            EndIndex = endIndex;
        }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Reset()
        {
            IsValid = false;
            RawTag = null;
            Name = null;
            Attributes = null;
            StartIndex = 0;
            EndIndex = 0;
        }

        public void Clear() => Reset();

        // ==== OVERRIDES ====
        public override string ToString()
        {
            return $"Tag: {Name}, Attributes: {Attributes}, StartIndex: {StartIndex}, EndIndex: {EndIndex}";
        }
        public override bool Equals(object obj)
        {
            return obj is TextTagData data &&
                   Name == data.Name &&
                   Attributes == data.Attributes;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Attributes);
        }
    }
}
