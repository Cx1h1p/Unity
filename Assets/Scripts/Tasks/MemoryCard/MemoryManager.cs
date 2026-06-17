using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private CardFlip cardPrefab;

    [Header("Front Images (unique)")]
    [SerializeField] private Sprite[] uniqueImages;

    [Header("Timing")]
    [SerializeField] private float mismatchDelay = 0.8f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;
    [Range(0f, 1f)][SerializeField] private float clickVolume = 0.7f;
    [Range(0f, 1f)][SerializeField] private float correctVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float wrongVolume = 0.8f;

    public bool IsSolved { get; private set; }
    public System.Action OnSolved;

    private CardFlip first;
    private CardFlip second;
    private bool locked;
    private int matchedPairs;

    private Coroutine matchRoutine;

    private void OnEnable()
    {
        if (!IsSolved)
            GenerateBoard();
    }

    private void OnDisable()
    {
        
        if (matchRoutine != null)
            StopCoroutine(matchRoutine);

        matchRoutine = null;

        
        first = null;
        second = null;
        locked = false;
    }

    public void GenerateBoard()
    {
        if (IsSolved) return;

        ClearBoard();

       
        List<int> ids = new List<int>();
        for (int i = 0; i < uniqueImages.Length; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        matchedPairs = 0;
        first = null;
        second = null;
        locked = false;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            CardFlip card = Instantiate(cardPrefab, gridParent);
            card.Setup(id, uniqueImages[id]);
        }
    }

    public void CardSelected(CardFlip card)
    {
        if (IsSolved) return;
        if (locked) return;
        if (card == null) return;
        if (card == first) return;

        
        if (sfxSource != null && clickClip != null)
            sfxSource.PlayOneShot(clickClip, clickVolume);

        card.Show();

        if (first == null)
        {
            first = card;
            return;
        }

        second = card;

       
        if (matchRoutine != null)
            StopCoroutine(matchRoutine);

        matchRoutine = StartCoroutine(CheckMatch());
    }

    private IEnumerator CheckMatch()
    {
        locked = true;

        yield return new WaitForSecondsRealtime(mismatchDelay);

        if (first != null && second != null && first.cardID == second.cardID)
        {
            
            if (sfxSource != null && correctClip != null)
                sfxSource.PlayOneShot(correctClip, correctVolume);

            matchedPairs++;

            first = null;
            second = null;
            locked = false;

            if (matchedPairs >= uniqueImages.Length)
            {
                IsSolved = true;
                Debug.Log("✅ All pairs matched!");
                OnSolved?.Invoke();
            }

            matchRoutine = null;
            yield break;
        }

      
        if (sfxSource != null && wrongClip != null)
            sfxSource.PlayOneShot(wrongClip, wrongVolume);

        if (first != null) first.Hide();
        if (second != null) second.Hide();

        yield return new WaitForSecondsRealtime(0.15f);

        first = null;
        second = null;
        locked = false;

        matchRoutine = null;
    }

    private void ClearBoard()
    {
        if (gridParent == null) return;

        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public void ResetPuzzle()
    {
        IsSolved = false;
        GenerateBoard();
    }
}