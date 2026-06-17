using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialog : MonoBehaviour, IInteractable
{
    [TextArea]
    public string dialogText = "Hello! Find the items you need!";

    private bool playerInRange = false;

    public void OnInteract()
    {
        if (!playerInRange) return;

        // Show dialog on UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowQuest(dialogText);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInteractionText("Press E to talk");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractionText();
                UIManager.Instance.ShowQuest(""); 
            }
        }
    }
}
