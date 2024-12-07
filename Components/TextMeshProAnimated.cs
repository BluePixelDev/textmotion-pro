using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BP.TMPA
{
    [ExecuteAlways, RequireComponent(typeof(TMP_Text)), DisallowMultipleComponent]
    public class TextMeshProAnimated : MonoBehaviour
    {
        [SerializeField] private TextEffectsProfile effectsProfile;
        [SerializeField] private int maxVisibleCharacters;
        [SerializeField] private float updateRate = 24f;

        // Improved character visibility tracking
        private class CharacterVisibilityInfo
        {
            public float StartTime;
            public int CharacterIndex;
        }

        // Use a more efficient data structure for character visibility
        private readonly Dictionary<int, CharacterDataEntry> characterVisibilityMap = new();

        private TextEffectsProfile _prevProfile;
        private TMP_Text _textComponent;
        private TextMeshPreprocessor _preprocessor;
        private TMP_MeshInfo[] _cachedMeshInfo;

        private bool isDirty = false;
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
        private bool TagValidator(string tag, string attributes)
        {
            if (!effectsProfile) return false;
            return effectsProfile.HasTextEffectWithTag(tag)
                && effectsProfile.GetTextEffectWithTag(tag).ValidateTagAttributes(tag, attributes);
        }

        /// <summary>
        /// Gets the start time of a specific character's visibility
        /// </summary>
        /// <param name="characterIndex">The character index</param>
        /// <returns>The start time of character visibility, or -1 if not found</returns>
        public float GetCharacterVisibilityStartTime(int characterIndex)
        {
            return characterVisibilityMap.TryGetValue(characterIndex, out var data)
                ? data.FirstVisibleTime
                : -1f;
        }

        // ==== UNITY METHODS ====
        private void OnEnable()
        {
            PreProcessor.ClearCache();
            characterVisibilityMap.Clear();

            TextComponent.textPreprocessor = PreProcessor;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TextChangeEvent);

            TextComponent.ForceMeshUpdate(true, true);
            isDirty = true;
            CharacterUpdate();
        }
        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TextChangeEvent);
            TextComponent.textPreprocessor = null;
        }
        private void OnValidate()
        {
            // Forces mesh update if the profile changes
            if (_prevProfile != effectsProfile)
            {
                _prevProfile = effectsProfile;
                isDirty = true;
                TextComponent.ForceMeshUpdate(true, true);
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
                CharacterUpdate();
            }
        }
        private void OnDestroy()
        {
            PreProcessor.Dispose();
        }

        // ==== CALLBACKS ====
        private void TextChangeEvent(Object obj)
        {
            if (obj == TextComponent)
            {
                isDirty = true;
                CharacterUpdate();
            }
        }

        // ==== UPDATE LOOP ====
        private void CharacterUpdate()
        {
            if (!effectsProfile || !TextComponent) return;

            if (string.IsNullOrEmpty(TextComponent.text))
            {
                TextComponent.ClearMesh();
                return;
            }

            DirtyUpdate();

            // Iterates over all characters
            var charInfo = TextComponent.textInfo.characterInfo;
            for (int i = 0; i < charInfo.Length; i++)
            {
                ref var character = ref charInfo[i];

                // Skip invisible or zero-scaled characters
                if (!character.isVisible || character.scale == 0) continue;

                // Fetch tag effects that affect this indices, skip if none found.
                var tags = PreProcessor.GetTagEffectsAtIndex(character.index);
                if (tags == null) continue;

                for (int j = 0; j < tags.Count; j++)
                {
                    var tag = tags[j];
                    var textEffect = effectsProfile.GetTextEffectWithTag(tag.Name);
                    if (textEffect == null || !textEffect.IsActive()) continue;

                    TextRenderContext.Current.Reset(
                    tag,
                        characterVisibilityMap[character.index].FirstVisibleTime,
                        character.index,
                        animationTime,
                        this
                    );

                    textEffect.ApplyEffect();
                }
            }

            TextComponent.UpdateVertexData(TextAnimatedUtility.UpdateFlags);
            TextAnimatedUtility.ResetUpdateFlags();
            TextAnimatedUtility.UpdateMeshInfo(TextComponent, ref _cachedMeshInfo);
        }
        private void DirtyUpdate()
        {
            if (!isDirty) return;
            _cachedMeshInfo = TextComponent.textInfo.CopyMeshInfoVertexData();
            isDirty = false;
        }
    }
}