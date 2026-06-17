using UnityEngine;

public class ProtesterChaseTarget : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float stopDistance = 1.5f;

    [Header("Vehicle Target")]
    public Transform vehicleDamagePoint;
    public string vehicleDamagePointName =
        "VehicleDamagePoint";

    [Header("Auto Destroy")]
    public bool destroyIfPastVehicle = true;
    public float extraPastDistance = 1f;

    private bool moving = true;
    private bool startedLeftOfTarget;

    void Start()
    {
        FindVehicleDamagePoint();

        if (vehicleDamagePoint != null)
        {
            startedLeftOfTarget =
                transform.position.x <
                vehicleDamagePoint.position.x;
        }
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        if (!moving)
            return;

        if (vehicleDamagePoint == null)
        {
            FindVehicleDamagePoint();
            return;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                vehicleDamagePoint.position
            );

        if (distance > stopDistance)
        {
            Vector2 direction =
                (
                    (Vector2)vehicleDamagePoint.position -
                    (Vector2)transform.position
                ).normalized;

            transform.position +=
                (Vector3)(
                    direction *
                    speed *
                    Time.deltaTime
                );
        }

        if (destroyIfPastVehicle)
        {
            if (
                startedLeftOfTarget &&
                transform.position.x >
                vehicleDamagePoint.position.x +
                extraPastDistance
            )
            {
                SafeDestroy.DestroyObject(gameObject, 0.2f);
            }

            if (
                !startedLeftOfTarget &&
                transform.position.x <
                vehicleDamagePoint.position.x -
                extraPastDistance
            )
            {
                SafeDestroy.DestroyObject(gameObject, 0.2f);
            }
        }
    }

    private void FindVehicleDamagePoint()
    {
        GameObject foundPoint =
            GameObject.Find(vehicleDamagePointName);

        if (foundPoint != null)
        {
            vehicleDamagePoint =
                foundPoint.transform;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        vehicleDamagePoint = newTarget;

        if (vehicleDamagePoint != null)
        {
            startedLeftOfTarget =
                transform.position.x <
                vehicleDamagePoint.position.x;
        }
    }

    public void StopMoving()
    {
        moving = false;
    }
}