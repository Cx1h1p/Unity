using System.Collections;
using UnityEngine;

public class ProtesterVehicleDamage : MonoBehaviour
{
    [Header("Vehicle")]
    public Transform vehicleDamagePoint;
    public string vehicleDamagePointName = "VehicleDamagePoint";
    public VehicleHealth vehicleHealth;

    [Header("Damage")]
    public float damageDistance = 1.1f;
    public float damageAmount = 10f;

    [Header("Hit Feedback")]
    public Color hitVehicleColor = Color.red;
    public float hitFlashTime = 0.12f;

    [Header("Fade")]
    public float fadeDuration = 0.5f;

    [Header("Sound")]
    public AudioClip[] hitVehicleClips;
    public float volume = 0.5f;

    private bool hasDamagedVehicle = false;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Collider2D col;
    private Rigidbody2D rb;
    private ProtesterChaseTarget chase;
    private ProtesterHealth protesterHealth;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        chase = GetComponent<ProtesterChaseTarget>();
        protesterHealth = GetComponent<ProtesterHealth>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        FindVehicleReferences();
    }

    void Update()
    {
        if (!BonusLevelGameState.GameplayActive)
            return;

        if (hasDamagedVehicle)
            return;

        if (protesterHealth != null && protesterHealth.IsDying)
            return;

        if (vehicleDamagePoint == null || vehicleHealth == null)
        {
            FindVehicleReferences();
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            vehicleDamagePoint.position
        );

        if (distance <= damageDistance)
        {
            hasDamagedVehicle = true;

            vehicleHealth.TakeDamage(damageAmount);

            StartCoroutine(DamageVehicleRoutine());
        }
    }

    private void FindVehicleReferences()
    {
        if (vehicleDamagePoint == null)
        {
            GameObject foundPoint =
                GameObject.Find(vehicleDamagePointName);

            if (foundPoint != null)
                vehicleDamagePoint = foundPoint.transform;
        }

        if (vehicleHealth == null)
        {
            vehicleHealth =
                FindObjectOfType<VehicleHealth>();
        }
    }

    IEnumerator DamageVehicleRoutine()
    {
        if (chase != null)
            chase.StopMoving();

        if (col != null)
            col.enabled = false;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = hitVehicleColor;

        if (audioSource != null &&
            hitVehicleClips != null &&
            hitVehicleClips.Length > 0)
        {
            int randomIndex =
                Random.Range(0, hitVehicleClips.Length);

            AudioClip clip =
                hitVehicleClips[randomIndex];

            if (clip != null)
            {
                audioSource.PlayOneShot(clip, volume);

                yield return new WaitForSeconds(
                    clip.length
                );
            }
        }
        else
        {
            yield return new WaitForSeconds(
                hitFlashTime
            );
        }

        if (spriteRenderer != null)
        {
            float timer = 0f;

            Color startColor =
                spriteRenderer.color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

                float alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        timer / fadeDuration
                    );

                spriteRenderer.color =
                    new Color(
                        startColor.r,
                        startColor.g,
                        startColor.b,
                        alpha
                    );

                yield return null;
            }
        }

        SafeDestroy.DestroyObject(gameObject, 0.2f);
    }
}