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
        [SerializeField] private MotionProfile profile;
        [SerializeField] private MotionUpdateMode updateMode;
        [SerializeField, Range(0, 100)] private float timeScale = 1f;
        [SerializeField] private float frameRate = 24f;
        [SerializeField] private Vector2Int visibilityRange;

        private TMP_Text textComponent;
        private MotionProfile prevProfile;
        private MotionPreprocessor preprocessor;

        private TMP_MeshInfo[] cachedMeshInfo;
        private float animationTime = 0;
        private float elapsedTime = 0;
        private float lastUpdateTime = 0f;

        private readonly TagEffectContext renderContext = new();
        private readonly Dictionary<int, CharacterData> characterData = new();

        /// <summary>
        /// Gets the <see cref="TMP_Text"/> component attached to this GameObject.
        /// Attempts to cache the reference on first access. Logs an error and disables the component if not found.
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
        /// Gets the active motion profile which defines how text effects are applied.
        /// </summary>
        public MotionProfile Profile => profile;

        public float TimeScale
        {
            get => timeScale;
            set => timeScale = Mathf.Clamp(value, 0f, 100f);
        }

        /// <summary>
        /// Gets the motion preprocessor instance, used to clean and parse input text before applying motion effects.
        /// Initializes it lazily if not already created.
        /// </summary>
        public MotionPreprocessor Preprocessor => preprocessor ??= CreatePreprocessor();
        private MotionPreprocessor CreatePreprocessor()
        {
            var validator = new MotionValidator(this);
            var parser = new MotionParser(validator);
            return new MotionPreprocessor(parser);
        }

        private void OnEnable()
        {
            animationTime = 0f;
            elapsedTime = 0f;
            lastUpdateTime = 0f;

            Preprocessor.ClearCache();
            TextComponent.textPreprocessor = Preprocessor;
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
            if (profile)
                profile.ComponentsChanged -= TextEffectsChanged;

            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChange);
            TextComponent.textPreprocessor = null;
            TextComponent.ForceMeshUpdate(true, true);

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }
        private void OnValidate()
        {
            if (profile)
            {
                profile.ComponentsChanged -= TextEffectsChanged;
                profile.ComponentsChanged += TextEffectsChanged;
            }

            if (prevProfile != profile)
            {
                prevProfile = profile;
                TextComponent.ForceMeshUpdate(true, true);
            }

            TimeScale = timeScale;
        }
        private void Update()
        {
            if (!Application.isPlaying)
                return;

            MotionUpdate();
        }
        private void TextEffectsChanged() => TextComponent.ForceMeshUpdate(true, true);
        private void OnTextChange(Object obj)
        {
            if (obj != TextComponent) return;
            CacheMeshInfo();
            RenderTextEffects();
        }

        // ==== INTERNAL LOOP ====
        private void EditorUpdate()
        {
            if (updateMode == MotionUpdateMode.Runtime) return;
            if (Application.isPlaying) return;

#if UNITY_EDITOR
            if (!Selection.gameObjects.Contains(gameObject) && updateMode == MotionUpdateMode.SelectedInEditor)
                return;

            MotionUpdate();
            EditorApplication.QueuePlayerLoopUpdate();
#endif
        }
        private void MotionUpdate()
        {
            if (TextComponent == null || profile == null)
                return;

            elapsedTime += Time.deltaTime;
            float targetUpdateInterval = 1f / frameRate;
            float timeSinceLastUpdate = elapsedTime - lastUpdateTime;

            if (timeSinceLastUpdate >= targetUpdateInterval)
            {
                int updateCount = Mathf.FloorToInt(timeSinceLastUpdate / targetUpdateInterval);
                animationTime += updateCount * targetUpdateInterval * timeScale;
                RenderTextEffects();
                lastUpdateTime = elapsedTime;
            }
        }
        private void RenderTextEffects()
        {
            if (TextComponent == null || profile == null)
                return;

            // If there is no text, clear the visibility timing data.
            if (TextComponent.textInfo.characterCount == 0)
            {
                characterData.Clear();
                return;
            }

            profile.ResetContext(this);
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

                var tags = Preprocessor.GetTagsAt(character.index);
                if (tags == null)
                    continue;

                // Applies each tag in the current range
                foreach (var tag in tags)
                {
                    if (!profile.TryGetTagEffect(tag.Name, out var textEffect))
                        continue;

                    renderContext.Reset(
                        this,
                        tag,
                        characterData[character.index],
                        i,
                        animationTime
                    );
                    textEffect.Apply(renderContext);
                }
            }

            TextComponent.UpdateVertexData(MotionRenderFlags.Pop());
            MotionUtility.UpdateMeshInfo(TextComponent, ref cachedMeshInfo);
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
        private void CacheMeshInfo() => cachedMeshInfo = TextComponent.textInfo.CopyMeshInfoVertexData();
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
    }
}
