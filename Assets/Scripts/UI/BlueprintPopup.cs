using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject blueprintPanel;

    [Header("Player")]
    [SerializeField] private TopDownMovement2D playerMovement;   
    [SerializeField] private Rigidbody2D playerRb;               

    [Header("Task Settings")]
    [SerializeField] private string taskId = "Blueprint";

    private MemoryManager mm;

    private void Awake()
    {
        mm = FindFirstObjectByType<MemoryManager>();
        if (mm != null)
            mm.OnSolved += HandleSolved;

        
        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<TopDownMovement2D>();

        if (playerRb == null && playerMovement != null)
            playerRb = playerMovement.GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        if (mm != null)
            mm.OnSolved -= HandleSolved;
    }

    private void HandleSolved()
    {
        TaskManager.Instance?.CompleteTask(taskId);

        if (Level2UIManager.Instance != null)
            Level2UIManager.Instance.ShowTaskCompleted(taskId + " task completed!");

        Close();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (mm == null)
            mm = FindFirstObjectByType<MemoryManager>();

        
        if (mm != null && mm.IsSolved)
        {
            gameObject.SetActive(false);
            return;
        }

     
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRb != null)
            playerRb.velocity = Vector2.zero;

        if (dimmer != null) dimmer.SetActive(true);
        if (blueprintPanel != null) blueprintPanel.SetActive(true);

        mm?.GenerateBoard();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void Close()
    {
        if (blueprintPanel != null) blueprintPanel.SetActive(false);
        if (dimmer != null) dimmer.SetActive(false);

        
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRb != null)
            playerRb.velocity = Vector2.zero;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}