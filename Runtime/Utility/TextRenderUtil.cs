using TMPro;

namespace BP.TMPA
{
    public static class TextRenderUtil
    {
        /// <summary>
        /// Gets the current vertex data update flags.
        /// </summary>
        private static TMP_VertexDataUpdateFlags UpdateFlags = TMP_VertexDataUpdateFlags.None;

        /// <summary>
        /// Adds additional update flags to the current set of flags.
        /// </summary>
        /// <param name="flags">Flags to add to the current update flags.</param>
        public static void AddUpdateFlags(TMP_VertexDataUpdateFlags flags) => UpdateFlags |= flags;

        /// <summary>
        /// Resets the update flags to None.
        /// </summary>
        internal static void ResetUpdateFlags() => UpdateFlags = TMP_VertexDataUpdateFlags.None;

        /// <summary>
        /// "Pops" the flags and resets them.
        /// </summary>
        /// <returns>The state of flags before being popped.</returns>
        internal static TMP_VertexDataUpdateFlags PopFlags()
        {
            var temp = UpdateFlags;
            ResetUpdateFlags();
            return temp;
        }
    }
}
