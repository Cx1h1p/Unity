using System.Collections;
using UnityEngine;
using TMPro;

public class BonusDeathScreenScoreDisplay : MonoBehaviour
{
    [Header("Score Texts")]
    [SerializeField] private TMP_Text yourScoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Count Animation")]
    [SerializeField] private float countDuration = 1.2f;

    [Header("Count Sound")]
    [SerializeField] private AudioSource countAudioSource;
    [SerializeField] private AudioClip countClip;

    [Range(0f, 1f)]
    [SerializeField] private float countVolume = 1f;

    [Header("New High Score Popup")]
    [SerializeField] private NewHighScoreAnimation newHighScoreAnimation;

    [Header("Best Score Center Count")]
    [SerializeField] private float bestScoreBigFontSize = 200f;
    [SerializeField] private float moveToCenterDuration = 0.35f;
    [SerializeField] private float stayAfterCountDuration = 0.5f;
    [SerializeField] private float returnDuration = 0.35f;

    [Header("Cool Text Effect")]
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float pulseScale = 1.15f;

    private Coroutine mainRoutine;

    public void UpdateScoreDisplay()
    {
        if (mainRoutine != null)
            StopCoroutine(mainRoutine);

        mainRoutine = StartCoroutine(UpdateScoreDisplayRoutine());
    }

    private IEnumerator UpdateScoreDisplayRoutine()
    {
        int currentScore = 0;

        if (ScoreManager.Instance != null)
            currentScore = ScoreManager.Instance.CurrentScore;

        bool isNewHighScore = BonusScoreSaveData.SaveBestScoreIfHigher(currentScore);
        int bestScore = BonusScoreSaveData.GetBestScore();

        if (yourScoreText != null)
            yourScoreText.text = "0";

        if (bestScoreText != null)
            bestScoreText.text = "0";

        if (isNewHighScore && newHighScoreAnimation != null)
        {
            yield return newHighScoreAnimation.PlayAndWait();

            yield return CountCurrentScoreAndBestScoreCenter(
                currentScore,
                bestScore
            );
        }
        else
        {
            yield return CountScoresNormal(currentScore, bestScore);
        }
    }

    private IEnumerator CountScoresNormal(int currentScore, int bestScore)
    {
        PlayCountingSound(countVolume);

        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / countDuration);

            int shownScore = Mathf.RoundToInt(Mathf.Lerp(0, currentScore, progress));
            int shownBest = Mathf.RoundToInt(Mathf.Lerp(0, bestScore, progress));

            if (yourScoreText != null)
                yourScoreText.text = shownScore.ToString();

            if (bestScoreText != null)
                bestScoreText.text = shownBest.ToString();

            yield return null;
        }

        if (yourScoreText != null)
            yourScoreText.text = currentScore.ToString();

        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString();

        StopCountingSound();
    }

    private IEnumerator CountCurrentScoreAndBestScoreCenter(
        int currentScore,
        int bestScore
    )
    {
        if (bestScoreText == null)
            yield break;

        RectTransform bestRect = bestScoreText.GetComponent<RectTransform>();

        Vector2 originalBestPosition = bestRect.anchoredPosition;
        Vector3 originalBestScale = bestRect.localScale;
        float originalBestFontSize = bestScoreText.fontSize;
        Color originalBestColor = bestScoreText.color;
        FontStyles originalBestStyle = bestScoreText.fontStyle;

        bestScoreText.fontStyle = FontStyles.Bold;
        bestScoreText.enableWordWrapping = false;

        Vector2 centerPosition = Vector2.zero;

        float timer = 0f;

        while (timer < moveToCenterDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / moveToCenterDuration);

            bestRect.anchoredPosition = Vector2.Lerp(
                originalBestPosition,
                centerPosition,
                t
            );

            bestScoreText.fontSize = Mathf.Lerp(
                originalBestFontSize,
                bestScoreBigFontSize,
                t
            );

            bestRect.localScale = Vector3.Lerp(
                originalBestScale,
                Vector3.one,
                t
            );

            yield return null;
        }

        bestRect.anchoredPosition = centerPosition;
        bestScoreText.fontSize = bestScoreBigFontSize;
        bestScoreText.text = "0";

        PlayCountingSound(countVolume * 2f);

        timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / countDuration);

            int shownCurrent = Mathf.RoundToInt(
                Mathf.Lerp(0, currentScore, progress)
            );

            int shownBest = Mathf.RoundToInt(
                Mathf.Lerp(0, bestScore, progress)
            );

            if (yourScoreText != null)
                yourScoreText.text = shownCurrent.ToString();

            bestScoreText.text = shownBest.ToString();

            float pulse =
                1f + Mathf.Sin(progress * Mathf.PI * 10f) * 0.08f;

            bestRect.localScale = Vector3.one * pulse;

            bestScoreText.color = Color.Lerp(
                originalBestColor,
                glowColor,
                Mathf.PingPong(Time.unscaledTime * 4f, 1f)
            );

            yield return null;
        }

        if (yourScoreText != null)
            yourScoreText.text = currentScore.ToString();

        bestScoreText.text = bestScore.ToString();
        bestRect.localScale = Vector3.one * pulseScale;

        StopCountingSound();

        yield return new WaitForSecondsRealtime(stayAfterCountDuration);

        timer = 0f;

        while (timer < returnDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / returnDuration);

            bestRect.anchoredPosition = Vector2.Lerp(
                centerPosition,
                originalBestPosition,
                t
            );

            bestScoreText.fontSize = Mathf.Lerp(
                bestScoreBigFontSize,
                originalBestFontSize,
                t
            );

            bestRect.localScale = Vector3.Lerp(
                Vector3.one * pulseScale,
                originalBestScale,
                t
            );

            bestScoreText.color = Color.Lerp(
                glowColor,
                originalBestColor,
                t
            );

            yield return null;
        }

        bestRect.anchoredPosition = originalBestPosition;
        bestRect.localScale = originalBestScale;
        bestScoreText.fontSize = originalBestFontSize;
        bestScoreText.color = originalBestColor;
        bestScoreText.fontStyle = originalBestStyle;
    }

    private void PlayCountingSound(float volume)
    {
        if (countAudioSource != null && countClip != null)
        {
            countAudioSource.clip = countClip;
            countAudioSource.volume = Mathf.Clamp01(volume);
            countAudioSource.loop = true;
            countAudioSource.Play();
        }
    }

    private void StopCountingSound()
    {
        if (countAudioSource != null && countAudioSource.isPlaying)
            countAudioSource.Stop();
    }
}