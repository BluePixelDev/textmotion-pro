using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// <see cref="TextMotionPro"/> manages animated text effects on a <see cref="TMP_Text"/> component.
    /// </summary>
    [AddComponentMenu("MotionPro/Text Motion Pro")]
    [ExecuteAlways, RequireComponent(typeof(TMP_Text)), DisallowMultipleComponent]
    public class TextMotionPro : MonoBehaviour
    {
        [SerializeField] private MotionProfile effectsProfile;
        [SerializeField] private bool runInEditor = true;
        [SerializeField, Range(4, 120)] private float frameRate = 24f;
        [SerializeField] private Vector2Int visibilityRange;

        private TMP_Text textComponent;
        private MotionProfile prevProfile;
        private MotionPreprocessor preprocessor;

        private TMP_MeshInfo[] cachedMeshInfo;
        private float animationTime = 0;
        private float elapsedTime = 0;
        private float lastUpdateTime = 0f;

        private readonly MotionRenderContext renderContext = new();
        private readonly Dictionary<int, CharacterData> characterData = new();

        /// <summary>
        /// Gets the TMP_Text component attached to this GameObject.
        /// </summary>
        public TMP_Text TextComponent
        {
            get
            {
                if (textComponent == null)
                {
                    TryGetComponent(out textComponent);
                }
                if (textComponent == null)
                {
                    Debug.LogError("TextMeshPro component missing! Please attach a TMP_Text component to this GameObject.");
                    enabled = false;
                }
                return textComponent;
            }
        }

        /// <summary>
        /// The currently used TextMotion profile.
        /// </summary>
        public MotionProfile Profile => effectsProfile;

        /// <summary>
        /// Gets the preprocessor instance, initializing it if necessary.
        /// </summary>
        private MotionPreprocessor PreProcessor => preprocessor ??= CreatePreProcessor();
        private MotionPreprocessor CreatePreProcessor()
        {
            var validator = new MotionValidator(this);
            var parser = new MotionParser(validator);
            return new MotionPreprocessor(parser);
        }

        private void OnEnable()
        {
            animationTime = 0;
            PreProcessor.ClearCache();
            TextComponent.textPreprocessor = PreProcessor;
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChange);

            TextComponent.ForceMeshUpdate(true);
            CacheMeshInfo();
            RenderTextEffects();

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
#endif
        }
        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChange);
            TextComponent.textPreprocessor = null;
            TextComponent.ForceMeshUpdate(true, true);

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void OnValidate()
        {
            if (prevProfile != effectsProfile)
            {
                prevProfile = effectsProfile;
                TextComponent.ForceMeshUpdate(true, true);
            }
        }

        /// <summary>
        /// Updates the animation each frame.
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
                return;

            MotionUpdate();
        }

        /// <summary>
        /// Called by the Editor to update the component when it is selected.
        /// </summary>
        private void EditorUpdate()
        {
            if (!runInEditor) return;
            if (Application.isPlaying) return;

#if UNITY_EDITOR
            if (!Selection.gameObjects.Contains(gameObject))
                return;

            MotionUpdate();
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        /// <summary>
        /// Called when the text changes.
        /// Updates the cached mesh info and re-renders text effects.
        /// </summary>
        /// <param name="obj">The object that changed.</param>
        private void OnTextChange(Object obj)
        {
            if (obj != TextComponent) return;
            CacheMeshInfo();
            RenderTextEffects();
        }

        private CharacterData GetCharacterDataOrAdd(int index)
        {
            if (!characterData.TryGetValue(index, out var data))
            {
                data = new CharacterData(index);
                characterData[index] = data;
            }

            return data;
        }
        private void UpdateCharacterVisibility(int index)
        {
            bool isInRange = index >= visibilityRange.x && index <= visibilityRange.y;
            var charData = GetCharacterDataOrAdd(index);
            if (!isInRange)
            {
                if (charData.isVisible)
                {
                    charData.isVisible = false;
                    charData.hiddenStartTime = animationTime;
                }
            }
            else
            {
                if (!charData.isVisible)
                {
                    charData.isVisible = true;
                    charData.visibleStartTime = animationTime;
                }
            }

        }


        /// <summary>
        /// Advances the animation based on the set frame rate.
        /// Consolidates time calculations and triggers a re-render when needed.
        /// </summary>
        private void MotionUpdate()
        {
            if (TextComponent == null || effectsProfile == null)
                return;

            elapsedTime += Time.deltaTime;
            float targetUpdateInterval = 1f / frameRate;
            float timeSinceLastUpdate = elapsedTime - lastUpdateTime;

            if (timeSinceLastUpdate >= targetUpdateInterval)
            {
                int updateCount = Mathf.FloorToInt(timeSinceLastUpdate / targetUpdateInterval);
                animationTime += updateCount * targetUpdateInterval;
                RenderTextEffects();
                lastUpdateTime = elapsedTime;
            }
        }

        /// <summary>
        /// Iterates over all characters in the text and applies active text effects.
        /// Tracks the time each character has been visible to drive effect animations.
        /// </summary>
        private void RenderTextEffects()
        {
            if (TextComponent == null || effectsProfile == null)
                return;

            // If there is no text, clear the visibility timing data.
            if (TextComponent.textInfo.characterCount == 0)
            {
                characterData.Clear();
                return;
            }

            effectsProfile.ResetContext(this);
            var charInfo = TextComponent.textInfo.characterInfo;
            for (int i = 0; i < charInfo.Length; i++)
            {
                ref TMP_CharacterInfo character = ref charInfo[i];

                //Skips character insvisible from TextMeshPro
                if (!character.isVisible)
                    continue;

                UpdateCharacterVisibility(character.index);

                if (characterData.TryGetValue(character.index, out var characterMotionData))
                {
                    if (!characterMotionData.isVisible)
                    {
                        return;
                    }
                }

                var tags = PreProcessor.GetTagEffectsAtIndex(character.index);
                if (tags == null)
                    continue;

                // Applies each tag in the current range
                foreach (var tag in tags)
                {
                    if (!effectsProfile.TryGetTextEffectWithTag(tag.Name, out var textEffect))
                        continue;

                    renderContext.Reset(
                        this,
                        tag,
                        characterData[character.index],
                        i,
                        animationTime
                    );
                    textEffect.ApplyEffect(renderContext);
                }
            }

            TextComponent.UpdateVertexData(MotionRenderFlags.Pop());
            MotionUtility.UpdateMeshInfo(TextComponent, ref cachedMeshInfo);
        }

        /// <summary>
        /// Caches the current mesh info from the TMP_Text component.
        /// Clears character timing data if there is no text.
        /// </summary>
        private void CacheMeshInfo() => cachedMeshInfo = TextComponent.textInfo.CopyMeshInfoVertexData();
    }
}
