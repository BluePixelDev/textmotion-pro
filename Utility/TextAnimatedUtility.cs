using System;
using TMPro;

namespace BP.TMPA
{
    /// <summary>
    /// Provides utility methods for managing vertex data and array operations in TextMeshProAnimated.
    /// </summary>
    public static class TextAnimatedUtility
    {
        /// <summary>
        /// Gets the current vertex data update flags.
        /// </summary>
        public static TMP_VertexDataUpdateFlags UpdateFlags { get; private set; } = TMP_VertexDataUpdateFlags.None;

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
        /// Copies the contents of one array to another, resizing the target array if necessary.
        /// </summary>
        /// <typeparam name="T">The type of array elements.</typeparam>
        /// <param name="source">The source array to copy from.</param>
        /// <param name="target">The target array to copy into (passed by reference to allow resizing).</param>
        public static void CopyArrayContents<T>(T[] source, ref T[] target)
        {
            if (target.Length < source.Length)
                Array.Resize(ref target, source.Length);

            Array.Copy(source, target, target.Length);
        }

        /// <summary>
        /// Updates the mesh information for a TextMesh Pro component using cached mesh data.
        /// </summary>
        /// <param name="text">The TextMesh Pro component to update.</param>
        /// <param name="cachedMeshInfo">The cached mesh information to apply.</param>
        public static void UpdateMeshInfo(TMP_Text text, ref TMP_MeshInfo[] cachedMeshInfo)
        {
            if (cachedMeshInfo == null) return;
            TMP_TextInfo textInfo = text.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var originalMeshInfo = cachedMeshInfo[i];
                var meshInfo = textInfo.meshInfo[i];
                CopyArrayContents(originalMeshInfo.uvs0, ref meshInfo.uvs0);
                CopyArrayContents(originalMeshInfo.uvs2, ref meshInfo.uvs2);
                CopyArrayContents(originalMeshInfo.vertices, ref meshInfo.vertices);
                CopyArrayContents(originalMeshInfo.colors32, ref meshInfo.colors32);
            }
        }
    }
}