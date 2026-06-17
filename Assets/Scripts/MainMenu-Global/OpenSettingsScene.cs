using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSettingsScene : MonoBehaviour
{
    public void OpenSettings()
    {
        SceneReturnData.PreviousSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Settings");
    }
}