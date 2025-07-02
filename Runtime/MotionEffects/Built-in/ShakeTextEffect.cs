using TMPro;
using UnityEngine;

namespace BP.TextMotion
{
    [TextMotion("Shake", TextMotionRole.Effect, "Makes the text shake")]
    public class ShakeTextEffect : TagEffect
    {
        [SerializeField] private float shakeAmount = 2f;
        [SerializeField] private float randomShakeInterval = 0.1f;

        public override string Key => "shake";
        public override bool IsActive() => true;
        public override bool ValidateTag(string tag, string attributes) => true;
        public override void Apply(TagEffectContext context)
        {
            var text = context.TextMotion.TextComponent;
            var index = context.CharacterIndex;

            var characterInfo = text.textInfo.characterInfo[index];
            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            if (context.AnimationTime % randomShakeInterval > randomShakeInterval * 0.5f) return;

            // Generate random shake offset instead of using Perlin noise
            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeY = Random.Range(-shakeAmount, shakeAmount);

            Vector3[] vertexPositions = text.textInfo.meshInfo[materialIndex].vertices;
            vertexPositions[vertexIndex + 0] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 1] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 2] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 3] += new Vector3(shakeX, shakeY, 0f);
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}
