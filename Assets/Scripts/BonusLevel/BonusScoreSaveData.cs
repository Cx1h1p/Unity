using UnityEngine;

public static class BonusScoreSaveData
{
    private const string BestScoreKey = "BonusBestScore";

    public static int GetBestScore()
    {
        return PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    public static bool SaveBestScoreIfHigher(int currentScore)
    {
        int bestScore = GetBestScore();

        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt(BestScoreKey, currentScore);
            PlayerPrefs.Save();

            Debug.Log("New best score saved: " + currentScore);
            return true;
        }

        Debug.Log("Score was not higher than best score.");
        return false;
    }

    public static void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BestScoreKey);
        PlayerPrefs.Save();

        Debug.Log("Best score reset.");
    }
}