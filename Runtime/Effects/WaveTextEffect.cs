namespace BP.TMPA
{
    using TMPro;
    using UnityEngine;

    namespace BP.TMPA
    {
        [TextEffect("Wave", "Makes the text move up and down using sine wave")]
        public class WaveTextEffect : TextEffectBase
        {
            [SerializeField] private float frequency = 6f;  // How fast the movement oscillates
            [SerializeField] private float amplitude = 3f;  // How much the character moves up and down
            [SerializeField] private float offset = 120f;

            public override string EffectTag => "wave";
            public override bool ValidateTagAttributes(string tag, string attributes) => true;

            public override void ApplyEffect()
            {
                if (!Application.isPlaying) return;

                // Get the current character info
                var characterInfo = Text.textInfo.characterInfo[CharacterIndex];
                int materialIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;

                // Calculate the sine wave offset based on the animation time
                float funcOffset = Context.AnimationTime * frequency + (CharacterIndex + 1) * 120;
                float waveOffset = Mathf.Sin(funcOffset) * amplitude;  // Sine wave effect

                // Modify the character's vertex positions based on the sine wave
                Vector3[] vertexPositions = Text.textInfo.meshInfo[materialIndex].vertices;
                vertexPositions[vertexIndex + 0] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 1] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 2] += new Vector3(0f, waveOffset, 0f);
                vertexPositions[vertexIndex + 3] += new Vector3(0f, waveOffset, 0f);

                // Mark the mesh as needing an update
                TextRenderUtil.AddUpdateFlags(TMP_VertexDataUpdateFlags.Vertices);
            }
        }
    }

}
