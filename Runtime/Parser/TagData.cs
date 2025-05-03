using System;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Represents a TextMeshPro animation tag with its raw data,
    /// parsed name, attributes, and the indices where it applies.
    /// </summary>
    public class TagData
    {
        /// <summary>
        /// Gets or sets the raw tag string (e.g., "<popin>").
        /// </summary>
        public string RawTag { get; set; }

        /// <summary>
        /// Gets or sets the parsed tag name (e.g., "popin").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attributes defined within the tag.
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// Gets or sets the starting character index where the tag applies.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the ending character index where the tag applies.
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a closing tag.
        /// </summary>
        public bool IsClosing { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current tag data.
        /// Two tags are considered equal if their <see cref="RawTag"/> strings are identical.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><c>true</c> if the specified object is equal to the current tag data; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is TagData data && RawTag == data.RawTag;
        }

        /// <summary>
        /// Returns a hash code for the tag data based on its <see cref="RawTag"/>.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(RawTag);
        }

        /// <summary>
        /// Returns a string representation of the tag data.
        /// The raw tag is displayed without the "<" character.
        /// </summary>
        /// <returns>A string representing the tag data.</returns>
        public override string ToString()
        {
            return $"RawTag: {RawTag.Replace("<", "")}, Name: {Name}, Attributes: {Attributes}, " +
                   $"StartIndex: {StartIndex}, EndIndex: {EndIndex}, IsClosing: {IsClosing}";
        }
    }
}
