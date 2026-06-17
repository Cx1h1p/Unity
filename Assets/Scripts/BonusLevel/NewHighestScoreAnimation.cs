using System.Collections;
using UnityEngine;

public class NewHighScoreAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("UI SFX")]
    [SerializeField] private AudioSource uiSfxAudioSource;
    [SerializeField] private AudioClip whooshClip;

    [Range(0f, 1f)]
    [SerializeField] private float whooshVolume = 1f;

    [Header("Timing")]
    [SerializeField] private float popupDuration = 0.5f;
    [SerializeField] private float stayDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.35f;

    [Header("Animation")]
    [SerializeField] private Vector3 startScale = new Vector3(0.15f, 0.15f, 1f);
    [SerializeField] private Vector3 finalScale = Vector3.one;
    [SerializeField] private float spinDegrees = 720f;

    private Coroutine currentRoutine;

    public IEnumerator PlayAndWait()
    {
        gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        yield return currentRoutine = StartCoroutine(PlayRoutine());
    }

    public void Play()
    {
        gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        canvasGroup.alpha = 0f;
        rectTransform.localScale = startScale;
        rectTransform.localRotation = Quaternion.identity;

        if (uiSfxAudioSource != null && whooshClip != null)
        {
            uiSfxAudioSource.PlayOneShot(whooshClip, whooshVolume);
        }

        float timer = 0f;

        while (timer < popupDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / popupDuration);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            rectTransform.localScale = Vector3.Lerp(startScale, finalScale, t);

            float zRotation = Mathf.Lerp(0f, spinDegrees, t);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = finalScale;
        rectTransform.localRotation = Quaternion.identity;

        yield return new WaitForSecondsRealtime(stayDuration);

        timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fadeOutDuration);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}