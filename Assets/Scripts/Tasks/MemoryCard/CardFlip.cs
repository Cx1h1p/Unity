using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardFlip : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform visual;
    [SerializeField] private Image back;
    [SerializeField] private Image front;

    [Header("Flip")]
    [SerializeField] private float flipSpeed = 0.12f;

    [HideInInspector] public int cardID;

    private bool faceUp;
    private bool flipping;
    private MemoryManager manager;

    private void Awake()
    {
        
        manager = FindFirstObjectByType<MemoryManager>();
    }

    
    public void Setup(int id, Sprite frontSprite)
    {
        cardID = id;

        if (front != null) front.sprite = frontSprite;

        faceUp = false;
        flipping = false;

        if (front != null) front.enabled = false;
        if (back != null) back.enabled = true;

        if (visual != null) visual.localScale = Vector3.one;
    }

    
    public void OnClick()
    {
        if (flipping) return;
        if (manager == null) manager = FindFirstObjectByType<MemoryManager>();
        if (manager == null) return;

        manager.CardSelected(this);
    }

    public void Show()
    {
        if (flipping) return;
        if (faceUp) return;
        StartCoroutine(FlipRoutine(true));
    }

    public void Hide()
    {
        if (flipping) return;
        if (!faceUp) return;
        StartCoroutine(FlipRoutine(false));
    }

    private IEnumerator FlipRoutine(bool turnFaceUp)
    {
        flipping = true;

        float t = 0f;

        // shrink
        while (t < flipSpeed)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / flipSpeed);
            if (visual != null) visual.localScale = new Vector3(Mathf.Lerp(1f, 0f, a), 1f, 1f);
            yield return null;
        }

        // swap
        faceUp = turnFaceUp;
        if (front != null) front.enabled = faceUp;
        if (back != null) back.enabled = !faceUp;

        // grow
        t = 0f;
        while (t < flipSpeed)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / flipSpeed);
            if (visual != null) visual.localScale = new Vector3(Mathf.Lerp(0f, 1f, a), 1f, 1f);
            yield return null;
        }

        if (visual != null) visual.localScale = Vector3.one;
        flipping = false;
    }
}