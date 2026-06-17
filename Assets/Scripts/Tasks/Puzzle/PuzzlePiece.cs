using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rt;
    private Canvas canvas;
    private CanvasGroup cg;

    private Transform originalParent;
    private Vector2 originalAnchoredPos;

    public bool locked { get; private set; }

    private int currentRotation; 
    private bool animating = false;

    [Header("Animations")]
    public float snapDuration = 0.12f;
    public float popScale = 1.12f;
    public float popDuration = 0.10f;

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
       
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked || animating) return;

        if (eventData.button == PointerEventData.InputButton.Right)
            Rotate90();
    }

    private void Rotate90()
    {
        if (rt == null) rt = GetComponent<RectTransform>();
        if (rt == null) return;

        currentRotation = (currentRotation + 90) % 360;
        rt.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void RandomizeRotation()
    {
        if (locked) return;

        if (rt == null) rt = GetComponent<RectTransform>();
        if (rt == null) return;

        currentRotation = Random.Range(0, 4) * 90;
        rt.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public bool IsCorrectRotation() => currentRotation == 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (locked || animating) return;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        originalParent = transform.parent;
        originalAnchoredPos = rt.anchoredPosition;

        transform.SetParent(canvas.transform, true);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (locked || animating) return;
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (locked || animating) return;

        cg.blocksRaycasts = true;

        
        if (!locked)
        {
            transform.SetParent(originalParent, true);
            rt.anchoredPosition = originalAnchoredPos;
        }
    }

    public void SnapToSlot(Transform slot)
    {
        if (locked || animating) return;
        StartCoroutine(SnapRoutine(slot));
    }

    private IEnumerator SnapRoutine(Transform slot)
    {
        animating = true;
        locked = true;

        cg.blocksRaycasts = true;

        transform.SetParent(slot, false);

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;

        Vector2 startMin = rt.offsetMin;
        Vector2 startMax = rt.offsetMax;

        Vector2 targetMin = Vector2.zero;
        Vector2 targetMax = Vector2.zero;

        float t = 0f;
        while (t < snapDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / snapDuration);
            float eased = 1f - Mathf.Pow(1f - a, 3f);

            rt.offsetMin = Vector2.Lerp(startMin, targetMin, eased);
            rt.offsetMax = Vector2.Lerp(startMax, targetMax, eased);

            yield return null;
        }

        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        currentRotation = 0;
        rt.localRotation = Quaternion.identity;

        Vector3 baseScale = Vector3.one;
        Vector3 peakScale = baseScale * popScale;

        t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / popDuration);
            float eased = 1f - Mathf.Pow(1f - a, 3f);
            rt.localScale = Vector3.Lerp(baseScale, peakScale, eased);
            yield return null;
        }

        t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / popDuration);
            float eased = 1f - Mathf.Pow(1f - a, 3f);
            rt.localScale = Vector3.Lerp(peakScale, baseScale, eased);
            yield return null;
        }

        rt.localScale = Vector3.one;
        transform.SetAsLastSibling();

        animating = false;

        PuzzleManager.Instance?.CheckCompletion();
    }

   
    public void ResetPiece()
    {
        locked = false;
        animating = false;
        currentRotation = 0;

        if (rt == null) rt = GetComponent<RectTransform>();
        if (rt != null) rt.localRotation = Quaternion.identity;

        if (cg == null) cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = true;
    }
}