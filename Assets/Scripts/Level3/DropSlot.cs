using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private SparkController sparkController;
    [SerializeField] private DimmerFeedback dimmer;
    [SerializeField] private WireTaskManager taskManager;

    [SerializeField] private WrongWireVideoHandler wrongWireVideoHandler;

    [Header("Correct ID")]
    public string correctID;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragged = eventData.pointerDrag;

        if (dragged == null)
            return;

        PartSlot part = dragged.GetComponent<PartSlot>();
        DraggablePart drag = dragged.GetComponent<DraggablePart>();

        if (part == null || drag == null)
            return;

        drag.MarkDroppedOnSlot();

        // CORRECT WIRE
        if (part.partID == correctID)
        {
            dragged.transform.SetParent(transform, false);

            RectTransform rect = dragged.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            drag.Lock();

            if (dimmer != null)
                dimmer.ShowCorrect();
        }
        // WRONG WIRE
        else
        {
            drag.ResetToStart();

            if (wrongWireVideoHandler != null)
            {
                wrongWireVideoHandler.PlayWrongWireSequence();
            }
            else
            {
                // Fallback if no video handler assigned

                if (sparkController != null)
                    sparkController.PlayEffect();

                if (dimmer != null)
                    dimmer.ShowWrong();

                float damage = Random.Range(10f, 26f);

                if (GlobalHP.Instance != null)
                    GlobalHP.Instance.TakeDamage(damage);

                Debug.Log("Damage taken: " + damage);
            }
        }

        if (taskManager != null)
        {
            Debug.Log("Calling CheckCompletion");
            taskManager.CheckCompletion();
        }
    }

    public bool IsCorrect()
    {
        if (transform.childCount == 0)
            return false;

        Transform child = transform.GetChild(0);

        PartSlot part = child.GetComponent<PartSlot>();

        if (part == null)
            return false;

        return part.partID == correctID;
    }
}