using UnityEngine;
using System.Collections;

public class DimmerFeedback : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UnityEngine.UI.Image image;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private float displayTime = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wrongClip;
    [SerializeField] private AudioClip correctClip;
    [Range(0f, 1f)][SerializeField] private float volume = 0.8f;

    private Coroutine currentRoutine;

    public void ShowWrong()
    {
        PlaySound(wrongClip);
        ShowColor(new Color(1f, 0.4f, 0.4f, 0.25f));
    }

    public void ShowCorrect()
    {
        PlaySound(correctClip);
        ShowColor(new Color(0.4f, 1f, 0.4f, 0.22f));
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    void ShowColor(Color color)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FlashRoutine(color));
    }

    IEnumerator FlashRoutine(Color color)
    {
        image.color = color;

        float targetAlpha = color.a;

        while (canvasGroup.alpha < targetAlpha)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        yield return new WaitForSeconds(displayTime);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}