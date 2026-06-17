using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup escMenuGroup;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string settingsScene = "Settings";

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.35f;

    private bool isPaused = false;
    private bool busy = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HideFade();

        if (escMenuGroup != null)
        {
            escMenuGroup.alpha = 0f;
            escMenuGroup.interactable = false;
            escMenuGroup.blocksRaycasts = false;
            escMenuGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !busy)
        {
            if (isPaused)
                StartCoroutine(CloseMenuRoutine());
            else
                StartCoroutine(OpenMenuRoutine());
        }
    }

    IEnumerator OpenMenuRoutine()
    {
        busy = true;
        isPaused = true;

        escMenuGroup.gameObject.SetActive(true);

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            escMenuGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            yield return null;
        }

        escMenuGroup.alpha = 1f;
        escMenuGroup.interactable = true;
        escMenuGroup.blocksRaycasts = true;

        Time.timeScale = 0f;

        busy = false;
    }

    IEnumerator CloseMenuRoutine()
    {
        busy = true;

        escMenuGroup.interactable = false;
        escMenuGroup.blocksRaycasts = false;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            escMenuGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            yield return null;
        }

        escMenuGroup.alpha = 0f;
        escMenuGroup.gameObject.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
        busy = false;
    }

    public void ResumeGame()
    {
        if (!busy)
            StartCoroutine(CloseMenuRoutine());
    }

    public void RestartLevel()
    {
        if (!busy)
            StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadMainMenu()
    {
        if (!busy)
            StartCoroutine(LoadSceneRoutine(mainMenuScene));
    }

    public void OpenSettings()
    {
        if (!busy)
            StartCoroutine(OpenSettingsRoutine());
    }

    IEnumerator OpenSettingsRoutine()
    {
        busy = true;

        SceneReturnData.PreviousSceneName =
            SceneManager.GetActiveScene().name;

        yield return StartCoroutine(FadeScreen());

        Time.timeScale = 0f;

        SceneManager.LoadScene(settingsScene, LoadSceneMode.Additive);

        busy = false;
    }

    public void ReturnFromSettings()
    {
        HideFade();

        isPaused = true;
        busy = false;

        if (escMenuGroup != null)
        {
            escMenuGroup.gameObject.SetActive(true);

            escMenuGroup.alpha = 1f;
            escMenuGroup.interactable = true;
            escMenuGroup.blocksRaycasts = true;
        }

        Time.timeScale = 0f;
    }

    IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        yield return StartCoroutine(FadeScreen());

        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAndDestroyMusic();

        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeScreen());

        Time.timeScale = 1f;

        if (sceneName == mainMenuScene &&
            MusicManager.Instance != null)
        {
            MusicManager.Instance.StopAndDestroyMusic();
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeScreen()
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

    private void HideFade()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.gameObject.SetActive(true);
        }
    }
    public void OpenMenuFromButton()
    {
        if (!busy && !isPaused)
            StartCoroutine(OpenMenuRoutine());
    }
}