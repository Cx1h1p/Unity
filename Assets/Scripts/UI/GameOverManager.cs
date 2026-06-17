using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("References")]
    public GameObject worldRoot;     
    public GameObject deathScreen;
    public GameObject hudRoot;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (deathScreen != null) deathScreen.SetActive(false);
    }

    public void ShowDeath()
    {
        if (deathScreen != null)
            deathScreen.SetActive(true);

        if (hudRoot != null)
            hudRoot.SetActive(false);

        if (worldRoot != null)
            worldRoot.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
