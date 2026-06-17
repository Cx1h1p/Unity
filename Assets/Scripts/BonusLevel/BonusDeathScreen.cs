using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BonusDeathScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathScreenObject;
    public CanvasGroup deathScreenGroup;

    [Header("Score Display")]
    public BonusDeathScreenScoreDisplay scoreDisplay;

    [Header("Death Screen Fade In")]
    public float deathFadeDuration = 0.6f;

    [Header("Scene Fade Out")]
    public CanvasGroup fadeOverlay;
    public Image fadeOverlayImage;
    public float sceneFadeDuration = 1f;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    [Header("Button Sound")]
    public AudioSource uiSFXSource;
    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float clickVolume = 1f;

    private bool showing = false;
    private bool loading = false;

    private void Start()
    {
        if (deathScreenObject != null)
        {
            deathScreenObject.SetActive(true);
            deathScreenObject.transform.SetAsLastSibling();
        }

        if (deathScreenGroup != null)
        {
            deathScreenGroup.alpha = 0f;
            deathScreenGroup.interactable = false;
            deathScreenGroup.blocksRaycasts = false;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
            fadeOverlay.interactable = false;
        }

        if (fadeOverlayImage != null)
        {
            fadeOverlayImage.color = Color.black;
        }
    }

    public void ShowDeathScreen()
    {
        if (showing)
            return;

        showing = true;

        Debug.Log("ShowDeathScreen called");

        BonusLevelGameState.LockGameplay();

        if (scoreDisplay != null)
        {
            scoreDisplay.UpdateScoreDisplay();
        }
        else
        {
            Debug.LogWarning("BonusDeathScreen: scoreDisplay is not assigned.");
        }

        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (deathScreenObject == null || deathScreenGroup == null)
        {
            Debug.LogError("DeathScreen Object or CanvasGroup missing.");
            yield break;
        }

        deathScreenObject.SetActive(true);
        deathScreenObject.transform.SetAsLastSibling();

        deathScreenGroup.interactable = true;
        deathScreenGroup.blocksRaycasts = true;

        Time.timeScale = 0f;

        float t = 0f;

        while (t < deathFadeDuration)
        {
            t += Time.unscaledDeltaTime;

            deathScreenGroup.alpha =
                Mathf.Lerp(0f, 1f, t / deathFadeDuration);

            yield return null;
        }

        deathScreenGroup.alpha = 1f;
    }

    public void RestartLevel()
    {
        if (loading)
            return;

        StartCoroutine(SceneTransitionRoutine(
            SceneManager.GetActiveScene().name,
            false
        ));
    }

    public void LoadMainMenu()
    {
        if (loading)
            return;

        StartCoroutine(SceneTransitionRoutine(
            mainMenuScene,
            true
        ));
    }

    private IEnumerator SceneTransitionRoutine(string sceneName, bool stopMusic)
    {
        loading = true;

        BonusLevelGameState.LockGameplay();

        if (uiSFXSource != null && clickSound != null)
        {
            uiSFXSource.PlayOneShot(clickSound, clickVolume);
        }

        if (fadeOverlayImage != null)
        {
            fadeOverlayImage.color = Color.black;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.transform.SetAsLastSibling();

            fadeOverlay.blocksRaycasts = true;
            fadeOverlay.interactable = true;
            fadeOverlay.alpha = 0f;

            float t = 0f;

            while (t < sceneFadeDuration)
            {
                t += Time.unscaledDeltaTime;

                fadeOverlay.alpha =
                    Mathf.Lerp(0f, 1f, t / sceneFadeDuration);

                yield return null;
            }

            fadeOverlay.alpha = 1f;
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        Time.timeScale = 1f;

        if (stopMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.StopAndDestroyMusic();
        }

        SceneManager.LoadScene(sceneName);
    }
}