using UnityEngine;

public class PlayerTransitionActor : MonoBehaviour
{
    [Header("Sprite swap")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite faceCameraSprite;

    [Header("Disable these scripts during transition")]
    [SerializeField] private Behaviour[] scriptsToDisable;

    private Sprite originalSprite;
    private Vector3 originalScale;

    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalSprite = sr.sprite;
            originalScale = sr.transform.localScale;
        }
    }

    
    public void Freeze(bool frozen)
    {
        // stop physics movement
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
           
        }

        // stop animator from overriding sprites
        if (anim != null)
            anim.enabled = !frozen;

        // stop any movement scripts etc.
        if (scriptsToDisable != null)
        {
            foreach (var b in scriptsToDisable)
                if (b != null) b.enabled = !frozen;
        }
    }

    public void FaceCameraSprite(bool on)
    {
        if (sr == null) return;

        if (on && faceCameraSprite != null)
        {
            sr.sprite = faceCameraSprite;
        }
        else
        {
            sr.sprite = originalSprite;
        }

        
        sr.transform.localScale = originalScale;
    }
}
