using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerController2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float jumpForce = 7f;

    [Header("Ground")]
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float fallGravityMultiplier = 3f;

    private Rigidbody2D rb;
    private Collider2D col;
    private PlayerController2D controller;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        controller = GetComponent<PlayerController2D>();

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Animation
        float animSpeed = Mathf.Abs(rb.velocity.x) / speed;
        animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);


        // Flip sprite
        if (moveInput > 0f)
            spriteRenderer.flipX = false;
        else if (moveInput < 0f)
            spriteRenderer.flipX = true;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Grounded check
        controller.isGrounded =
            col.IsTouchingLayers(groundLayer) &&
            rb.velocity.y <= 0.01f;

        // Horizontal movement
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Jump
        if (jumpRequested)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequested = false;
        }

        // Stronger gravity when falling
        if (!controller.isGrounded && rb.velocity.y < 0f)
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
}
