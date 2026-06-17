using UnityEngine;
using System.Collections;

public class PlayGamePanelOpener : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup panelGroup;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Sound")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip clickSound;

    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
    }

    public void OpenPanel()
    {
        PlayClick();
        StartFade(1f, true);
    }

    public void ClosePanel()
    {
        PlayClick();
        StartFade(0f, false);
    }

    private void PlayClick()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound, clickVolume);
        }
    }

    private void StartFade(float targetAlpha, bool open)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadePanel(targetAlpha, open));
    }

    private IEnumerator FadePanel(float targetAlpha, bool open)
    {
        float startAlpha = panelGroup.alpha;

        if (open)
        {
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            panelGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);

            yield return null;
        }

        panelGroup.alpha = targetAlpha;

        if (!open)
        {
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
    }
}