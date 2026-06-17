using UnityEngine;

public class WaterBullet : MonoBehaviour
{
    public float speed = 12f;
    public float gravity = 2f;
    public float lifeTime = 2f;

    private Vector2 velocity;

    public void Init(Vector2 direction)
    {
        velocity = direction.normalized * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        velocity += Vector2.down * gravity * Time.deltaTime;

        transform.position += (Vector3)(velocity * Time.deltaTime);

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Protester"))
        {
            ProtesterHealth health = other.GetComponent<ProtesterHealth>();

            if (health != null)
            {
                health.TakeHit();
            }

            Destroy(gameObject);
        }
    }
}