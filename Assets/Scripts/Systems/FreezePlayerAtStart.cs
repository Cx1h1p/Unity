using UnityEngine;

public class FreezePlayerAtStart : MonoBehaviour
{
    [SerializeField] private TopDownMovement2D movementScript;

    private Rigidbody2D rb;

    void Start()
    {
        if (movementScript == null)
            movementScript = FindFirstObjectByType<TopDownMovement2D>();

        if (movementScript != null)
            movementScript.enabled = false;

        rb = movementScript != null
            ? movementScript.GetComponent<Rigidbody2D>()
            : null;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void Unfreeze()
    {
        if (movementScript != null)
            movementScript.enabled = true;
    }
}