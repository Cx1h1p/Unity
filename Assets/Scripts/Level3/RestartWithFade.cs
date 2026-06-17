using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartWithFade : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Delay")]
    [SerializeField] private float delayBeforeFade = 1f;

    [Header("Main Menu")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    public void Restart()
    {
        StartCoroutine(RestartRoutine());
    }

    public void LoadMainMenu()
    {
        StartCoroutine(MainMenuRoutine());
    }

    IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return StartCoroutine(FadeToBlack());

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator MainMenuRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return StartCoroutine(FadeToBlack());

        Time.timeScale = 1f;

      
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAndDestroyMusic();

        SceneManager.LoadScene(mainMenuScene);
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        float startAlpha = fadeGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            float a = Mathf.Lerp(startAlpha, 1f, t / fadeDuration);

            fadeGroup.alpha = a;

            yield return null;
        }

        fadeGroup.alpha = 1f;
    }
}