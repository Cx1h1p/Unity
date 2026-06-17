using UnityEngine;
using TMPro;

public class BonusLevelTimer : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;

    [Header("References")]
    public ProtesterSpawner protesterSpawner;
    public VehicleHealth vehicleHealth;
    public CanvasGroup deathScreenCanvas;

    [Header("Difficulty")]
    public float spawnIncreaseEvery = 20f;
    public float scoreIncreaseEvery = 10f;

    private float elapsedTime = 0f;
    private float nextSpawnIncreaseTime = 20f;
    private float nextScoreIncreaseTime = 10f;

    private bool stopped = false;

    void Update()
    {
        if (stopped)
            return;

        if (vehicleHealth != null && vehicleHealth.IsDead)
        {
            StopTimer();
            return;
        }

        if (deathScreenCanvas != null && deathScreenCanvas.alpha >= 1f)
        {
            StopTimer();
            return;
        }

        if (!BonusLevelGameState.GameplayActive)
            return;

        elapsedTime += Time.deltaTime;

        UpdateTimerUI();

        if (elapsedTime >= nextSpawnIncreaseTime)
        {
            if (protesterSpawner != null)
                protesterSpawner.IncreaseSpawnRate();

            nextSpawnIncreaseTime += spawnIncreaseEvery;
        }

        if (elapsedTime >= nextScoreIncreaseTime)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.IncreaseScoreMultiplier();

            nextScoreIncreaseTime += scoreIncreaseEvery;
        }
    }

    private void StopTimer()
    {
        stopped = true;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text =
            minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}