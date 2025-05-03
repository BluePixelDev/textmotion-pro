using TMPro;
using UnityEngine;

namespace BP.TextMotionPro
{


    [TextEffect("LoopingAnimator", "Animates position, rotation, and scale in a loop using animation curves")]
    public class AnimatorTextEffect : TextEffect
    {
        public enum AnimatorFlow
        {
            Loop,
            PingPong
        }

        [SerializeField] private string tag;
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve xPos, yPos;
        [SerializeField] private Vector2 position;
        [SerializeField] private AnimationCurve xScale, yScale;
        [SerializeField] private Vector2 scale;
        [SerializeField] private AnimationCurve rotation;
        [SerializeField] private float angle;
        [SerializeField] private AnimatorFlow animatorFlow;

        public override string EffectTag => tag;
        public override bool IsActive() => !string.IsNullOrEmpty(tag);
        public override bool ValidateTag(string tag, string attributes) => true;
        public override void ApplyEffect(MotionRenderContext context)
        {
            var text = context.Renderer.TextComponent;
            var characterInfo = text.textInfo.characterInfo[context.CharacterIndex];
            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            float loopTime = animatorFlow switch
            {
                AnimatorFlow.Loop => GetLoopTime(context.AnimationTime),
                AnimatorFlow.PingPong => (1 + Mathf.Sin(context.AnimationTime / duration)) / 2,
                _ => 0
            };

            float normalizedTime = loopTime;
            float posX = xPos.Evaluate(normalizedTime) * position.x;
            float posY = yPos.Evaluate(normalizedTime) * position.y;

            float rotationAngle = rotation.Evaluate(normalizedTime) * angle;
            float scaleXValue = xScale.Evaluate(normalizedTime) * scale.x;
            float scaleYValue = yScale.Evaluate(normalizedTime) * scale.y;

            Vector3[] vertexPositions = text.textInfo.meshInfo[materialIndex].vertices;

            Vector3 positionOffset = new(posX, posY);
            vertexPositions[vertexIndex + 0] += positionOffset;
            vertexPositions[vertexIndex + 1] += positionOffset;
            vertexPositions[vertexIndex + 2] += positionOffset;
            vertexPositions[vertexIndex + 3] += positionOffset;

            Vector3 center =
                (vertexPositions[vertexIndex + 0] + vertexPositions[vertexIndex + 1] + vertexPositions[vertexIndex + 2] + vertexPositions[vertexIndex + 3]) / 4f;

            Vector3 resultScale = new(scaleXValue, scaleYValue);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotationAngle), resultScale);
            for (int i = 0; i < 4; i++)
            {
                vertexPositions[vertexIndex + i] = rotationMatrix.MultiplyPoint3x4(vertexPositions[vertexIndex + i] - center) + center;
            }

            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
        }

        private float GetLoopTime(float animationTime)
        {
            float time = animationTime / duration;
            float decimalTime = time - Mathf.Floor(time);
            return decimalTime;
        }
    }
}
