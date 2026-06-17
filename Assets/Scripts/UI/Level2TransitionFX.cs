using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level2TransitionFX : MonoBehaviour
{
    public static Level2TransitionFX Instance;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Level3";

    [Header("Camera")]
    [SerializeField] private Transform cameraTarget;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip transitionClip;
    [Range(0f, 1f)]
    [SerializeField] private float transitionVolume = 1f;

    [Header("Timing")]
    [SerializeField] private float fadeShakeTime = 2.2f;
    [SerializeField] private float blackHoldTime = 1.2f;

    [Header("Shake / Spin")]
    [SerializeField] private float shakeStrength = 0.28f;
    [SerializeField] private float spinDegrees = 25f;

    private CanvasGroup fadeGroup;
    private GameObject fadeCanvasObject;

    private bool busy;
    private Vector3 baseCamPos;
    private Quaternion baseCamRot;

    private void Awake()
    {
        Instance = this;

        CreateFadeOverlay();

        if (Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
        }
    }

    public void PlayToScene()
    {
        if (busy) return;

        StartCoroutine(TransitionRoutine());
    }

    public void PlayToScene(string sceneName)
    {
        if (busy) return;

        nextSceneName = sceneName;
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        busy = true;
        Time.timeScale = 1f;

        CacheCamera();

        if (sfxSource != null && transitionClip != null)
        {
            sfxSource.PlayOneShot(transitionClip, transitionVolume);
        }

        
        yield return FadeShakeAndSpin();

       
        yield return new WaitForSecondsRealtime(blackHoldTime);

        
        DontDestroyOnLoad(fadeCanvasObject);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(nextSceneName);
        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
        {
            yield return null;
        }

       
        if (fadeCanvasObject != null)
        {
            Destroy(fadeCanvasObject);
        }

        busy = false;
    }

    private void CreateFadeOverlay()
    {
        fadeCanvasObject = new GameObject("Level2 Transition Canvas");

        Canvas canvas = fadeCanvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        fadeCanvasObject.AddComponent<CanvasScaler>();
        fadeCanvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("Black Fade Image");
        imageObject.transform.SetParent(fadeCanvasObject.transform, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeGroup = imageObject.AddComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.interactable = false;
    }

    private void CacheCamera()
    {
        if (Camera.main != null)
        {
            cameraTarget = Camera.main.transform;
        }

        if (cameraTarget != null)
        {
            baseCamPos = cameraTarget.position;
            baseCamRot = cameraTarget.rotation;
        }
    }

    private IEnumerator FadeShakeAndSpin()
    {
        fadeGroup.blocksRaycasts = true;
        fadeGroup.alpha = 0f;

        float t = 0f;

        while (t < fadeShakeTime)
        {
            t += Time.unscaledDeltaTime;

            float a = Mathf.Clamp01(t / fadeShakeTime);
            float smooth = Mathf.SmoothStep(0f, 1f, a);

            fadeGroup.alpha = Mathf.Lerp(0f, 1f, smooth);

            if (cameraTarget != null)
            {
                float curve = Mathf.Sin(a * Mathf.PI);

                Vector2 offset =
                    Random.insideUnitCircle *
                    shakeStrength *
                    curve;

                cameraTarget.position =
                    baseCamPos +
                    new Vector3(offset.x, offset.y, 0f);

                float spin =
                    Mathf.Sin(a * Mathf.PI * 2f) *
                    spinDegrees *
                    curve;

                cameraTarget.rotation =
                    baseCamRot *
                    Quaternion.Euler(0f, 0f, spin);
            }

            yield return null;
        }

        fadeGroup.alpha = 1f;

        if (cameraTarget != null)
        {
            cameraTarget.position = baseCamPos;
            cameraTarget.rotation = baseCamRot;
        }
    }
}