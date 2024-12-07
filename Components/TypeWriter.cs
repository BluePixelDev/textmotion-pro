using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BP.TMPA
{
    [AddComponentMenu("Mesh/TMPA/TextMeshPro - Type Writer")]
    public class TypeWriter : MonoBehaviour
    {
        [Tooltip("The default rate of characters appearing per second (1s / writeRate)")]
        [SerializeField] private float readRate = 10;
        private TMP_Text m_TextComponent;

        private float currentReadRate;
        private Coroutine readerCoroutine;

        public event UnityAction<string> ReadStart;
        public event UnityAction ReadComplete;

        private void OnEnable()
        {
            TryGetComponent(out m_TextComponent);
            ResetValues();
            Read();
        }
        public void Read()
        {
            readerCoroutine = StartCoroutine(Reader());
        }
        public void Complete()
        {
            StopCoroutine(readerCoroutine);
            ResetValues();
            ReadComplete?.Invoke();
        }

        public void ResetValues()
        {
            currentReadRate = readRate;
            m_TextComponent.maxVisibleCharacters = int.MaxValue;
        }

        private IEnumerator Reader()
        {
            m_TextComponent.maxVisibleCharacters = 0;
            int visibleCounter = 0;
            TMP_TextInfo textInfo = m_TextComponent.textInfo;

            while (visibleCounter < textInfo.characterCount)
            {
                yield return new WaitForSeconds(1 / currentReadRate);
                visibleCounter++;
                m_TextComponent.maxVisibleCharacters = visibleCounter;
            }

            ReadComplete?.Invoke();
            ResetValues();
        }
    }
}