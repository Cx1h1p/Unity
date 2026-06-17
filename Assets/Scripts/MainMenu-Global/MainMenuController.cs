using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuButtonController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string firstLevelSceneName = "Level1";
    [SerializeField] private string settingsSceneName = "Settings";

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float delayBeforeFade = 0.03f;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Sound")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private float clickVolume = 0.8f;

    private bool busy = false;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.gameObject.SetActive(false);
        }
    }

    public void PlayGame()
    {
        if (busy) return;

        StartCoroutine(ButtonRoutine(() =>
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }));
    }

    public void OpenSettings()
    {
        if (busy) return;

        StartCoroutine(ButtonRoutine(() =>
        {
            SceneReturnData.PreviousSceneName =
                SceneManager.GetActiveScene().name;

            SceneManager.LoadScene(settingsSceneName);
        }));
    }

    public void QuitGame()
    {
        if (busy) return;

        StartCoroutine(ButtonRoutine(() =>
        {
            Debug.Log("Quit Game");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }));
    }

    public void OpenInfo()
    {
        if (busy) return;

        PlayClickOnly();

        Debug.Log("Open Info");
    }

    private IEnumerator ButtonRoutine(System.Action action)
    {
        busy = true;

        PlayClickOnly();

        yield return new WaitForSecondsRealtime(delayBeforeFade);

        yield return FadeToBlack();

        action?.Invoke();
    }

    private void PlayClickOnly()
    {
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip, clickVolume);
        }
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeGroup == null)
            yield break;

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.blocksRaycasts = true;

        float t = 0f;

        fadeGroup.alpha = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            fadeGroup.alpha = Mathf.Lerp(
                0f,
                1f,
                t / fadeDuration
            );

            yield return null;
        }

        fadeGroup.alpha = 1f;
    }
}