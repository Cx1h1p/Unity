using UnityEngine;

public class SpawnAreaFollow : MonoBehaviour
{
    public Transform waterCannon;

    [Header("Offset From Vehicle")]
    public float offsetX = 18f;
    public float offsetY = 0f;

    void LateUpdate()
    {
        if (waterCannon == null)
            return;

        transform.position = new Vector3(
            waterCannon.position.x + offsetX,
            waterCannon.position.y + offsetY,
            transform.position.z
        );
    }
}