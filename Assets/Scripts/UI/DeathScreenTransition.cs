using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathScreenTransition : MonoBehaviour
{
    [Header("Death Screen Fade In")]
    [SerializeField] private CanvasGroup deathScreenGroup;
    [SerializeField] private float deathScreenFadeDuration = 0.6f;

    [Header("Scene Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Delay")]
    [SerializeField] private float delayBeforeFade = 0.15f;

    [Header("Main Menu")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool busy = false;

    void Start()
    {
        StartCoroutine(SceneFadeOut());
    }

    void OnEnable()
    {
        StartCoroutine(DeathScreenFadeIn());
    }

    IEnumerator SceneFadeOut()
    {
        if (fadeGroup == null)
            yield break;

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.blocksRaycasts = true;

        fadeGroup.alpha = 1f;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            fadeGroup.alpha =
                Mathf.Lerp(1f, 0f, t / fadeDuration);

            yield return null;
        }

        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }

    IEnumerator DeathScreenFadeIn()
    {
        if (deathScreenGroup == null)
            yield break;

        deathScreenGroup.alpha = 0f;
        deathScreenGroup.interactable = false;
        deathScreenGroup.blocksRaycasts = false;

        float t = 0f;

        while (t < deathScreenFadeDuration)
        {
            t += Time.unscaledDeltaTime;

            deathScreenGroup.alpha =
                Mathf.Lerp(0f, 1f, t / deathScreenFadeDuration);

            yield return null;
        }

        deathScreenGroup.alpha = 1f;
        deathScreenGroup.interactable = true;
        deathScreenGroup.blocksRaycasts = true;
    }

    public void Restart()
    {
        if (!busy)
            StartCoroutine(RestartRoutine());
    }

    public void LoadMainMenu()
    {
        if (!busy)
            StartCoroutine(MainMenuRoutine());
    }

    IEnumerator RestartRoutine()
    {
        busy = true;

        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return StartCoroutine(FadeToBlack());

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    IEnumerator MainMenuRoutine()
    {
        busy = true;

        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return StartCoroutine(FadeToBlack());

        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAndDestroyMusic();

        SceneManager.LoadScene(mainMenuScene);
    }

    IEnumerator FadeToBlack()
    {
        if (fadeGroup == null)
            yield break;

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.blocksRaycasts = true;

        float t = 0f;
        float startAlpha = fadeGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            fadeGroup.alpha =
                Mathf.Lerp(startAlpha, 1f, t / fadeDuration);

            yield return null;
        }

        fadeGroup.alpha = 1f;
    }
}