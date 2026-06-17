using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameButtons : MonoBehaviour
{
    [Header("UI Sound")]
    [SerializeField] private AudioSource uiSFXSource;
    [SerializeField] private AudioClip clickSound;

    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;

    private bool loading = false;

    private void Start()
    {
        // Player reached this end screen, so bonus is unlocked forever
        GameSaveData.UnlockBonusLevel();
    }

    public void LoadMainMenu()
    {
        if (!loading)
            StartCoroutine(LoadSceneRoutine("MainMenu"));
    }

    public void LoadBonusLevel()
    {
        if (!loading)
            StartCoroutine(LoadSceneRoutine("BonusLevel"));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        loading = true;

        Time.timeScale = 1f;

        if (uiSFXSource != null && clickSound != null)
            uiSFXSource.PlayOneShot(clickSound, clickVolume);

        if (fadeCanvas != null)
        {
            float t = 0f;
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = true;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }

            fadeCanvas.alpha = 1f;
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.4f);
        }

        if (sceneName == "MainMenu" && MusicManager.Instance != null)
            MusicManager.Instance.StopAndDestroyMusic();

        SceneManager.LoadScene(sceneName);
    }
}