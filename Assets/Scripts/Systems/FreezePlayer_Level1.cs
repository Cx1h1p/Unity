using UnityEngine;
using System.Collections;

public class FreezePlayer_Level1 : MonoBehaviour
{
    [SerializeField] private MonoBehaviour movementScript; 

    private Rigidbody2D rb;

    void Start()
    {
        if (movementScript == null)
            Debug.LogError("Movement script not assigned!");

        // disable movement
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

        //  auto unfreeze after 3 seconds
        StartCoroutine(UnfreezeAfterDelay(3f));
    }

    IEnumerator UnfreezeAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (movementScript != null)
            movementScript.enabled = true;
    }
}