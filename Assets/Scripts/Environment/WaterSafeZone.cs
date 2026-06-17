using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSafeZone : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private ParticleSystem enterSplash;
    [SerializeField] private ParticleSystem exitSplash;
    [SerializeField] private ParticleSystem loopSprinkle;
    [SerializeField] private bool sprinkleFollowsPlayer = true;

    [Header("Audio")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioClip enterReliefClip;
    [SerializeField] private float targetAmbienceVolume = 0.2f;
    [SerializeField] private float fadeTime = 0.35f;

    [Header("Splash SFX")]
    [SerializeField] private AudioClip waterSplashClip;
    [SerializeField] private float splashVolume = 0.8f;
    [SerializeField] private float splashCooldown = 0.15f;

    private float lastSplashTime;
    private Coroutine fadeRoutine;

    [Header("Healing")]
    public float regenRateInWater = 5f;
    public float healDelay = 0.5f;

    private readonly Dictionary<PlayerHealth, float> enterTime = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //  SHOW BLUE DIMMER
        DamageHealDimmer.Instance?.ShowHealHold();

        
        if (ambienceSource && waterSplashClip && Time.time - lastSplashTime >= splashCooldown)
        {
            ambienceSource.pitch = Random.Range(0.95f, 1.05f);
            ambienceSource.PlayOneShot(waterSplashClip, splashVolume);
            lastSplashTime = Time.time;
        }

        Vector3 p = other.transform.position;

        //  ENTER SPLASH
        PlaySplashAt(enterSplash, p);

        //  LOOP SPRINKLE
        if (loopSprinkle)
        {
            if (sprinkleFollowsPlayer)
                loopSprinkle.transform.position =
                    new Vector3(p.x, p.y, loopSprinkle.transform.position.z);

            loopSprinkle.Play();
        }

        //  AMBIENCE
        if (ambienceSource)
        {
            if (enterReliefClip)
                ambienceSource.PlayOneShot(enterReliefClip, 1f);

            FadeAmbience(targetAmbienceVolume);
        }

        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph != null)
            enterTime[ph] = Time.time;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph == null) return;

        //  FOLLOW PLAYER
        if (sprinkleFollowsPlayer && loopSprinkle && loopSprinkle.isPlaying)
        {
            Vector3 p = other.transform.position;

            loopSprinkle.transform.position =
                new Vector3(p.x, p.y, loopSprinkle.transform.position.z);
        }

        //  ENABLE REGEN
        if (enterTime.TryGetValue(ph, out float t) &&
            Time.time - t >= healDelay)
        {
            ph.regenRate = regenRateInWater;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //  HIDE DIMMER
        DamageHealDimmer.Instance?.Hide();

        Vector3 p = other.transform.position;

        //  EXIT SPLASH
        PlaySplashAt(exitSplash, p);

        //  STOP LOOP
        if (loopSprinkle)
        {
            loopSprinkle.Stop(true,
                ParticleSystemStopBehavior.StopEmitting);
        }

        //  FADE OUT AMBIENCE
        FadeAmbience(0f);

        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.regenRate = 0f;
            enterTime.Remove(ph);
        }
    }

    private void PlaySplashAt(ParticleSystem ps, Vector3 worldPos)
    {
        if (!ps) return;

        ps.transform.position =
            new Vector3(worldPos.x, worldPos.y, ps.transform.position.z);

        ps.Play();
    }

    private void FadeAmbience(float to)
    {
        if (!ambienceSource) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(to));
    }

    private IEnumerator FadeRoutine(float to)
    {
        if (!ambienceSource.isPlaying)
            ambienceSource.Play();

        float from = ambienceSource.volume;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            ambienceSource.volume =
                Mathf.Lerp(from, to, t / fadeTime);

            yield return null;
        }

        ambienceSource.volume = to;

        if (Mathf.Approximately(to, 0f))
            ambienceSource.Stop();
    }
}