using UnityEngine;

public class TopDownAnimSmooth : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sr;

    string currentState;

    enum Face { Back, Front, Side }
    Face face = Face.Back;

    float faceHold = 0.08f;   
    float faceTimer = 0f;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Play(string state)
    {
        if (currentState == state) return;
        currentState = state;
        anim.Play(state);
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        bool moving = (x != 0 || y != 0);

        
        if (faceTimer <= 0f && moving)
        {
            if (Mathf.Abs(x) > Mathf.Abs(y)) face = Face.Side;
            else face = (y > 0) ? Face.Back : Face.Front;

            faceTimer = faceHold;
        }
        else
        {
            faceTimer -= Time.deltaTime;
        }

        
        if (face == Face.Side)
        {
            sr.flipX = (x < 0); 
            Play(moving ? "WalkSide" : "Idle_Back");
        }
        else if (face == Face.Back)
        {
            sr.flipX = false;
            Play(moving ? "WalkBack" : "Idle_Back");
        }
        else 
        {
            sr.flipX = false;
            Play(moving ? "WalkFront" : "Idle_Back");
        }
    }
}
