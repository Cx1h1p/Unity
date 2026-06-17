using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;

    private int score = 0;
    private float scoreMultiplier = 1f;

    public int CurrentScore => score;

    private void Awake()
    {
        Instance = this;

        Debug.Log("ScoreManager Awake - Instance set");

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        int finalAmount = Mathf.RoundToInt(amount * scoreMultiplier);

        score += finalAmount;

        Debug.Log("Score added: " + finalAmount + " | Total: " + score);

        UpdateUI();
    }

    public void IncreaseScoreMultiplier()
    {
        scoreMultiplier *= 1.10f;

        Debug.Log("Score multiplier increased: " + scoreMultiplier);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreText is not assigned.");
        }
    }
}