using UnityEngine;

public class ProtesterSpawner : MonoBehaviour
{
    public static ProtesterSpawner Instance;

    [Header("Prefab")]
    public GameObject protesterPrefab;

    [Header("References")]
    public Transform waterCannon;
    public Transform spawnArea;
    public BoxCollider2D spawnAreaCollider;

    [Header("Spawning")]
    public float spawnInterval = 1f;
    public float minimumSpawnInterval = 0.15f;
    public int maxProtestersAlive = 20;

    private float nextSpawnTime;
    private int aliveProtesters = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        if (aliveProtesters >= maxProtestersAlive)
            return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnProtester();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    public void RegisterProtester()
    {
        aliveProtesters++;
    }

    public void UnregisterProtester()
    {
        aliveProtesters--;
        aliveProtesters = Mathf.Max(0, aliveProtesters);
    }

    public void IncreaseSpawnRate()
    {
        spawnInterval *= 0.7f;
        spawnInterval = Mathf.Max(spawnInterval, minimumSpawnInterval);

        Debug.Log("Spawn interval now: " + spawnInterval);
    }

    void SpawnProtester()
    {
        if (protesterPrefab == null)
        {
            Debug.LogError("Protester prefab missing.");
            return;
        }

        if (spawnAreaCollider == null || spawnArea == null || waterCannon == null)
            return;

        Bounds bounds = spawnAreaCollider.bounds;
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector3 spawnPosition = new Vector3(
            spawnArea.position.x,
            randomY,
            0f
        );

        GameObject protester = Instantiate(
            protesterPrefab,
            spawnPosition,
            Quaternion.identity
        );

        RegisterProtester();

        ProtesterChaseTarget chase =
            protester.GetComponent<ProtesterChaseTarget>();

        if (chase != null)
            chase.SetTarget(waterCannon);
    }
}