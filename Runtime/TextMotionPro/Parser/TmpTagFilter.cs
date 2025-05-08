using System.Collections.Generic;

namespace BP.TextMotion
{
    /// <summary>
    /// Provides functionality to detect and filter out tags reserved by TextMeshPro.
    /// </summary>
    public static class TmpTagFilter
    {
        private static readonly HashSet<string> ReservedTags = new()
        {
            "b", "i", "u", "s", "sub", "sup",
            "color", "size", "font", "align",
            "line-height", "voffset", "margin",
            "indent", "sprite", "space", "style",
            "rotate", "cspace", "mspace", "nobr",
            "link", "noparse"
        };


        /// <summary>
        /// Determines whether the specified tag name is reserved by TextMeshPro.
        /// </summary>
        /// <param name="tagName">The name of the tag to check.</param>
        /// <returns><c>true</c> if the tag is reserved; otherwise, <c>false</c>.</returns>
        public static bool IsReserved(string tagName)
        {
            return ReservedTags.Contains(tagName);
        }
    }
}
