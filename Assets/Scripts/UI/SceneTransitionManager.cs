using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Overlay (full-screen UI Image)")]
    [SerializeField] private RectTransform overlayRect;
    [SerializeField] private Image overlayImage;

    [Header("Timings")]
    [Tooltip("How long the player 'faces the camera' before the screen starts spinning/fading.")]
    [SerializeField] private float faceCameraHold = 0.9f;

    [Tooltip("How long the spin lasts.")]
    [SerializeField] private float spinDuration = 1.2f;

    [Tooltip("How long the fade to black lasts (can match spinDuration).")]
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Spin")]
    [Tooltip("How many full rotations the overlay does.")]
    [SerializeField] private int spinTurns = 2;

    private bool running;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PlayAndLoad(string sceneName, PlayerTransitionActor actor)
    {
        if (running) return;
        StartCoroutine(CoPlay(sceneName, actor));
    }

    IEnumerator CoPlay(string sceneName, PlayerTransitionActor actor)
    {
        running = true;

        
        if (actor != null)
        {
            actor.Freeze(true);
            actor.FaceCameraSprite(true);
        }

        
        yield return new WaitForSeconds(faceCameraHold);

        
        if (overlayRect != null)
        {
            overlayRect.gameObject.SetActive(true);
            overlayRect.localRotation = Quaternion.identity;
        }

        if (overlayImage != null)
        {
            var c = overlayImage.color;
            c.a = 0f;
            overlayImage.color = c;
        }

        
        float totalDegrees = 360f * Mathf.Max(1, spinTurns);
        float t = 0f;

        while (t < spinDuration)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;

            float spin01 = Mathf.Clamp01(t / spinDuration);
            float degreesNow = totalDegrees * spin01;

            if (overlayRect != null)
                overlayRect.localRotation = Quaternion.Euler(0f, 0f, degreesNow);

            if (overlayImage != null)
            {
                float fade01 = Mathf.Clamp01(t / Mathf.Max(0.0001f, fadeDuration));
                var c = overlayImage.color;
                c.a = fade01;
                overlayImage.color = c;
            }

            yield return null;
        }

       
        if (overlayImage != null)
        {
            var c = overlayImage.color;
            c.a = 1f;
            overlayImage.color = c;
        }

        SceneManager.LoadScene(sceneName);
    }
}
