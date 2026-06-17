using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Damage Audio")]
    [SerializeField] private AudioSource hurtSource;
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private float hurtSoundCooldown = 0.4f;

    private float lastHurtSoundTime;

    public float maxHP = 100f;
    [HideInInspector] public float currentHP;
    public float regenRate = 0f;

    void Start()
    {
        currentHP = maxHP;
        UIManager.Instance?.SetHealth(currentHP, maxHP);
    }

    void Update()
    {
        if (regenRate > 0 && currentHP < maxHP)
        {
            currentHP = Mathf.Min(maxHP, currentHP + regenRate * Time.deltaTime);
            UIManager.Instance?.SetHealth(currentHP, maxHP);
        }

        if (currentHP <= 0)
        {
            GameOverManager.Instance?.ShowDeath();
            enabled = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (Time.time - lastHurtSoundTime >= hurtSoundCooldown)
        {
            if (hurtSource != null && hurtClips != null && hurtClips.Length > 0)
            {
                hurtSource.pitch = Random.Range(0.95f, 1.05f);

                AudioClip clip = hurtClips[Random.Range(0, hurtClips.Length)];
                hurtSource.PlayOneShot(clip);
            }

            lastHurtSoundTime = Time.time;
        }

        currentHP = Mathf.Clamp(currentHP - amount, 0f, maxHP);
        UIManager.Instance?.SetHealth(currentHP, maxHP);
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
        UIManager.Instance?.SetHealth(currentHP, maxHP);
    }
}