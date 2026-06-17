using UnityEngine;

public class BonusLevelGameState : MonoBehaviour
{
    public static bool GameplayActive { get; private set; } = false;

    public static void LockGameplay()
    {
        GameplayActive = false;
    }

    public static void StartGameplay()
    {
        GameplayActive = true;
    }
}