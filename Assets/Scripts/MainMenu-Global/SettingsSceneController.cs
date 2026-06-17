using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsSceneController : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;

    [SerializeField] private float delayBeforeFade = 0.25f;
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Sound")]
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip applySuccessClip;

    [SerializeField] private float clickVolume = 0.8f;
    [SerializeField] private float applySuccessVolume = 0.9f;

    private bool busy = false;

    public void CloseSettings()
    {
        if (busy) return;

        StartCoroutine(ReturnRoutine(false));
    }

    public void ApplySettings()
    {
        if (busy) return;

        StartCoroutine(ReturnRoutine(true));
    }

    private IEnumerator ReturnRoutine(bool apply)
    {
        busy = true;

        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip, clickVolume);
        }

        if (apply)
        {
            PlayerPrefs.Save();

            yield return new WaitForSecondsRealtime(0.15f);

            if (sfxSource != null &&
                applySuccessClip != null)
            {
                sfxSource.PlayOneShot(
                    applySuccessClip,
                    applySuccessVolume
                );
            }
        }

        yield return new WaitForSecondsRealtime(
            delayBeforeFade
        );

        yield return FadeToBlack();

        // SETTINGS OPENED FROM GAME
        if (SceneManager.sceneCount > 1)
        {
            if (PauseMenu.Instance != null)
                PauseMenu.Instance.ReturnFromSettings();

            SceneManager.UnloadSceneAsync("Settings");
        }
        else
        {
            // SETTINGS OPENED FROM MAIN MENU

            Time.timeScale = 1f;

            string sceneToLoad =
                SceneReturnData.PreviousSceneName;

            if (string.IsNullOrEmpty(sceneToLoad))
                sceneToLoad = "MainMenu";

            SceneManager.LoadScene(sceneToLoad);
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

            fadeGroup.alpha =
                Mathf.Lerp(0f, 1f, t / fadeDuration);

            yield return null;
        }

        fadeGroup.alpha = 1f;
    }
}