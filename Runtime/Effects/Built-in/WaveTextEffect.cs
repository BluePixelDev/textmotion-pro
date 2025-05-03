namespace BP.TextMotionPro
{
    using TMPro;
    using UnityEngine;

    namespace BP.TMPA
    {
        [TextEffect("Wave", "Makes the text move up and down using sine wave")]
        public class WaveTextEffect : TextEffect
        {
            [SerializeField] private float frequency = 6f;
            [SerializeField] private float amplitude = 3f;
            [SerializeField] private float offset = 20f;

            public override string EffectTag => "wave";
            public override bool ValidateTag(string tag, string attributes) => true;

            public override void ApplyEffect(MotionRenderContext context)
            {
                var text = context.Renderer.TextComponent;

                // Get the current character info
                var characterInfo = text.textInfo.characterInfo[context.CharacterIndex];
                int materialIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;

                // Calculate the sine wave offset based on the animation time
                float funcOffset = context.AnimationTime * frequency + context.CharacterIndex * Mathf.Deg2Rad * offset;
                float waveOffset = Mathf.Sin(funcOffset) * amplitude;  // Sine wave effect

                // Modify the character's vertex positions based on the sine wave
                Vector3[] vertexPositions = text.textInfo.meshInfo[materialIndex].vertices;
                vertexPositions[vertexIndex + 0] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 1] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 2] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 3] += new Vector3(0f, waveOffset, 0f);

                // Mark the mesh as needing an update
                MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.All);
            }
        }
    }

}
