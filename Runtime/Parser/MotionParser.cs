using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Parses input text to extract tags and generate corresponding tag ranges.
    /// </summary>
    public class MotionParser
    {
        /// <summary>
        /// The regular expression pattern used to identify TMPA tags.
        /// </summary>
        public const string RegexTagPattern = @"<(\/?)([-a-zA-Z0-9]+)(?:=([^><]*))?>";

        // Compiled regex for matching tags.
        private readonly Regex tagRegex;
        // Validator for ensuring that tags meet required criteria.
        private readonly ITagValidator tagValidator;

        // Stack of currently active (open) tags.
        private readonly LinkedList<TagData> activeStack;
        // List of tag ranges found during parsing.
        private readonly List<TagRange> tagRanges;

        // Parsing state variables.
        private TagData[] validTags;
        private TagRange currentRange;
        private int textLength, currentIndex, tagCount, changeIndex, shiftOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionParser"/> class.
        /// </summary>
        /// <param name="validator">An instance of <see cref="ITagValidator"/> used for validating tags.</param>
        public MotionParser(ITagValidator validator)
        {
            tagRegex = new Regex(RegexTagPattern, RegexOptions.Compiled);
            tagValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            activeStack = new LinkedList<TagData>();
            tagRanges = new List<TagRange>();
            ResetState();
        }

        /// <summary>
        /// Parses the input text and returns a collection of valid tag ranges.
        /// </summary>
        /// <param name="input">The text to parse.</param>
        /// <returns>A read-only collection of <see cref="TagRange"/> objects.</returns>
        public IReadOnlyCollection<TagRange> Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty", nameof(input));

            ResetState();
            textLength = input.Length;
            validTags = ExtractValidTags(input);
            tagCount = validTags.Length;

            foreach (var tag in validTags)
            {
                if (tag.IsClosing)
                    HandleTagClose(tag);
                else
                    HandleTagOpen(tag);

                shiftOffset += tag.RawTag.Length;
                currentIndex++;
            }

            return tagRanges;
        }

        /// <summary>
        /// Removes all valid tags from the input text.
        /// </summary>
        /// <param name="text">The text from which to remove tags.</param>
        /// <returns>The text with all valid tags removed.</returns>
        public string RemoveTags(string text)
        {
            string output = text;
            var matches = tagRegex.Matches(text);

            foreach (Match match in matches.Cast<Match>())
            {
                string tag = match.Value;
                string tagName = match.Groups[2].Value;
                string attributes = match.Groups[3].Value;

                if (tagValidator.Validate(tagName, attributes))
                    output = output.Replace(tag, string.Empty);
            }

            return output;
        }

        /// <summary>
        /// Resets the parser state to prepare for a new parse operation.
        /// </summary>
        private void ResetState()
        {
            textLength = 0;
            tagCount = 0;
            currentIndex = 0;
            changeIndex = -1;
            shiftOffset = 0;
            activeStack.Clear();
            tagRanges.Clear();
            currentRange = null;
        }

        /// <summary>
        /// Extracts tags from the input text using regex.
        /// </summary>
        /// <param name="input">The input text.</param>
        /// <returns>An array of <see cref="TagData"/> parsed from the text.</returns>
        public TagData[] ExtractValidTags(string input)
        {
            var matches = tagRegex.Matches(input);
            List<TagData> validTags = new();
            foreach (Match match in matches.Cast<Match>())
            {
                TagData tag = ParseTagData(match);
                if (tagValidator.Validate(tag.Name, tag.Attributes))
                {
                    validTags.Add(tag);
                }
            }
            return validTags.ToArray();
        }

        /// <summary>
        /// Parses a single regex match into a <see cref="TagData"/> object.
        /// </summary>
        /// <param name="match">The regex match containing tag information.</param>
        /// <returns>A <see cref="TagData"/> instance representing the tag.</returns>
        public TagData ParseTagData(Match match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            string tag = match.Value;
            bool isClosingTag = match.Groups[1].Value == "/";
            string tagName = match.Groups[2].Value;
            string attributes = match.Groups[3].Value;
            int tagIndex = match.Index;

            return new TagData
            {
                RawTag = tag,
                Name = tagName,
                Attributes = attributes,
                StartIndex = tagIndex,
                EndIndex = tagIndex + (tag.Length - 1),
                IsClosing = isClosingTag
            };
        }

        /// <summary>
        /// Handles an opening tag by adding it to the active stack and updating tag ranges.
        /// </summary>
        /// <param name="tag">The opening tag data.</param>
        private void HandleTagOpen(TagData tag)
        {
            activeStack.AddFirst(tag);

            // Update the change index if there is no left-adjacent tag.
            if (!HasLeftAdj(tag.StartIndex))
            {
                changeIndex = tag.StartIndex - shiftOffset - 1;
            }

            // If there's no right-adjacent tag, close the current range and start a new one.
            if (!HasRightAdj(tag.EndIndex))
            {
                CloseRange(changeIndex);
                CreateNewRange(tag.StartIndex - shiftOffset);
            }
        }

        /// <summary>
        /// Handles a closing tag by matching it with its corresponding opening tag and updating tag ranges.
        /// </summary>
        /// <param name="tag">The closing tag data.</param>
        private void HandleTagClose(TagData tag)
        {
            // Find the corresponding opening tag in the active stack.
            var existingTag = activeStack.FirstOrDefault(x => x.Name.Equals(tag.Name));
            if (existingTag == null)
                return;

            activeStack.Remove(existingTag);
            bool isLast = tag.EndIndex == textLength - 1;

            if (!HasLeftAdj(tag.StartIndex))
            {
                changeIndex = tag.StartIndex - shiftOffset - 1;
            }

            if (currentRange == null)
                return;

            if (!HasRightAdj(tag.EndIndex))
            {
                CloseRange(changeIndex);
                // If not the last tag and there are still active tags, start a new range.
                if (!isLast && activeStack.Count != 0)
                {
                    CreateNewRange(tag.StartIndex - shiftOffset);
                }
            }
        }

        /// <summary>
        /// Checks whether the tag preceding the current one is directly adjacent (to the left).
        /// </summary>
        /// <param name="startIndex">The start index of the current tag.</param>
        /// <returns><c>true</c> if there is an adjacent tag on the left; otherwise, <c>false</c>.</returns>
        private bool HasLeftAdj(int startIndex) =>
            currentIndex - 1 >= 0 && validTags[currentIndex - 1].EndIndex == startIndex - 1;

        /// <summary>
        /// Checks whether the tag following the current one is directly adjacent (to the right).
        /// </summary>
        /// <param name="EndIndex">The end index of the current tag.</param>
        /// <returns><c>true</c> if there is an adjacent tag on the right; otherwise, <c>false</c>.</returns>
        private bool HasRightAdj(int EndIndex) =>
            currentIndex + 1 < tagCount && validTags[currentIndex + 1].StartIndex == EndIndex + 1;

        /// <summary>
        /// Closes the current tag range by setting its end index.
        /// </summary>
        /// <param name="EndIndex">The end index for the current range.</param>
        private void CloseRange(int EndIndex)
        {
            if (currentRange == null)
                return;

            currentRange.EndIndex = EndIndex;
            currentRange = null;
        }

        /// <summary>
        /// Creates a new tag range starting at the specified index.
        /// Adds all currently active tags to the new range.
        /// </summary>
        /// <param name="startIndex">The start index for the new range.</param>
        private void CreateNewRange(int startIndex)
        {
            if (currentRange != null)
            {
                throw new InvalidOperationException("Cannot create a new range when the current range is not closed");
            }

            currentRange = new TagRange(startIndex, textLength - 1);
            tagRanges.Add(currentRange);

            // Add all active tags (in reverse order) to the new range.
            currentRange.AddOrOverwriteTags(activeStack.Reverse());
        }
    }
}
