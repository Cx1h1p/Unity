using UnityEngine;

public class CollisionDebug : MonoBehaviour
{
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        bool touching = col.IsTouchingLayers(LayerMask.GetMask("Ground"));
        Debug.Log(touching
            ? "Player IS touching the ground!"
            : "Player NOT touching ground");
    }
}
