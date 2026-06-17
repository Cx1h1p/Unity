using UnityEngine;

public static class GameSaveData
{
    private const string BonusUnlockedKey = "BonusUnlocked";

    public static bool IsBonusUnlocked()
    {
        return PlayerPrefs.GetInt(BonusUnlockedKey, 0) == 1;
    }

    public static void UnlockBonusLevel()
    {
        PlayerPrefs.SetInt(BonusUnlockedKey, 1);
        PlayerPrefs.Save();
    }
}