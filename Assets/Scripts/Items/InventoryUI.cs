using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Required items for Level 1")]
    [SerializeField] private List<Item> requiredItems = new List<Item>();


    public List<Item> RequiredItems => requiredItems;

    [Header("UI References")]
    [SerializeField] private Transform rowsParent;   
    [SerializeField] private GameObject rowPrefab;   

    private readonly Dictionary<string, InventoryRowUI> rowById =
        new Dictionary<string, InventoryRowUI>();

    public void Build(PlayerInventory inv)
    {
        if (!rowsParent || !rowPrefab || inv == null) return;

        
        for (int i = rowsParent.childCount - 1; i >= 0; i--)
            Destroy(rowsParent.GetChild(i).gameObject);

        rowById.Clear();

       
        foreach (var item in requiredItems)
        {
            if (item == null || string.IsNullOrEmpty(item.id)) continue;

            var go = Instantiate(rowPrefab, rowsParent);
            var row = go.GetComponent<InventoryRowUI>();

            bool has = inv.Has(item.id);
            row.Init(item, has);

            rowById[item.id] = row;
        }
    }

    
    public void MarkCollected(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        if (rowById.TryGetValue(itemId, out var row))
            row.SetCollected(true);
    }


    public bool AllItemsCollected(PlayerInventory inv)
    {
        if (inv == null) return false;

        foreach (var item in requiredItems)
        {
            if (item == null) continue;
            if (!inv.Has(item.id))
                return false;
        }

        return true;
    }
}
