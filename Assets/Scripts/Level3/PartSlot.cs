using UnityEngine;
using UnityEngine.UI;

public class PartSlot : MonoBehaviour
{
    public string partID = "";

    private Image img;

    [Header("Sprites")]
    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite yellowSprite;

    void Awake()
    {
        img = GetComponent<Image>();

        if (img == null)
            Debug.LogError(gameObject.name + " is missing Image component!");
    }

    void OnEnable()
    {
        RefreshVisual(); 
    }

    public void SetPart(string id)
    {
        partID = id;
        RefreshVisual();
    }

    void RefreshVisual()
    {
        if (img == null) return;

        switch (partID)
        {
            case "RED":
                img.sprite = redSprite;
                break;

            case "BLUE":
                img.sprite = blueSprite;
                break;

            case "YELLOW":
                img.sprite = yellowSprite;
                break;

            default:
                img.sprite = null;
                break;
        }
    }

    public void CopyFrom(PartSlot other)
    {
        if (other == null) return;

        partID = other.partID;
        RefreshVisual();
    }
}