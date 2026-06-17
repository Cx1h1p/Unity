using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePart : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startPosition;
    private Transform startParent;

    private bool locked = false;
    private bool droppedOnSlot = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        SaveStart();
    }

    public void SaveStart()
    {
        startParent = transform.parent;
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (locked) return;

        droppedOnSlot = false;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (locked) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (locked) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (!droppedOnSlot)
        {
            ResetToStart();
        }
    }

    public void MarkDroppedOnSlot()
    {
        droppedOnSlot = true;
    }

    public void ResetToStart()
    {
        transform.SetParent(startParent, false);
        rectTransform.anchoredPosition = startPosition;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void Lock()
    {
        locked = true;
        droppedOnSlot = true;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1f;
    }
}