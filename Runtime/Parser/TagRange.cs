using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Represents a range of text indices that share a set of TMPA tags.
    /// </summary>
    public class TagRange
    {
        // Internal collection to store tags for this range.
        private readonly HashSet<TagData> _tags;

        /// <summary>
        /// Gets or sets the starting index of the range.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the ending index of the range.
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// Gets the collection of tags associated with this range.
        /// </summary>
        public IReadOnlyCollection<TagData> Tags => _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagRange"/> class.
        /// </summary>
        /// <param name="startIndex">The starting character index of the range.</param>
        /// <param name="endIndex">The ending character index of the range.</param>
        public TagRange(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            _tags = new HashSet<TagData>();
        }

        /// <summary>
        /// Adds a new tag to the range. If a tag with the same name already exists, it is overwritten.
        /// </summary>
        /// <param name="newTag">The new tag data to add or overwrite.</param>
        /// <returns>
        /// <c>true</c> if the tag was added successfully; otherwise, <c>false</c>.
        /// </returns>
        public bool AddOrOverwriteTag(TagData newTag)
        {
            // Remove an existing tag with the same name, if any.
            var existingTag = _tags.FirstOrDefault(t => t.Name.Equals(newTag.Name));
            if (existingTag != null)
            {
                _tags.Remove(existingTag);
            }

            return _tags.Add(newTag);
        }

        /// <summary>
        /// Adds or overwrites multiple tags within the range.
        /// </summary>
        /// <param name="newTags">An enumerable collection of new tag data.</param>
        public void AddOrOverwriteTags(IEnumerable<TagData> newTags)
        {
            foreach (var tag in newTags)
            {
                AddOrOverwriteTag(tag);
            }
        }

        /// <summary>
        /// Returns a string that represents the current tag range,
        /// including the start/end indices and all associated tags.
        /// </summary>
        /// <returns>A string representation of the tag range.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"StartIndex: {StartIndex}, EndIndex: {EndIndex}, Tags: [");

            foreach (var tag in _tags)
            {
                builder.Append(tag).Append(" ");
            }

            builder.Append("]");
            return builder.ToString();
        }
    }
}
