using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace BP.TMPA
{
    [ExecuteAlways, RequireComponent(typeof(TMP_Text)), DisallowMultipleComponent]
    public class TextMeshProAnimated : MonoBehaviour
    {
        [SerializeField] private TextEffectsProfile effectsProfile;
        [SerializeField] private int _maxVisibleCharacters;
        [SerializeField] private float updateRate = 24f;

        private TextEffectsProfile _prevProfile;
        private TMP_Text _textComponent;
        private TextMeshPreprocessor _preprocessor;

        private TMP_MeshInfo[] _cachedMeshInfo;
        private TMP_MeshInfo[] _localMeshInfo;

        private float animationTime = 0;
        private int _prevMaxVisibleCharacters;

        public int MaxVisibleCharacters
        {
            get => _maxVisibleCharacters;
            set
            {
                _maxVisibleCharacters = value;
                if (!Application.isPlaying)
                {
                    RenderTextEffects();
                }
            }
        }

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
            _prevMaxVisibleCharacters = _maxVisibleCharacters;
            PreProcessor.ClearCache();
            TextComponent.textPreprocessor = PreProcessor;
            TextEventManager.TEXT_CHANGED_EVENT.Add(OnTextUpdate);

            // We forcibly update text mesh pro to get the data we need
            ForceUpdateRender();
        }
        private void OnDisable()
        {
            TextEventManager.TEXT_CHANGED_EVENT.Remove(OnTextUpdate);
            TextComponent.textPreprocessor = null;

            // We forcibly update text mesh pro to get the data we need
            ForceUpdateRender();
        }
        private void OnValidate()
        {
            // Forces mesh update if the profile changes
            if (_prevProfile != effectsProfile || _maxVisibleCharacters != _prevMaxVisibleCharacters)
            {
                _prevProfile = effectsProfile;
                _prevMaxVisibleCharacters = _maxVisibleCharacters;
                ForceUpdateRender();
            }
        }
        private void Update()
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
        private void OnDestroy() => PreProcessor.Dispose();

        private void OnTextUpdate(Object obj)
        {
            if (obj == TextComponent)
            {

            }
        }

        // ==== FORCE UPDATERS ====
        private void ForceUpdateRender()
        {
            if (TextComponent.textInfo.characterCount == 0) return;
            TextComponent.ForceMeshUpdate(true);
            UpdateMeshCache();  // Cache the current mesh data
            RenderTextEffects();
        }

        // ==== UPDATING CACHE ====
        private void UpdateMeshCache()
        {
            // Caching the newly generared mesh data
            if (TextComponent.textInfo.characterCount == 0) return;
            _cachedMeshInfo = TextComponent.textInfo.CopyMeshInfoVertexData();
        }

        // ==== RENDERING ====
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
            int visibleCharacterCount = 0;
            for (int i = 0; i < charInfo.Length; i++)
            {
                ref var character = ref charInfo[i];
                if (!character.isVisible || character.scale == 0) continue;

                // Only count non-whitespace characters
                if (!char.IsWhiteSpace(character.character))
                {
                    // Check if we've exceeded max visible characters
                    bool isVisible = MaxVisibleCharacters < 0 || visibleCharacterCount < MaxVisibleCharacters;
                    ModifyCharacterVisibility(i, isVisible);
                    TextAnimatedUtility.AddUpdateFlags(TMP_VertexDataUpdateFlags.Colors32);
                    visibleCharacterCount++;
                }

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

            TextComponent.UpdateVertexData(TextAnimatedUtility.UpdateFlags);
            TextAnimatedUtility.ResetUpdateFlags();
            TextAnimatedUtility.UpdateMeshInfo(TextComponent, ref _cachedMeshInfo);
        }

        private void ModifyCharacterVisibility(int index, bool isVisible)
        {
            ref TMP_CharacterInfo characterInfo = ref TextComponent.textInfo.characterInfo[index];
            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            ref Color32[] tmpColors = ref TextComponent.textInfo.meshInfo[materialIndex].colors32;
            ref Color32[] cachedColors = ref _cachedMeshInfo[materialIndex].colors32;

            // Modify the color alpha to control visibility
            for (int j = 0; j < 4; j++)
            {
                int colorIndex = vertexIndex + j;
                byte targetAlpha = isVisible ? cachedColors[colorIndex].a : (byte)0;
                tmpColors[colorIndex].a = targetAlpha;
            }
        }
    }
}