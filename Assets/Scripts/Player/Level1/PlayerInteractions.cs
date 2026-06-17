using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
public class PlayerInteraction : MonoBehaviour
{
    public float interactRadius = 1.5f;
    public LayerMask interactMask;
    public KeyCode interactKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactMask);
            foreach (var c in hits)
            {
                IInteractable interactable = c.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                    break;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
