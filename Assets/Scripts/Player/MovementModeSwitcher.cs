using UnityEngine;
using UnityEngine.SceneManagement;

public class MovementModeSwitcher : MonoBehaviour
{
    [Header("Disable in TopDown Levels (Level2, Level3)")]
    [SerializeField] private Behaviour platformMovement;
    [SerializeField] private Behaviour platformController;

    [Header("Enable in TopDown Levels (Level2, Level3)")]
    [SerializeField] private Behaviour topDownMovement;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Apply(gameObject.scene.name);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Settings")
            return;

        Apply(scene.name);
    }

    void Apply(string sceneName)
    {
        bool isTopDown = sceneName == "Level2" || sceneName == "Level3";

        if (platformMovement != null)
            platformMovement.enabled = !isTopDown;

        if (platformController != null)
            platformController.enabled = !isTopDown;

        if (topDownMovement != null)
            topDownMovement.enabled = isTopDown;

        if (rb != null)
        {
            if (isTopDown)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                rb.drag = 10f;
                rb.freezeRotation = true;
            }
            else
            {
                rb.gravityScale = 3f;
                rb.drag = 0f;
                rb.freezeRotation = true;
            }
        }

        Debug.Log("Movement mode applied for scene: " + sceneName + " | TopDown: " + isTopDown);
    }
}