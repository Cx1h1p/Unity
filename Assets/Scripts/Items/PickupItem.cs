using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private Item item;   // ScriptableObject

    private bool playerInRange;
    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerInventory == null)
            Debug.LogError("PlayerInventory not found in scene.");
    }

    private void Update()
    {
        if (!playerInRange) return;

        // Show the "Press E" prompt while in range
        if (UIManager.Instance != null && item != null)
        {
            UIManager.Instance.ShowInteractionText(
                "Press <b><color=#FF8800>[ E ]</color></b> to pick up: <color=#000000>"
                + item.displayName +
                "</color>"
            );
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (item == null)
        {
            Debug.LogWarning("PickupItem has no Item assigned.");
            return;
        }

        if (playerInventory != null && playerInventory.TryAdd(item))
        {
            // Hide interaction prompt
            UIManager.Instance?.HideInteractionText();

            // Show pickup popup ("Item picked up: Name")
            UIManager.Instance?.ShowPickupPopup(item.displayName);

            // Disable item object
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.Instance?.HideInteractionText();
        }
    }
}