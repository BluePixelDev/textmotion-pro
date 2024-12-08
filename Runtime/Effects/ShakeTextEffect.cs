using TMPro;
using UnityEngine;

namespace BP.TMPA
{
    [TextEffect("Shake", "Makes the text shake")]
    public class ShakeTextEffect : TextEffectBase
    {
        [SerializeField] private float shakeAmount = 2f;
        [SerializeField] private float shakeSpeed = 10f;
        [SerializeField] private float randomShakeInterval = 0.1f;

        private float timeOffset = 0f;

        public override string EffectTag => "shake";
        public override bool ValidateTagAttributes(string tag, string attributes) => true;
        public override void ApplyEffect()
        {
            if (!Application.isPlaying) return;
            var characterInfo = Text.textInfo.characterInfo[CharacterIndex];
            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            timeOffset += Context.AnimationTime * shakeSpeed;

            if (timeOffset < randomShakeInterval) return;
            timeOffset = 0f;

            // Generate random shake offset instead of using Perlin noise
            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeY = Random.Range(-shakeAmount, shakeAmount);

            Vector3[] vertexPositions = Text.textInfo.meshInfo[materialIndex].vertices;
            vertexPositions[vertexIndex + 0] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 1] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 2] += new Vector3(shakeX, shakeY, 0f);
            vertexPositions[vertexIndex + 3] += new Vector3(shakeX, shakeY, 0f);
            TextRenderUtil.AddUpdateFlags(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}
