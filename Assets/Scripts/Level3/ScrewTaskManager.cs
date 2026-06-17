using UnityEngine;
using System.Collections;

public class ScrewTaskManager : MonoBehaviour
{
    [Header("Screws")]
    [SerializeField] private ScrewRotator[] screws;

    [Header("Panel To Close")]
    [SerializeField] private GameObject firstPanelRoot;

    [Header("End Screen")]
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private RectTransform endGameScreenRect;

    [Header("Animation")]
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private float animationDuration = 1.2f;
    [SerializeField] private float startScale = 0.1f;
    [SerializeField] private float endScale = 1f;
    [SerializeField] private float startRotation = 720f;

    [Header("Sound")]
    [SerializeField] private AudioClip completionSound;
    [Range(0f, 1f)]
    [SerializeField] private float completionVolume = 0.8f;

    [Header("Spin Sound")]
    [SerializeField] private AudioClip spinWhooshClip;
    [Range(0f, 1f)]
    [SerializeField] private float spinWhooshVolume = 0.8f;

    private bool completed = false;

    public void CheckCompletion()
    {
        if (completed)
            return;

        if (screws == null || screws.Length == 0)
            return;

        foreach (ScrewRotator screw in screws)
        {
            if (screw == null || !screw.IsDone())
                return;
        }

        completed = true;
        StartCoroutine(CompleteRoutine());
    }

    IEnumerator CompleteRoutine()
    {
        yield return new WaitForSecondsRealtime(closeDelay);

        if (firstPanelRoot != null)
            firstPanelRoot.SetActive(false);

        if (endGameScreen == null || endGameScreenRect == null)
            yield break;

        endGameScreen.SetActive(true);

        if (completionSound != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(completionSound, completionVolume);

        if (spinWhooshClip != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(spinWhooshClip, spinWhooshVolume);

        Time.timeScale = 0f;

        endGameScreenRect.localScale = Vector3.one * startScale;
        endGameScreenRect.localRotation = Quaternion.Euler(0f, 0f, startRotation);

        float t = 0f;

        while (t < animationDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / animationDuration);

            float eased = 1f - Mathf.Pow(1f - a, 3f);

            float scale = Mathf.Lerp(startScale, endScale, eased);
            float rotation = Mathf.Lerp(startRotation, 0f, eased);

            endGameScreenRect.localScale = Vector3.one * scale;
            endGameScreenRect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            yield return null;
        }

        endGameScreenRect.localScale = Vector3.one * endScale;
        endGameScreenRect.localRotation = Quaternion.identity;
    }
}