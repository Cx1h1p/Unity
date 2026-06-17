using System.Collections;
using UnityEngine;

public class ProtesterHealth : MonoBehaviour
{
    [Header("Health")]
    public int minHitsToDie = 1;
    public int maxHitsToDie = 3;

    [Header("Hit Feedback")]
    public Color hitColor = new Color(0.2f, 0.7f, 1f, 1f);
    public float hitFlashTime = 0.12f;

    [Header("Death Fade")]
    public float fadeDuration = 0.5f;

    [Header("Sound")]
    public AudioClip hitClip;
    public AudioClip deathClip;
    public float volume = 0.5f;

    public bool IsDying { get; private set; } = false;

    private int currentHits;
    private int hitsNeeded;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private AudioSource audioSource;
    private Collider2D col;
    private ProtesterChaseTarget chase;
    private ProtesterVehicleDamage vehicleDamage;

    private static bool hitSoundPlaying = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        chase = GetComponent<ProtesterChaseTarget>();
        vehicleDamage = GetComponent<ProtesterVehicleDamage>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        hitsNeeded = Random.Range(minHitsToDie, maxHitsToDie + 1);
    }

    public void TakeHit()
    {
        if (IsDying)
            return;

        currentHits++;

        StopCoroutine(nameof(HitFlash));
        StartCoroutine(HitFlash());

        if (currentHits >= hitsNeeded)
        {
            StartCoroutine(DieRoutine());
        }
        else
        {
            TryPlayHitSound();
        }
    }

    private void TryPlayHitSound()
    {
        if (hitClip == null)
            return;

        if (hitSoundPlaying)
            return;

        StartCoroutine(HitSoundRoutine());
    }

    private IEnumerator HitSoundRoutine()
    {
        hitSoundPlaying = true;

        audioSource.PlayOneShot(hitClip, volume);

        yield return new WaitForSeconds(hitClip.length);

        hitSoundPlaying = false;
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitFlashTime);

        if (!IsDying)
            spriteRenderer.color = originalColor;
    }

    IEnumerator DieRoutine()
    {
        IsDying = true;

        if (col != null)
            col.enabled = false;

        if (chase != null)
            chase.enabled = false;

        if (vehicleDamage != null)
            vehicleDamage.enabled = false;

        TryPlayHitSound();

        if (hitClip != null)
            yield return new WaitForSeconds(hitClip.length);

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip, volume);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(10);

        if (spriteRenderer != null)
        {
            float timer = 0f;
            Color startColor = spriteRenderer.color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

                float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

                spriteRenderer.color = new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    alpha
                );

                yield return null;
            }
        }

        if (ProtesterSpawner.Instance != null)
            ProtesterSpawner.Instance.UnregisterProtester();

        SafeDestroy.DestroyObject(gameObject, 0.2f);
    }

    private void OnDestroy()
    {
        if (ProtesterSpawner.Instance != null && !IsDying)
        {
            ProtesterSpawner.Instance.UnregisterProtester();
        }
    }
}