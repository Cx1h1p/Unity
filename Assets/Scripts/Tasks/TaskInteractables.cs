using UnityEngine;

public class TaskInteractable : MonoBehaviour
{
    [Header("Task")]
    public string taskId = "Puzzle";
    private bool playerInside;

    [Header("Blueprint (only used if taskId == Blueprint)")]
    [SerializeField] private BlueprintPopup blueprintPopup;

    [Header("Generic panel (PuzzlePanel, CraftingPanel, etc.)")]
    [SerializeField] private GameObject taskPanel;

    [Header("UI Text")]
    [SerializeField] private string interactHint = "Press <b><color=#FF8800>[ E ]</color></b> to interact: ";
    [SerializeField] private string completedHint = "<b><color=#00AA00>Task completed!</color></b>";

    [Header("Code Task Gate Message")]
    [SerializeField]
    private string codeGateMessage =
        "You need to complete all other tasks before being able to finish the code task.";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        ShowCorrectPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (Level2UIManager.Instance != null)
            Level2UIManager.Instance.HideInteraction();
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Level2UIManager.Instance != null)
                Level2UIManager.Instance.HideInteraction();

            
            if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(taskId))
            {
                if (Level2UIManager.Instance != null)
                    Level2UIManager.Instance.ShowInteraction(completedHint);

                return;
            }

            
            if (taskId == "Code" && !AllOtherTasksCompletedExceptCode())
            {
                if (Level2UIManager.Instance != null)
                    Level2UIManager.Instance.ShowInteraction(codeGateMessage);

                return;
            }

            
            if (taskId == "Blueprint")
            {
                if (blueprintPopup == null)
                    blueprintPopup = FindFirstObjectByType<BlueprintPopup>();

                if (blueprintPopup != null)
                {
                    blueprintPopup.Open();
                }
                else
                {
                    Debug.LogError("BlueprintPopup not found. Add BlueprintPopup to a scene object and assign it.");
                }

                return;
            }

           
            if (taskPanel != null)
            {
                taskPanel.SetActive(true);
                return;
            }

            Debug.Log("Started task: " + taskId + " (no panel assigned)");
        }
    }

    private void ShowCorrectPrompt()
    {
        if (Level2UIManager.Instance == null) return;

        bool completed = TaskManager.Instance != null && TaskManager.Instance.IsCompleted(taskId);

       
        if (!completed && taskId == "Code" && !AllOtherTasksCompletedExceptCode())
        {
            Level2UIManager.Instance.ShowInteraction(codeGateMessage);
            return;
        }

        if (completed)
        {
            Level2UIManager.Instance.ShowInteraction(completedHint);
        }
        else
        {
            Level2UIManager.Instance.ShowInteraction(
                interactHint + "<color=#000000>" + taskId + "</color>"
            );
        }
    }

    private bool AllOtherTasksCompletedExceptCode()
    {
        if (TaskManager.Instance == null) return true;

        foreach (var kv in TaskManager.Instance.GetAllTasks())
        {
            if (kv.Key == "Code") continue;
            if (!kv.Value) return false;
        }

        return true;
    }
}