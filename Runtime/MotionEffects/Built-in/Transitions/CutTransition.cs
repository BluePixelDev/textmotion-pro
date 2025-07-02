using TMPro;
using UnityEngine;

namespace BP.TextMotion
{
    [TextMotion("Cut", TextMotionRole.Transition)]
    public class CutTransition : TextTransition
    {
        public override string Key => "cut";
        public override void Apply(TransitionContext context)
        {
            var text = context.TextMotion.TextComponent;
            var charInfo = text.textInfo.characterInfo[context.CharacterIndex];

            if (!charInfo.isVisible) return;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = text.textInfo.meshInfo[materialIndex].vertices;
            bool isVisible = context.Phase == TransitionPhase.In;

            if (!isVisible)
            {
                Vector3 collapsePoint = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) * 0.5f;
                vertices[vertexIndex + 0] = collapsePoint;
                vertices[vertexIndex + 1] = collapsePoint;
                vertices[vertexIndex + 2] = collapsePoint;
                vertices[vertexIndex + 3] = collapsePoint;
            }

            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}
