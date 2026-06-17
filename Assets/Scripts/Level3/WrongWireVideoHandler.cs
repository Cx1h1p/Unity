using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class WrongWireVideoHandler : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Fade Overlay")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Effects After Video")]
    [SerializeField] private SparkController sparkController;
    [SerializeField] private DimmerFeedback dimmer;

    [Header("Damage")]
    [SerializeField] private float minDamage = 10f;
    [SerializeField] private float maxDamage = 25f;

    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        if (videoPanel != null)
            videoPanel.SetActive(false);

        if (fadeOverlay != null)
            fadeOverlay.alpha = 0f;
    }

    public void PlayWrongWireSequence()
    {
        if (!IsPlaying)
            StartCoroutine(WrongWireSequence());
    }

    private IEnumerator WrongWireSequence()
    {
        IsPlaying = true;

        Time.timeScale = 0f;

        
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        // Show video
        videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();

       
        yield return StartCoroutine(FadeOverlay(1f, 0f));

        // Wait for video to finish
        while (videoPlayer.isPlaying)
            yield return null;

       
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        // Hide video
        videoPlayer.Stop();
        videoPanel.SetActive(false);

       
        yield return StartCoroutine(FadeOverlay(1f, 0f));

        Time.timeScale = 1f;

        
        if (sparkController != null)
            sparkController.PlayEffect();

        if (dimmer != null)
            dimmer.ShowWrong();

        float damage = Random.Range(minDamage, maxDamage + 1f);

        if (GlobalHP.Instance != null)
            GlobalHP.Instance.TakeDamage(damage);

        Debug.Log("Damage taken after video: " + damage);

        IsPlaying = false;
    }

    private IEnumerator FadeOverlay(float from, float to)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            if (fadeOverlay != null)
                fadeOverlay.alpha = Mathf.Lerp(from, to, t / fadeDuration);

            yield return null;
        }

        if (fadeOverlay != null)
            fadeOverlay.alpha = to;
    }
}