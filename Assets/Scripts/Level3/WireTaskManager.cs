using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WireTaskManager : MonoBehaviour
{
    [Header("Slots")]
    public DropSlot[] slots;

    [Header("Panels")]
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup firstPanelRoot;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip completeClip;
    [Range(0f, 1f)][SerializeField] private float volume = 0.8f;

    private bool completed = false;
    private bool routineStarted = false;

    void Update()
    {
        CheckCompletion(); // 🔥 continuous check like PuzzleManager
    }

    public void CheckCompletion()
    {
        if (completed || routineStarted) return;
        if (slots == null || slots.Length == 0) return;

        foreach (DropSlot slot in slots)
        {
            if (!slot.IsCorrect())
                return;
        }

        completed = true;
        routineStarted = true;

        Debug.Log("WIRE TASK COMPLETE");

        StartCoroutine(CompleteRoutine());
    }

    IEnumerator CompleteRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        // 🔊 Sound
        if (completeClip != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(completeClip, volume);

        // 🔻 Fade OUT
        yield return StartCoroutine(Fade(mainPanel, 1f, 0f));
        mainPanel.gameObject.SetActive(false);

        // 🔺 Activate next
        firstPanelRoot.gameObject.SetActive(true);
        firstPanelRoot.alpha = 0f;

        yield return StartCoroutine(Fade(firstPanelRoot, 0f, 1f));
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        cg.alpha = to;
    }
    public bool IsCompleted()
    {
        return completed;
    }
}