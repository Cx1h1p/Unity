using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image status;

    [Header("Status Sprites")]
    [SerializeField] private Sprite collectedSprite; // ✓
    [SerializeField] private Sprite missingSprite;   // ✗

    public void Init(Item item, bool collected)
    {
        if (icon) { icon.sprite = item.icon; icon.enabled = item.icon != null; }
        if (nameText) nameText.text = item.displayName;
        SetCollected(collected);
    }
    public void SetCollected(bool collected)
    {
        if (!status) return;

        status.sprite = collected ? collectedSprite : missingSprite;

        status.color = collected ? Color.green : Color.red;

        if (nameText)
            nameText.alpha = collected ? 1f : 0.55f;
    }

}
