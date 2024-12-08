using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BP.TMPA
{
    internal class TagRange
    {
        public TextTagData[] tags;
        public int StartIndex;
        public int EndIndex;
    }

    public delegate bool TagValidator(string name, string attributes);
    public class TextParser
    {
        public const string TagPattern = @"<(\/?)([A-Za-z0-9]+)(=[^>]+)?(\/?)>";

        public static string RemoveCustomTags(string text, TagValidator validator)
        {
            string output = text;
            var matches = Regex.Matches(text, TagPattern);
            foreach (Match match in matches)
            {
                // Tag properties
                string tag = match.Value;
                string tagName = match.Groups[2].Value;
                string attributes = match.Groups[3].Value;

                if (validator.Invoke(tagName, attributes))
                    output = output.Replace(tag, string.Empty);
            }
            return output;
        }

        private readonly TagValidator validator;
        public TextParser(TagValidator validator)
        {
            this.validator = validator;
        }

        // Here we store all unique instances of a Tag we encountered
        private readonly Dictionary<string, TextTagData> registeredTags = new();
        // The currently active tags (Not closed)
        private readonly HashSet<TextTagData> activeStack = new();
        // Ranges where tags change
        private readonly List<TagRange> tagRanges = new();
        private int currentIndex = 0;

        public void ParseTags(string input)
        {
            Reset();
            var matches = Regex.Matches(input, TagPattern);
            foreach (Match match in matches.Cast<Match>())
                HandleTagMatch(match);
        }

        private void Reset()
        {
            currentIndex = 0;
            registeredTags.Clear();
            activeStack.Clear();
            tagRanges.Clear();
        }

        private void HandleTagMatch(Match match)
        {
            // Tag properties
            string tag = match.Value.ToLower();
            bool isClosingTag = match.Groups[1].Value == "/";
            string tagName = match.Groups[2].Value;
            string attributes = match.Groups[3].Value;
            int tagIndex = match.Index;

            // We will check if it is a custom tag
            if (!validator(tagName, attributes)) return;
            if (isClosingTag)
            {
                CloseTag(tag);
            }
            else
            {
                OpenTag(tag, tagName, attributes);
            }
        }
        private void CloseTag(string tag)
        {
            CloseLastRange();
            activeStack.RemoveWhere(x => x.RawTag == tag);
        }
        private void OpenTag(string tag, string tagName, string attributes)
        {
            CloseLastRange();
            if (!registeredTags.TryGetValue(tag, out TextTagData tagData))
            {
                tagData = new TextTagData();
                tagData.Initialize(tag, tagName, attributes, 0, 0);
            }
            var range = new TagRange
            {
                tags = activeStack.ToArray()
            };
            tagRanges.Add(range);
            activeStack.Add(tagData);
        }

        private void CloseLastRange()
        {
            if (tagRanges.Count > 0)
                tagRanges[^1].EndIndex = currentIndex;
        }
    }
}
