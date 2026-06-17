using System.Collections;
using UnityEngine;
using TMPro;

public class CodeKeypadTask : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private string taskId = "Code";

    [Header("Correct 4-digit code")]
    [SerializeField] private string correctCode = "7382";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI codeDisplay;
    [SerializeField] private TextMeshProUGUI lockStatusDisplay;

    [Header("Close")]
    [SerializeField] private GameObject panelToClose;

    [Header("Next Scene (after transition)")]
    [SerializeField] private string nextSceneName = "Level3";

    [Header("Display Behaviour")]
    [SerializeField] private bool clearInputOnOpen = true;
    [SerializeField] private bool maskInput = true;
    [SerializeField] private char maskChar = '*';
    [SerializeField] private char emptyChar = '-';

    [Header("LOCKED / UNLOCKED Colors")]
    [SerializeField] private Color lockedTextColor = new Color(155f / 255f, 210f / 255f, 111f / 255f, 1f);   // #9BD26F
    [SerializeField] private Color unlockedTextColor = new Color(92f / 255f, 255f / 255f, 136f / 255f, 1f); // #5CFF88

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickClip;

    [Tooltip("Plays ONLY when the code is correct (e.g. 5s long).")]
    [SerializeField] private AudioClip processingClip;

    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;

    [Range(0f, 1f)][SerializeField] private float buttonClickVolume = 0.6f;
    [Range(0f, 1f)][SerializeField] private float processingVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float correctVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float wrongVolume = 1.0f;

    [Header("Feedback Timings")]
    [SerializeField] private float wrongFlashTime = 0.25f;
    [SerializeField] private float correctFlashTime = 0.25f;
    [SerializeField] private float closeDelayAfterCorrect = 0.45f;

    [Header("UNLOCKED Text Animation")]
    [SerializeField] private float lockedFadeOutTime = 0.20f;
    [SerializeField] private float unlockedLetterDelay = 0.05f;
    [SerializeField] private float unlockedFadeInTime = 0.12f;

    private string current = "";
    private bool busy = false;

    private string originalCodeText;
    private Color originalCodeColor;

    private string originalLockText;
    private Color originalLockColor;

    private string emptyPlaceholderText;

    void Awake()
    {
        if (codeDisplay != null)
        {
            originalCodeText = codeDisplay.text;
            originalCodeColor = codeDisplay.color;
            emptyPlaceholderText = originalCodeText;
        }

        if (lockStatusDisplay != null)
        {
            originalLockText = lockStatusDisplay.text;
            originalLockColor = lockStatusDisplay.color;
        }
    }

    void OnEnable()
    {
        busy = false;

        if (lockStatusDisplay != null)
        {
            lockStatusDisplay.text = "LOCKED";
            lockStatusDisplay.color = lockedTextColor;
            lockStatusDisplay.maxVisibleCharacters = int.MaxValue;
        }

        if (clearInputOnOpen)
        {
            current = "";
            RefreshDisplay();
        }
    }

    void OnDisable()
    {
        if (codeDisplay != null)
        {
            codeDisplay.text = originalCodeText;
            codeDisplay.color = originalCodeColor;
        }

        if (lockStatusDisplay != null)
        {
            lockStatusDisplay.text = originalLockText;
            lockStatusDisplay.color = originalLockColor;
            lockStatusDisplay.maxVisibleCharacters = int.MaxValue;
        }

        current = "";
        busy = false;
    }

    
    public void AddDigit(int digit) => PressDigit(digit);
    public void Clear() => PressCancel();
    public void Submit() => PressEnter();

    public void PressDigit(int digit)
    {
        if (busy) return;
        PlayClick();

        if (current.Length >= 4) return;
        current += digit.ToString();
        RefreshDisplay();
    }

    public void PressCancel()
    {
        if (busy) return;
        PlayClick();

        current = "";
        RefreshDisplay();
    }

    public void PressEnter()
    {
        if (busy) return;
        PlayClick();

        if (current.Length != 4) return;

        bool isCorrect = (current == correctCode);

        if (isCorrect)
            StartCoroutine(CorrectFlowRoutine());
        else
            StartCoroutine(WrongRevealRoutine());
    }

    
    private IEnumerator WrongRevealRoutine()
    {
        busy = true;

        PlayWrong();

        if (codeDisplay != null)
        {
            Color before = codeDisplay.color;
            codeDisplay.color = Color.red;
            yield return new WaitForSecondsRealtime(wrongFlashTime);
            codeDisplay.color = before;
        }

        current = "";
        RefreshDisplay();

        busy = false;
    }

    private IEnumerator CorrectFlowRoutine()
    {
        busy = true;

        PlayCorrect();

        if (codeDisplay != null)
        {
            Color before = codeDisplay.color;
            codeDisplay.color = Color.green;
            yield return new WaitForSecondsRealtime(correctFlashTime);
            codeDisplay.color = before;
        }

        if (audioSource != null && processingClip != null)
        {
            audioSource.PlayOneShot(processingClip, processingVolume);
            yield return new WaitForSecondsRealtime(processingClip.length);
        }

        if (lockStatusDisplay != null)
            yield return StartCoroutine(AnimateUnlockedText());

        yield return new WaitForSecondsRealtime(closeDelayAfterCorrect);

        if (TaskManager.Instance != null)
            TaskManager.Instance.CompleteTask(taskId);

        if (Level2UIManager.Instance != null)
            Level2UIManager.Instance.ShowTaskCompleted("CODE COMPLETED");

        if (Level2UIManager.Instance != null && panelToClose != null)
            Level2UIManager.Instance.ClosePanel(panelToClose);
        else if (panelToClose != null)
            panelToClose.SetActive(false);
        else
            gameObject.SetActive(false);

        if (Level2TransitionFX.Instance != null)
            Level2TransitionFX.Instance.PlayToScene(nextSceneName);
        else
            Debug.LogError("Level2TransitionFX.Instance not found in scene. Add Level2TransitionFX_Manager and assign overlay/camera/audio.");

    }

    private IEnumerator AnimateUnlockedText()
    {
        if (lockStatusDisplay == null)
            yield break;

        Color startColor = lockedTextColor;
        float t = 0f;

        lockStatusDisplay.maxVisibleCharacters = int.MaxValue;

        while (t < lockedFadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            float a = 1f - Mathf.Clamp01(t / lockedFadeOutTime);
            lockStatusDisplay.color = new Color(startColor.r, startColor.g, startColor.b, a);
            yield return null;
        }

        
        lockStatusDisplay.text = "UNLOCKED";
        lockStatusDisplay.ForceMeshUpdate();

        Color target = unlockedTextColor;
        lockStatusDisplay.color = new Color(target.r, target.g, target.b, 0f);
        lockStatusDisplay.maxVisibleCharacters = 0;

        
        float ft = 0f;
        while (ft < unlockedFadeInTime)
        {
            ft += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(ft / unlockedFadeInTime);
            lockStatusDisplay.color = new Color(target.r, target.g, target.b, a);
            yield return null;
        }

       
        int total = lockStatusDisplay.text.Length;
        for (int i = 1; i <= total; i++)
        {
            lockStatusDisplay.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(unlockedLetterDelay);
        }
    }

    private void RefreshDisplay()
    {
        if (codeDisplay == null) return;

        if (current.Length == 0)
        {
            codeDisplay.text = string.IsNullOrEmpty(emptyPlaceholderText) ? "****" : emptyPlaceholderText;
            return;
        }

        string shown = maskInput ? new string(maskChar, current.Length) : current;
        shown = shown.PadRight(4, emptyChar);
        codeDisplay.text = shown;
    }

    private void PlayClick()
    {
        if (audioSource != null && buttonClickClip != null)
            audioSource.PlayOneShot(buttonClickClip, buttonClickVolume);
    }

    private void PlayCorrect()
    {
        if (audioSource != null && correctClip != null)
            audioSource.PlayOneShot(correctClip, correctVolume);
    }

    private void PlayWrong()
    {
        if (audioSource != null && wrongClip != null)
            audioSource.PlayOneShot(wrongClip, wrongVolume);
    }
}