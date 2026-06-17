using System.Collections;
using UnityEngine;

public class BonusLevelIntroController : MonoBehaviour
{
    [Header("Intro")]
    public GameObject introPanel;
    public CanvasGroup introCanvasGroup;

    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip closeClip;

    [Header("Settings")]
    public float fadeOutDuration = 0.5f;
    public float gameplayStartDelay = 1f;

    private bool closing = false;

    void Start()
    {
        BonusLevelGameState.LockGameplay();

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 1f;
            introCanvasGroup.interactable = true;
            introCanvasGroup.blocksRaycasts = true;
        }
    }

    public void CloseIntro()
    {
        if (closing)
            return;

        closing = true;
        StartCoroutine(CloseRoutine());
    }

    IEnumerator CloseRoutine()
    {
        if (uiAudioSource != null && closeClip != null)
        {
            uiAudioSource.PlayOneShot(closeClip);
        }

        if (introCanvasGroup != null)
        {
            introCanvasGroup.interactable = false;
            introCanvasGroup.blocksRaycasts = false;

            float timer = 0f;

            while (timer < fadeOutDuration)
            {
                timer += Time.unscaledDeltaTime;

                introCanvasGroup.alpha =
                    Mathf.Lerp(1f, 0f, timer / fadeOutDuration);

                yield return null;
            }

            introCanvasGroup.alpha = 0f;
        }

        introPanel.SetActive(false);

        yield return new WaitForSecondsRealtime(gameplayStartDelay);

        BonusLevelGameState.StartGameplay();
    }
}