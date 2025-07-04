using TMPro;
using UnityEngine;

namespace BP.TextMotion
{
    [TextMotion("Rainbow", TextMotionRole.Effect, "Makes the text rainbowy")]
    public sealed class RainbowTextEffect : TagComponent
    {
        [SerializeField] private float test = 1;

        public sealed override string Key => "rainbow";
        public override bool IsActive() => true;
        public sealed override bool ValidateTag(string tag, string attributes) => true;
        public sealed override void Apply(MotionContext context)
        {
            var text = context.TextMotion.TextComponent;
            var textInfo = text.textInfo;
            var characterInfo = text.textInfo.characterInfo[context.CharacterIndex];

            // Early exit for invisible characters
            if (!characterInfo.isVisible) return;

            int meshIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            float time = context.AnimationTime;
            float offset = context.CharacterData.index * 0.1f;

            float cycleValue = Mathf.PingPong(time * 0.5f + offset * test, 1f);
            Color rainbowColor = Color.HSVToRGB(cycleValue, 1f, 1f);

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = rainbowColor;
            vertexColors[vertexIndex + 1] = rainbowColor;
            vertexColors[vertexIndex + 2] = rainbowColor;
            vertexColors[vertexIndex + 3] = rainbowColor;

            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
