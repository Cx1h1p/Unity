using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("References")]
    public Transform puzzleList;            
    public PuzzlePiece[] pieces;             
    public GameObject puzzlePanel;

    [Header("Task System")]
    public string taskId = "Puzzle";

    [Header("Shuffle Animation")]
    public float scatterDistance = 80f;
    public float scatterDuration = 0.15f;
    public float settleDuration = 0.20f;

    [Header("Sound Effects (Puzzle Piece Placement)")]
    public AudioSource sfxSource;
    public AudioClip correctClip;           
    public AudioClip wrongClip;             
    [Range(0f, 1f)] public float correctVolume = 0.4f;
    [Range(0f, 1f)] public float wrongVolume = 1f;

    private bool completedOnce = false;
    private bool completionRoutineStarted = false; 
    private Coroutine shuffleRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        completedOnce = false;
        completionRoutineStarted = false;

        
        if ((pieces == null || pieces.Length == 0) && puzzleList != null)
            pieces = puzzleList.GetComponentsInChildren<PuzzlePiece>(true);

        if (shuffleRoutine != null) StopCoroutine(shuffleRoutine);
        shuffleRoutine = StartCoroutine(ShuffleRotateAnimated());
    }

    void OnDisable()
    {
        if (shuffleRoutine != null)
        {
            StopCoroutine(shuffleRoutine);
            shuffleRoutine = null;
        }
    }

    public void PlayCorrectSfx()
    {
        if (sfxSource == null || correctClip == null) return;
        sfxSource.PlayOneShot(correctClip, correctVolume);
    }

    public void PlayWrongSfx()
    {
        if (sfxSource == null || wrongClip == null) return;
        sfxSource.PlayOneShot(wrongClip, wrongVolume);
    }

    private IEnumerator ShuffleRotateAnimated()
    {
        if (puzzleList == null)
        {
            Debug.LogError("PuzzleManager: puzzleList not assigned.");
            yield break;
        }

        if (pieces == null || pieces.Length == 0)
        {
            Debug.LogError("PuzzleManager: pieces array empty. Assign pieces or make sure they are under PuzzleList.");
            yield break;
        }

        // Random rotations at start
        foreach (var p in pieces)
            if (p != null) p.RandomizeRotation();

        LayoutElement[] les = new LayoutElement[pieces.Length];
        RectTransform[] rts = new RectTransform[pieces.Length];
        Vector2[] startPos = new Vector2[pieces.Length];
        Vector2[] scatterPos = new Vector2[pieces.Length];

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == null) continue;

            rts[i] = pieces[i].GetComponent<RectTransform>();
            if (rts[i] == null) continue;

            les[i] = pieces[i].GetComponent<LayoutElement>();
            if (les[i] == null) les[i] = pieces[i].gameObject.AddComponent<LayoutElement>();

            les[i].ignoreLayout = true;

            startPos[i] = rts[i].anchoredPosition;
            Vector2 randomOffset = Random.insideUnitCircle * scatterDistance;
            scatterPos[i] = startPos[i] + randomOffset;
        }

        // Scatter out
        float t = 0f;
        while (t < scatterDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / scatterDuration);
            float eased = 1f - Mathf.Pow(1f - a, 3f);

            for (int i = 0; i < pieces.Length; i++)
            {
                if (rts[i] == null) continue;
                rts[i].anchoredPosition = Vector2.Lerp(startPos[i], scatterPos[i], eased);
            }
            yield return null;
        }

        // Shuffle sibling order
        for (int i = puzzleList.childCount - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            puzzleList.GetChild(i).SetSiblingIndex(j);
        }

        // Turn layout back on
        for (int i = 0; i < pieces.Length; i++)
        {
            if (les[i] == null) continue;
            les[i].ignoreLayout = false;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)puzzleList);

        // Smooth settle
        Vector2[] settleFrom = new Vector2[pieces.Length];
        Vector2[] settleTo = new Vector2[pieces.Length];

        for (int i = 0; i < pieces.Length; i++)
        {
            if (rts[i] == null) continue;
            settleFrom[i] = rts[i].anchoredPosition;
        }

        yield return null;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (rts[i] == null) continue;
            settleTo[i] = rts[i].anchoredPosition;
            rts[i].anchoredPosition = settleFrom[i];
        }

        t = 0f;
        while (t < settleDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / settleDuration);
            float eased = 1f - Mathf.Pow(1f - a, 3f);

            for (int i = 0; i < pieces.Length; i++)
            {
                if (rts[i] == null) continue;
                rts[i].anchoredPosition = Vector2.Lerp(settleFrom[i], settleTo[i], eased);
            }
            yield return null;
        }

        for (int i = 0; i < pieces.Length; i++)
        {
            if (rts[i] == null) continue;
            rts[i].anchoredPosition = settleTo[i];
        }

        shuffleRoutine = null;
    }

    public void CheckCompletion()
    {
        if (completedOnce) return;
        if (completionRoutineStarted) return; 
        if (pieces == null || pieces.Length == 0) return;

        foreach (var p in pieces)
        {
            if (p == null || !p.locked) return;
        }

        completedOnce = true;
        completionRoutineStarted = true; 
        StartCoroutine(PuzzleCompleted());
    }

    private IEnumerator PuzzleCompleted()
    {
        TaskManager.Instance?.CompleteTask(taskId);

        // nicer like blueprint (optional)
        Level2UIManager.Instance?.ShowTaskCompleted(taskId + " task completed!");

        yield return new WaitForSecondsRealtime(1.2f);

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);
    }
}