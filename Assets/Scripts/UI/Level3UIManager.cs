using UnityEngine;
using TMPro;

public class Level3UIManager : MonoBehaviour
{
    public static Level3UIManager Instance;

    [Header("Interaction UI")]
    [SerializeField] private GameObject interactionTextObject;
    [SerializeField] private TextMeshProUGUI interactionText;

    private void Awake()
    {
        Instance = this;
        HideInteraction();
    }

    public void ShowInteraction(string message)
    {
        if (interactionTextObject == null || interactionText == null) return;

        interactionTextObject.SetActive(true);
        interactionText.text = message;
    }

    public void HideInteraction()
    {
        if (interactionTextObject == null) return;

        interactionTextObject.SetActive(false);
    }
}