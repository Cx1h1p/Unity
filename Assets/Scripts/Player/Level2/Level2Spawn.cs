using UnityEngine;

public class Level2Spawn : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; //  SpawnPoint_Level2 

    void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Level2Spawn: SpawnPoint not assigned.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Level2Spawn: Player not found. Make sure Player tag is set.");
            return;
        }

        // Teleport player to spawn
        player.transform.position = spawnPoint.position;

        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}