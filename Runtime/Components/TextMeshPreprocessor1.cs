using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.Pool;

namespace BP.TMPA
{
    internal class TextMeshPreprocessorBackup : ITextPreprocessor, IDisposable
    {
        private const string tagPattern = @"<(\/?)([A-Za-z0-9]+)(=[^>]+)?(\/?)>";

        // Caching mechanisms
        private string lastInputText;
        private string processedText;

        // Object pooling for TagData
        private readonly ObjectPool<TextTagData> tagDataPool;
        private readonly ObjectPool<List<TextTagData>> tagListPool;

        public Dictionary<int, List<TextTagData>> CharacterTagEffects { get; } = new();
        private readonly Func<string, string, bool> tagValidator;

        // Reusable collections to reduce allocations
        private readonly Dictionary<string, Stack<TextTagData>> activeStacks = new(16);

        public TextMeshPreprocessorBackup(Func<string, string, bool> tagValidator)
        {
            this.tagValidator = tagValidator ?? throw new ArgumentNullException(nameof(tagValidator));

            tagDataPool = new ObjectPool<TextTagData>(
                createFunc: () => new TextTagData(),
                actionOnGet: (tag) => tag.Reset(),
                actionOnRelease: (tag) => tag.Clear(),
                defaultCapacity: 64
            );

            tagListPool = new ObjectPool<List<TextTagData>>(
                createFunc: () => new List<TextTagData>(),
                actionOnGet: (list) => list.Clear(),
                defaultCapacity: 64
            );
        }

        public string PreprocessText(string inputText)
        {
            // Checks if the last processed text really changed
            if (inputText != lastInputText)
            {
                processedText = ProcessText(inputText);
                lastInputText = inputText;
            }

            return processedText;
        }

        private string ProcessText(string input)
        {
            ReleaseResources();
            CharacterTagEffects.Clear();
            activeStacks.Clear();

            string processedText = input;
            var matches = Regex.Matches(input, tagPattern);
            int charIndex = 0;

            foreach (Match match in matches.Cast<Match>())
            {
                // Tag properties
                string tag = match.Value;
                bool isClosingTag = match.Groups[1].Value == "/";
                string tagName = match.Groups[2].Value;
                string attributes = match.Groups[3].Value;
                int tagIndex = match.Index - charIndex;

                if (tagValidator(tagName, attributes))
                {
                    if (isClosingTag)
                    {
                        if (!activeStacks.TryGetValue(tagName, out var activeStack))
                            continue;

                        var matchingOpenTag = activeStack.Pop();
                        if (matchingOpenTag != null)
                        {
                            matchingOpenTag.Close(tagIndex);
                            for (int i = matchingOpenTag.StartIndex; i < matchingOpenTag.EndIndex; i++)
                            {
                                // Use pooled list for character tag effects
                                if (!CharacterTagEffects.TryGetValue(i, out var tagList))
                                {
                                    tagList = tagListPool.Get();
                                    CharacterTagEffects[i] = tagList;
                                }

                                // Only add the innermost tag at each position
                                if (!tagList.Any(t => t.Name == matchingOpenTag.Name))
                                    tagList.Add(matchingOpenTag);
                            }
                        }
                    }
                    else
                    {
                        var tagData = tagDataPool.Get();
                        tagData.Initialize(tag, tagName, attributes, tagIndex, int.MaxValue);
                        AddTagToStack(tagName, tagData);
                    }

                    // Removes tag from display and offsets the range
                    processedText = processedText.Replace(tag, string.Empty);
                    charIndex += tag.Length;
                }
            }

            foreach (var stack in activeStacks)
            {
                foreach (var tag in stack.Value)
                {
                    for (int i = tag.StartIndex; i < processedText.Length; i++)
                    {
                        // Use pooled list for character tag effects
                        if (!CharacterTagEffects.TryGetValue(i, out var tagList))
                        {
                            tagList = tagListPool.Get();
                            CharacterTagEffects[i] = tagList;
                        }

                        if (!tagList.Any(t => t.Name == tag.Name))
                            tagList.Add(tag);
                    }
                }
            }

            return processedText;
        }

        private void AddTagToStack(string name, TextTagData data)
        {
            if (activeStacks.TryGetValue(name, out var stack))
            {
                if (stack.Peek().RawTag != data.RawTag)
                    stack.Push(data);
            }
            else
            {
                var newStack = new Stack<TextTagData>();
                newStack.Push(data);
                activeStacks.Add(name, newStack);
            }
        }

        /// <summary>
        /// Releases all tag lists back to the pool.
        /// </summary>
        private void ReleaseResources()
        {
            // Release all the tag lists before rebuilding them
            foreach (var tagList in CharacterTagEffects.Values)
            {
                if (tagList != null)
                {
                    tagListPool.Release(tagList);
                }
            }

            // Clear the dictionary as well
            CharacterTagEffects.Clear();
        }

        public List<TextTagData> GetTagEffectsAtIndex(int index)
        {
            CharacterTagEffects.TryGetValue(index, out var tags);
            return tags;
        }

        public void Dispose()
        {
            tagDataPool.Clear();
            tagListPool.Clear();
            CharacterTagEffects.Clear();
        }

        public void ClearCache()
        {
            lastInputText = string.Empty;
            processedText = string.Empty;
        }
    }
}
