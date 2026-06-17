using UnityEngine;

public class WaterCannonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 3f;
    public float verticalSpeed = 4f;

    [Header("Vertical Limits")]
    public float minY = -3.5f;
    public float maxY = 3.5f;

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
            verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;

        Vector3 movement = new Vector3(
            forwardSpeed,
            verticalInput * verticalSpeed,
            0f
        );

        transform.position += movement * Time.deltaTime;

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}