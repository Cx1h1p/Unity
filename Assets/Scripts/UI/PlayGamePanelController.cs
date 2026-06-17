using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayGamePanelController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string level1SceneName = "Level_01_ForestFire";
    [SerializeField] private string bonusSceneName = "BonusLevel";

    [Header("Bonus UI")]
    [SerializeField] private Button bonusButton;
    [SerializeField] private Image bonusLockImage;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private TMP_Text bonusInfoText;

    [Header("Transition Fade")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Sound")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip wrongSound;

    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float wrongVolume = 1f;

    private bool loading = false;
    private Image fadeImage;

    private void Start()
    {
        RefreshUI();

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
            fadeCanvas.interactable = false;

            fadeImage = fadeCanvas.GetComponent<Image>();
        }
    }

    public void PlayMainGame()
    {
        if (loading) return;

        StartCoroutine(LoadSceneWithFade(level1SceneName));
    }

    public void PlayBonusLevel()
    {
        if (loading) return;

        
        if (!GameSaveData.IsBonusUnlocked())
        {
            PlayWrongSound();
            return;
        }

        
        StartCoroutine(LoadSceneWithFade(bonusSceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        loading = true;

        Time.timeScale = 1f;

        PlayClickSound();

        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);

            fadeCanvas.blocksRaycasts = true;
            fadeCanvas.interactable = true;

            fadeCanvas.alpha = 0f;

            
            if (fadeImage != null)
            {
                fadeImage.color = Color.black;
            }

            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;

                fadeCanvas.alpha =
                    Mathf.Lerp(0f, 1f, t / fadeDuration);

                yield return null;
            }

            fadeCanvas.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void PlayClickSound()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound, clickVolume);
        }
    }

    private void PlayWrongSound()
    {
        if (uiAudioSource != null && wrongSound != null)
        {
            uiAudioSource.PlayOneShot(wrongSound, wrongVolume);
        }

        if (fadeCanvas != null)
        {
            StartCoroutine(WrongFlash());
        }
    }

    private IEnumerator WrongFlash()
    {
        fadeCanvas.gameObject.SetActive(true);

        fadeCanvas.blocksRaycasts = false;
        fadeCanvas.interactable = false;

        if (fadeImage != null)
        {
            fadeImage.color = Color.red;
        }

        float flashDuration = 0.15f;

        float t = 0f;

        
        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;

            fadeCanvas.alpha =
                Mathf.Lerp(0f, 0.45f, t / flashDuration);

            yield return null;
        }

        
        t = 0f;

        while (t < flashDuration)
        {
            t += Time.unscaledDeltaTime;

            fadeCanvas.alpha =
                Mathf.Lerp(0.45f, 0f, t / flashDuration);

            yield return null;
        }

        fadeCanvas.alpha = 0f;

        
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
        }
    }

    public void RefreshUI()
    {
        bool unlocked = GameSaveData.IsBonusUnlocked();

        
        if (bonusButton != null)
        {
            bonusButton.interactable = true;
        }

        
        if (bonusLockImage != null)
        {
            bonusLockImage.sprite =
                unlocked ? unlockedSprite : lockedSprite;
        }

        
        if (bonusInfoText != null)
        {
            bonusInfoText.text = unlocked
                ? "BONUS LEVEL UNLOCKED"
                : "Complete Level 3 to unlock Bonus Level!";
        }
    }
}