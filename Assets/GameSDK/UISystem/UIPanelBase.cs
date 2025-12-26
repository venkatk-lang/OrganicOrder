using UnityEngine;
using System.Collections;
namespace IACGGames.UISystem
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        [Header("Animation Settings")]
        public float fadeDuration = 0.25f;
        private CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        protected virtual void OnDisable()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        public virtual void Show(float animTime = 0)
        {
            fadeDuration = animTime;
            gameObject.SetActive(true);
        }

        public virtual void Hide(float animTime = 0)
        {
            fadeDuration = animTime;
            if (gameObject.activeSelf)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOut());
            }

        }

        private IEnumerator FadeIn()
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = t / fadeDuration;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private IEnumerator FadeOut()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            float t = 0f;
            float start = canvasGroup.alpha;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, 0f, t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}