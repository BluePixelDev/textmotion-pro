using TMPro;
using UnityEngine;

namespace BP.TextMotion
{
    [TextEffect("PopIn", "Makes the text 'pop' in from a small size")]
    public class PopInTextEffect : TextEffect
    {
        [SerializeField] private float popInDuration = 0.5f;

        public override string Tag => "popin";
        public override bool ValidateTag(string tag, string attributes) => true;
        public override void ApplyEffect(MotionRenderContext context)
        {
            var text = context.TextMotion.TextComponent;

            var characterInfo = text.textInfo.characterInfo[context.CharacterIndex];
            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            float elapsedTime = context.AnimationTime - context.CharacterData.visibleStartTime;
            float scaleTime = elapsedTime / popInDuration;
            float scaleFactor = Mathf.Lerp(0, 1, scaleTime);

            // Return early if the scale is already one
            if (scaleFactor >= 1) return;

            Vector3[] vertexPositions = text.textInfo.meshInfo[materialIndex].vertices;
            Vector3 offset = (vertexPositions[vertexIndex + 0] + vertexPositions[vertexIndex + 2]) / 2;

            vertexPositions[vertexIndex + 0] -= offset;
            vertexPositions[vertexIndex + 1] -= offset;
            vertexPositions[vertexIndex + 2] -= offset;
            vertexPositions[vertexIndex + 3] -= offset;

            float scale = TextEasings.EaseOutBack(scaleFactor);
            Matrix4x4 matrix = Matrix4x4.Scale(scale * Vector3.one);
            vertexPositions[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertexPositions[vertexIndex + 0]);
            vertexPositions[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertexPositions[vertexIndex + 1]);
            vertexPositions[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertexPositions[vertexIndex + 2]);
            vertexPositions[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertexPositions[vertexIndex + 3]);

            vertexPositions[vertexIndex + 0] += offset;
            vertexPositions[vertexIndex + 1] += offset;
            vertexPositions[vertexIndex + 2] += offset;
            vertexPositions[vertexIndex + 3] += offset;

            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}