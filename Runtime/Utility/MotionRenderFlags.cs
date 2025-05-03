using TMPro;

namespace BP.TextMotionPro
{
    public static class MotionRenderFlags
    {
        /// <summary>
        /// Gets the current vertex data update flags.
        /// </summary>
        private static TMP_VertexDataUpdateFlags flags = TMP_VertexDataUpdateFlags.None;

        /// <summary>
        /// Adds additional update flags to the current set of flags.
        /// </summary>
        /// <param name="flags">Flags to add to the current update flags.</param>
        public static void Add(TMP_VertexDataUpdateFlags flags) => MotionRenderFlags.flags |= flags;

        /// <summary>
        /// Resets the update flags to None.
        /// </summary>
        internal static void Reset() => flags = TMP_VertexDataUpdateFlags.None;

        /// <summary>
        /// Checks if the update flags are not none.
        /// </summary>
        public static bool HasFlags => flags != TMP_VertexDataUpdateFlags.None;

        /// <summary>
        /// "Pops" the flags and resets them.
        /// </summary>
        /// <returns>The state of flags before being popped.</returns>
        internal static TMP_VertexDataUpdateFlags Pop()
        {
            var temp = flags;
            Reset();
            return temp;
        }
    }
}
