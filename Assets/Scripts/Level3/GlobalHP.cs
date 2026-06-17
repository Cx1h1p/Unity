using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalHP : MonoBehaviour
{
    public static GlobalHP Instance;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    [Header("Settings")]
    [SerializeField] private float maxHP = 100f;

    [Header("Death")]
    [SerializeField] private GameObject deathScreen;

    private float currentHP;
    private bool isDead = false;

    void Awake()
    {
        Instance = this;
        currentHP = maxHP;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        UpdateUI();

        Debug.Log("GLOBAL HP: -" + damage);

        //  CHECK DEATH
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("PLAYER DEAD");

        //  Show death screen
        if (deathScreen != null)
            deathScreen.SetActive(true);

        //  Freeze game
        Time.timeScale = 0f;
    }

    void UpdateUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHP;

        if (hpText != null)
            hpText.text = "HP: " + Mathf.RoundToInt(currentHP);
    }
}