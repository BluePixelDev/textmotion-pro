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
        [SerializeField] private TextMeshProAnimated proAnimated;

        private float currentReadRate;
        private Coroutine readerCoroutine;

        public event UnityAction<string> ReadStart;
        public event UnityAction ReadComplete;

        private void OnEnable()
        {
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
            proAnimated.MaxVisibleCharacters = int.MaxValue;
        }

        private IEnumerator Reader()
        {
            proAnimated.MaxVisibleCharacters = 0;
            int visibleCounter = 0;
            TMP_TextInfo textInfo = proAnimated.TextComponent.textInfo;

            while (visibleCounter < textInfo.characterCount)
            {
                yield return new WaitForSeconds(1 / currentReadRate);
                visibleCounter++;
                proAnimated.MaxVisibleCharacters = visibleCounter;
            }

            ReadComplete?.Invoke();
            ResetValues();
        }
    }
}