using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace BP.TMPA
{
    internal class TextMeshPreprocessor : ITextPreprocessor, IDisposable
    {
        // Caching mechanisms
        private string lastInputText;
        private string processedText;
        private readonly TagValidator tagValidator;

        public TextMeshPreprocessor(TagValidator tagValidator)
        {
            this.tagValidator = tagValidator;
        }

        public string PreprocessText(string inputText)
        {
            // Checks if the last processed text really changed
            if (inputText != lastInputText)
            {
                processedText = TextParser.RemoveCustomTags(inputText, tagValidator);
                lastInputText = inputText;
            }

            return processedText;
        }

        public List<TextTagData> GetTagEffectsAtIndex(int index)
        {
            return Array.Empty<TextTagData>().ToList();
        }

        public void ClearCache()
        {
            lastInputText = string.Empty;
            processedText = string.Empty;
        }

        public void Dispose()
        {

        }
    }
}
