using System.Collections;
using UnityEngine;
using TMPro;

public class Level2UIManager : MonoBehaviour
{
    public static Level2UIManager Instance;

    [Header("Interaction UI")]
    [SerializeField] private GameObject interactionTextObject;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Task Complete Popup")]
    [SerializeField] private GameObject taskCompletedObject;
    [SerializeField] private TextMeshProUGUI taskCompletedText;
    [SerializeField] private float taskCompletedDuration = 2f;

    [Header("End Transition (Fade Out)")]
    [SerializeField] private CanvasGroup taskCompletedCanvasGroup; //  CanvasGroup on the popup object 
    [SerializeField] private float fadeOutTime = 0.15f;

    [Header("Task Complete SFX (plays when popup closes)")]
    [SerializeField] private AudioSource sfxSource;          
    [SerializeField] private AudioClip taskCompleteClip;     // success sound
    [Range(0f, 1f)][SerializeField] private float taskCompleteVolume = 0.9f;

    [Header("UI Screen Shake (plays when popup closes)")]
    [SerializeField] private RectTransform shakeTarget;      
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 8f;

    private Coroutine popupRoutine;
    private Coroutine shakeRoutine;

    private Vector2 baseShakePos;

    public void ClosePanel(GameObject panelToClose)
    {
        if (panelToClose == null) return;

        panelToClose.SetActive(false);

       
        HideInteraction();
    }
    private void Awake()
    {
        Instance = this;
        HideInteraction();

        if (taskCompletedObject != null)
            taskCompletedObject.SetActive(false);

        if (shakeTarget != null)
            baseShakePos = shakeTarget.anchoredPosition;

        if (taskCompletedCanvasGroup == null && taskCompletedObject != null)
            taskCompletedCanvasGroup = taskCompletedObject.GetComponent<CanvasGroup>();
    }

    public void ShowInteraction(string message)
    {
        if (interactionTextObject == null || interactionText == null) return;
        interactionTextObject.SetActive(true);
        interactionText.text = message;
    }

    public void HideInteraction()
    {
        if (interactionTextObject == null) return;
        interactionTextObject.SetActive(false);
    }

    public void ShowTaskCompleted(string message = "TASK COMPLETED")
    {
        ShowTaskCompletedPopup(message, taskCompletedDuration);
    }

    public void ShowTaskCompletedPopup(string message, float seconds = 2f)
    {
        if (taskCompletedObject == null || taskCompletedText == null) return;

        if (popupRoutine != null) StopCoroutine(popupRoutine);
        popupRoutine = StartCoroutine(TaskCompletedRoutine(message, seconds));
    }

    private IEnumerator TaskCompletedRoutine(string msg, float seconds)
    {
        taskCompletedObject.SetActive(true);
        taskCompletedText.text = msg;

        if (taskCompletedCanvasGroup != null)
            taskCompletedCanvasGroup.alpha = 1f;

        float waitTime = Mathf.Max(0f, seconds - fadeOutTime);
        yield return new WaitForSecondsRealtime(waitTime);

           
        if (taskCompletedCanvasGroup != null && fadeOutTime > 0f)
        {
            float t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / fadeOutTime);
                taskCompletedCanvasGroup.alpha = Mathf.Lerp(1f, 0f, a);
                yield return null;
            }
            taskCompletedCanvasGroup.alpha = 0f;
        }

         
        taskCompletedObject.SetActive(false);
        popupRoutine = null;

        
        PlayTaskCompleteSfx();
        StartShake();
    }

    private void PlayTaskCompleteSfx()
    {
        if (sfxSource == null || taskCompleteClip == null) return;
        sfxSource.PlayOneShot(taskCompleteClip, taskCompleteVolume);
    }

    private void StartShake()
    {
        if (shakeTarget == null) return;

        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        baseShakePos = shakeTarget.anchoredPosition;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;
            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            shakeTarget.anchoredPosition = baseShakePos + offset;
            yield return null;
        }

        shakeTarget.anchoredPosition = baseShakePos;
        shakeRoutine = null;
    }
}