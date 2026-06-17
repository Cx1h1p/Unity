using UnityEngine;

public class WaterCannonInteractable : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject taskPanel; 
    [SerializeField]
    private string interactMessage =
        "Press <b><color=#FF8800>[ E ]</color></b> to repair the <b>Water Cannon</b>";

    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        if (Level3UIManager.Instance != null)
            Level3UIManager.Instance.ShowInteraction(interactMessage);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (Level3UIManager.Instance != null)
            Level3UIManager.Instance.HideInteraction();
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Level3UIManager.Instance != null)
                Level3UIManager.Instance.HideInteraction();

            if (taskPanel != null)
                taskPanel.SetActive(true);
            else
                Debug.LogWarning("WaterCannonInteractable: No taskPanel assigned.");
        }
    }
}