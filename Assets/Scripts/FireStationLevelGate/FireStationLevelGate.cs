using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class FireStationLevelGate : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string level2SceneName = "Level2";

    [Header("Requirements (drag your InventoryUI here)")]
    [SerializeField] private InventoryUI inventoryUI; 

    [Header("Optional UI prompt (e.g. Canvas_UI/HUD/InteractionText)")]
    [SerializeField] private TMP_Text interactionText;

    private PlayerInventory inv;
    private bool playerInRange;

    private void Start()
    {
        inv = FindFirstObjectByType<PlayerInventory>();
        if (interactionText) interactionText.gameObject.SetActive(false);

        if (inv == null) Debug.LogError("PlayerInventory not found in scene.");
        if (inventoryUI == null) Debug.LogError("InventoryUI not assigned on FireStationLevelGate.");
    }

    private void Update()
    {
        if (!playerInRange || inv == null || inventoryUI == null) return;

        bool ready = HasAllRequiredItems();

        if (interactionText)
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = ready
                ? "Press <b><color=#FF8800>[ E ]</color> to enter the FireBase</b>"
                : "Find all items first";
        }

        if (ready && Input.GetKeyDown(KeyCode.E))
        {
            var actor = FindFirstObjectByType<PlayerTransitionActor>();
            SceneTransitionManager.Instance.PlayAndLoad(level2SceneName, actor);

        }
    }

    private bool HasAllRequiredItems()
    {
        
        List<Item> required = inventoryUI.RequiredItems;
        if (required == null || required.Count == 0) return true;

        foreach (var item in required)
        {
            if (item == null) continue;
            if (!inv.Has(item.id)) return false;
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;

        if (interactionText) interactionText.gameObject.SetActive(false);
    }
}
