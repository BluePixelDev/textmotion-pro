using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Parses motion tags and actions from input text, emitting ranges and a clean output.
    /// </summary>
    public sealed class MotionParser : IParser
    {
        private static readonly Regex TagPattern = new(@"<(\/?)((?:[-\w]+))(?:=([^><]*))?>", RegexOptions.Compiled);
        private static readonly Regex ActionPattern = new(@"{([-\w]+)(?:=([^{}]*))?}", RegexOptions.Compiled);
        private readonly ITagValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionParser"/> class.
        /// </summary>
        /// <param name="tagValidator">Validator to approve or reject tags and parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tagValidator"/> is null.</exception>
        public MotionParser(ITagValidator tagValidator)
        {
            validator = tagValidator ?? throw new ArgumentNullException(nameof(tagValidator));
        }

        /// <summary>
        /// Parses the input, extracting tag ranges, actions, and returning clean text without markup.
        /// </summary>
        /// <param name="input">Raw input string containing tags and actions.</param>
        /// <returns>A <see cref="ParseResult"/> containing ranges, actions, and cleaned text.</returns>
        /// <exception cref="ArgumentException">If <paramref name="input"/> is null or whitespace.</exception>
        public ParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            // Collect events
            var events = new List<ParserEvent>();
            foreach (Match m in TagPattern.Matches(input))
            {
                bool closing = m.Groups[1].Value == "/";
                string name = m.Groups[2].Value;
                string value = m.Groups[3].Success ? m.Groups[3].Value : null;
                if (!validator.Validate(name, value))
                    continue;
                events.Add(new ParserEvent(m.Index, m.Length, true, closing, m.Value, name, value));
            }
            foreach (Match m in ActionPattern.Matches(input))
            {
                string name = m.Groups[1].Value;
                string value = m.Groups[2].Success ? m.Groups[2].Value : null;
                events.Add(new ParserEvent(m.Index, m.Length, false, false, m.Value, name, value));
            }
            events.Sort((a, b) => a.Index - b.Index);

            // Prepare output lists
            var ranges = new List<TagRange>();
            var actions = new List<TokenData>();

            var stacks = new Dictionary<string, Stack<TokenData>>();
            int cleanOffset = 0;
            int rangeStart = 0;

            void EmitRange(int end)
            {
                if (stacks.Count == 0 || end <= rangeStart)
                    return;

                var currentTags = stacks.Values.Select(s => s.Peek()).ToList();
                ranges.Add(new TagRange(rangeStart, end - 1, currentTags));
            }

            // Process events
            foreach (var ev in events)
            {
                int pos = ev.Index - cleanOffset;
                if (ev.IsTag)
                {
                    EmitRange(pos);
                    if (ev.IsClosing)
                    {
                        if (stacks.TryGetValue(ev.Name, out var stack) && stack.Count > 0)
                        {
                            stack.Pop();
                            if (stack.Count == 0)
                                stacks.Remove(ev.Name);
                        }
                    }
                    else
                    {
                        if (!stacks.TryGetValue(ev.Name, out var stack))
                        {
                            stack = new Stack<TokenData>();
                            stacks[ev.Name] = stack;
                        }
                        stack.Push(new TokenData(ev.Raw, ev.Name, ev.Value, pos));
                    }
                    rangeStart = pos;
                }
                else
                {
                    actions.Add(new TokenData(ev.Raw, ev.Name, ev.Value, pos));
                }
                cleanOffset += ev.Length;
            }

            EmitRange(input.Length - cleanOffset);
            string noActions = ActionPattern.Replace(input, string.Empty);
            string clean = TagPattern.Replace(noActions, match =>
            {
                string nm = match.Groups[2].Value;
                string val = match.Groups[3].Success ? match.Groups[3].Value : null;
                return validator.Validate(nm, val) ? string.Empty : match.Value;
            });

            return new ParseResult(ranges, actions, clean);
        }
    }
}
