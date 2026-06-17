using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    public PuzzlePiece correctPiece;
    private Image slotImage;

    void Awake()
    {
        slotImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        var piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (piece == null) return;

        
        if (piece != correctPiece || !piece.IsCorrectRotation())
        {
            PuzzleManager.Instance?.PlayWrongSfx();
            return;
        }

        
        piece.SnapToSlot(transform);

        PuzzleManager.Instance?.PlayCorrectSfx(); 

        if (slotImage != null)
            slotImage.enabled = false;
    }
}