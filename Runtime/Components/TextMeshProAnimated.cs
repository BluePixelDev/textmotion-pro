using TMPro;
using UnityEngine;

namespace BP.TMPA
{
    [ExecuteAlways, RequireComponent(typeof(TMP_Text)), DisallowMultipleComponent]
    public class TextMeshProAnimated : MonoBehaviour
    {
        [SerializeField] private TextEffectsProfile effectsProfile;
        [SerializeField] private float updateRate = 24f;

        private TextEffectsProfile _prevProfile;
        private TMP_Text _textComponent;
        private TextMeshPreprocessor _preprocessor;

        private TMP_MeshInfo[] _cachedMeshInfo;
        private float animationTime = 0;

        // ==== GETTERS ====
        public TMP_Text TextComponent
        {
            get
            {
                if (_textComponent == null)
                {
                    TryGetComponent(out _textComponent);
                }
                return _textComponent;
            }
        }
        private TextMeshPreprocessor PreProcessor
        {
            get
            {
                _preprocessor ??= new TextMeshPreprocessor(TagValidator);
                return _preprocessor;
            }
        }

        // ==== VALIDATOR ====
        private bool TagValidator(string tag, string attributes)
        {
            if (!effectsProfile) return false;
            return effectsProfile.HasTextEffectWithTag(tag)
                && effectsProfile.GetTextEffectWithTag(tag).ValidateTagAttributes(tag, attributes);
        }

        // ==== UNITY METHODS ====
        private void OnEnable()
        {
            // Clears andy cache from the pre processor
            PreProcessor.ClearCache();
            TextComponent.textPreprocessor = PreProcessor;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TextChange);
            // We forcibly update text mesh pro to get the data we need
            ForceTextRender();
        }
        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TextChange);
            TextComponent.textPreprocessor = null;
            // We forcibly update text mesh pro to get the data we need
            TextComponent.ForceMeshUpdate(true, true);
        }
        private void OnValidate()
        {
            // Forces mesh update if the profile changes
            if (_prevProfile != effectsProfile)
            {
                _prevProfile = effectsProfile;
                ForceTextRender();
            }
        }
        private void Update() => TextAnimatedUpdate();
        private void OnDestroy() => PreProcessor.Dispose();

        private void TextChange(Object obj)
        {
            CacheMeshInfo();
            RenderTextEffects();
        }

        // ==== FORCE UPDATERS ====
        private void ForceTextRender()
        {
            if (TextComponent.textInfo.characterCount == 0) return;
            TextComponent.ForceMeshUpdate(true);
            CacheMeshInfo();
            RenderTextEffects();
        }
        private void CacheMeshInfo()
        {
            // Caching the newly generared mesh data
            if (TextComponent.textInfo.characterCount == 0) return;
            _cachedMeshInfo = TextComponent.textInfo.CopyMeshInfoVertexData();
        }

        // ==== UPDATE / RENDER ====
        private void TextAnimatedUpdate()
        {
            // Updates timers, but only in playmode
            if (!Application.isPlaying || !effectsProfile) return;

            animationTime += Time.deltaTime;

            int targetFrameRate = Application.targetFrameRate > 0
            ? Application.targetFrameRate
            : 60;

            if (Time.frameCount % Mathf.CeilToInt(targetFrameRate / updateRate) == 0)
            {
                RenderTextEffects();
            }
        }
        private void RenderTextEffects()
        {
            if (!effectsProfile || !TextComponent) return;
            if (TextComponent.textInfo.characterCount == 0) return;
            if (string.IsNullOrEmpty(TextComponent.text))
            {
                TextComponent.ClearMesh();
                return;
            }

            // Iterates over all characters
            var charInfo = TextComponent.textInfo.characterInfo;
            for (int i = 0; i < charInfo.Length; i++)
            {
                ref var character = ref charInfo[i];
                if (!character.isVisible || character.scale == 0) continue;

                // Fetch tag effects that affect this indices, skip if none found.
                var tags = PreProcessor.GetTagEffectsAtIndex(character.index);
                if (tags == null) continue;

                for (int j = 0; j < tags.Count; j++)
                {
                    var tag = tags[j];
                    var textEffect = effectsProfile.GetTextEffectWithTag(tag.Name);
                    if (textEffect == null || !textEffect.IsActive()) continue;

                    TextRenderContext.Current.Reset(tag, 0, character.index, animationTime, this);
                    textEffect.ApplyEffect();
                }
            }

            ApplyTextEffects();
        }
        public void ApplyTextEffects()
        {
            TextComponent.UpdateVertexData(TextRenderUtil.PopFlags());
            TextAnimatedUtility.UpdateMeshInfo(TextComponent, ref _cachedMeshInfo);
        }
    }
}