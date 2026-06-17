using UnityEngine;

public class VehicleHealth : MonoBehaviour
{
    [Header("Vehicle Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI")]
    public RectTransform healthFill;
    public float fullWidth = 500f;

    [Header("Death Screen")]
    public BonusDeathScreen bonusDeathScreen;

    public bool IsDead { get; private set; } = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            VehicleDestroyed();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthFill == null) return;

        float healthPercent = currentHealth / maxHealth;

        healthFill.sizeDelta = new Vector2(
            fullWidth * healthPercent,
            healthFill.sizeDelta.y
        );
    }

    void VehicleDestroyed()
    {
        IsDead = true;

        Debug.Log("Vehicle destroyed - opening deathscreen");

        if (bonusDeathScreen != null)
        {
            bonusDeathScreen.ShowDeathScreen();
        }
        else
        {
            Debug.LogWarning("BonusDeathScreen is not assigned on VehicleHealth.");
            Time.timeScale = 0f;
        }
    }
}