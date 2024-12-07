using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace BP.TMPA
{
    [TextEffect("Rainbow", "Makes the text rainbowy")]
    public sealed class RainbowTextEffect : TextEffectBase
    {
        [SerializeField] private float test = 1;

        public sealed override string EffectTag => "rainbow";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override bool ValidateTagAttributes(string tag, string attributes) => true;
        public sealed override void ApplyEffect()
        {
            var textInfo = Text.textInfo;
            var characterInfo = Text.textInfo.characterInfo[CharacterIndex];

            // Early exit for invisible characters
            if (!characterInfo.isVisible) return;

            int meshIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            float time = Context.AnimationTime;

            int totalCharacterCount = textInfo.characterCount;
            float offset = (float)CharacterIndex / totalCharacterCount;

            float cycleValue = Mathf.PingPong(time * 0.5f + offset * test, 1f);
            Color rainbowColor = Color.HSVToRGB(cycleValue, 1f, 1f);

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = rainbowColor;
            vertexColors[vertexIndex + 1] = rainbowColor;
            vertexColors[vertexIndex + 2] = rainbowColor;
            vertexColors[vertexIndex + 3] = rainbowColor;

            TextAnimatedUtility.AddUpdateFlags(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
