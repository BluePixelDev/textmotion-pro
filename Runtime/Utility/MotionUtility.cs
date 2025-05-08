using System;
using TMPro;

namespace BP.TextMotion
{
    /// <summary>
    /// Provides utility methods for managing vertex data and array operations.
    /// </summary>
    public static class MotionUtility
    {
        /// <summary>
        /// Updates the mesh information for a TextMesh Pro component using cached mesh data.
        /// </summary>
        /// <param name="text">The TextMesh Pro component to update.</param>
        /// <param name="cachedMeshInfo">The cached mesh information to apply.</param>
        public static void UpdateMeshInfo(TMP_Text text, ref TMP_MeshInfo[] cachedMeshInfo)
        {
            if (cachedMeshInfo == null) return;
            TMP_TextInfo textInfo = text.textInfo;
            CopyMeshInfo(cachedMeshInfo, ref textInfo.meshInfo);
        }

        public static void CopyMeshInfo(TMP_MeshInfo[] src, ref TMP_MeshInfo[] dst)
        {
            for (int i = 0; i < src.Length; i++)
            {
                ref var srcMeshInfo = ref src[i];
                ref var dstMeshInfo = ref dst[i];
                CopyResizeArray(srcMeshInfo.uvs0, ref dstMeshInfo.uvs0);
                CopyResizeArray(srcMeshInfo.uvs2, ref dstMeshInfo.uvs2);
                CopyResizeArray(srcMeshInfo.vertices, ref dstMeshInfo.vertices);
                CopyResizeArray(srcMeshInfo.colors32, ref dstMeshInfo.colors32);
            }
        }

        /// <summary>
        /// Copies the contents of one array to another, resizing the target array if necessary.
        /// </summary>
        /// <typeparam name="T">The type of array elements.</typeparam>
        /// <param name="src">The source array to copy from.</param>
        /// <param name="dst">The destination array to copy into (passed by reference to allow resizing).</param>
        public static void CopyResizeArray<T>(T[] src, ref T[] dst)
        {
            if (dst == null || dst.Length < src.Length)
                Array.Resize(ref dst, src.Length);

            Array.Copy(src, dst, src.Length);
        }
    }
}