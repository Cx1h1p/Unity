using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    Vector2 input;

  
    Vector2 lastMoveDir = new Vector2(0f, 1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        
        if (animator != null)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 1f);
            animator.SetFloat("Speed", 0f);
        }
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;

        
        if (input.sqrMagnitude > 0.01f)
            lastMoveDir = input;

        
        if (animator != null)
        {
            if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
            {
                animator.SetFloat("MoveX", Mathf.Sign(lastMoveDir.x));
                animator.SetFloat("MoveY", 0f);
            }
            else
            {
                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveY", Mathf.Sign(lastMoveDir.y));
            }

            animator.SetFloat("Speed", input.sqrMagnitude);
        }

        // Flip sprite for left/right
        if (spriteRenderer != null && Mathf.Abs(lastMoveDir.x) > 0.01f)
        {
            spriteRenderer.flipX = lastMoveDir.x < 0;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }
}
