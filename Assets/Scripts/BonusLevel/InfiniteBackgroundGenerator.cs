using UnityEngine;

public class InfiniteBackgroundGenerator : MonoBehaviour
{
    [Header("Background")]
    public GameObject backgroundTilePrefab;
    public Transform playerOrCannon;

    [Header("Tile Settings")]
    public float tileWidth = 24f;
    public float generationStartX = 24f;

    public int tilesAhead = 3;
    public int tilesBehind = 1;

    private int lastCenterIndex = -999;

    void Start()
    {
        GenerateAroundPlayer();
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        GenerateAroundPlayer();
    }

    void GenerateAroundPlayer()
    {
        if (playerOrCannon == null)
            return;

        if (playerOrCannon.position.x < generationStartX)
            return;

        int centerIndex = Mathf.FloorToInt(
            (playerOrCannon.position.x - generationStartX) / tileWidth
        );

        if (centerIndex == lastCenterIndex)
            return;

        lastCenterIndex = centerIndex;

        for (int i = centerIndex - tilesBehind; i <= centerIndex + tilesAhead; i++)
        {
            string tileName = "StreetTile_" + i;

            if (GameObject.Find(tileName) != null)
                continue;

            Vector3 pos = new Vector3(
                generationStartX + i * tileWidth,
                0f,
                0f
            );

            GameObject tile = Instantiate(
                backgroundTilePrefab,
                pos,
                Quaternion.identity
            );

            tile.name = tileName;
        }
    }
}