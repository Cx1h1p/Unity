using UnityEngine;

public class InventoryBootstrap : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    private PlayerInventory inv;

    void Start()
    {
        inv = FindFirstObjectByType<PlayerInventory>();
        if (inv == null)
        {
            Debug.LogError("PlayerInventory not found in scene.");
            return;
        }

        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI reference not set on InventoryBootstrap.");
            return;
        }

        inventoryUI.Build(inv);

        
        inv.OnCollected += inventoryUI.MarkCollected;
    }

    void OnDestroy()
    {
        if (inv != null)
            inv.OnCollected -= inventoryUI.MarkCollected;
    }
}
