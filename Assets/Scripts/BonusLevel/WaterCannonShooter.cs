using UnityEngine;

public class WaterCannonShooter : MonoBehaviour
{
    public GameObject waterBulletPrefab;
    public Transform shootPoint;

    [Header("Shooting")]
    public float fireRate = 0.05f;

    [Header("Angle Limits")]
    public float minShootAngle = -35f;
    public float maxShootAngle = 35f;

    [Header("Sound")]
    public AudioClip shotClip;
    public float shotVolume = 0.45f;

    private float nextFireTime;
    private AudioSource audioSource;
    private Camera mainCam;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Vector2 shootDirection = GetLimitedShootDirection();

        GameObject bullet = Instantiate(
            waterBulletPrefab,
            shootPoint.position,
            Quaternion.identity
        );

        bullet.GetComponent<WaterBullet>().Init(shootDirection);

        if (audioSource != null && shotClip != null)
        {
            audioSource.PlayOneShot(shotClip, shotVolume);
        }
    }

    Vector2 GetLimitedShootDirection()
    {
        if (mainCam == null)
            return Vector2.right;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 rawDirection =
            ((Vector2)mouseWorld - (Vector2)shootPoint.position).normalized;

        float angle = Mathf.Atan2(rawDirection.y, rawDirection.x) * Mathf.Rad2Deg;

        // Prevent shooting behind the vehicle
        angle = Mathf.Clamp(angle, minShootAngle, maxShootAngle);

        float radians = angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ).normalized;
    }
}