using UnityEngine;

public class OpenEscMenuButton : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource uiSource;
    public AudioClip clickClip;
    public float volume = 0.8f;

    public void OpenEscMenu()
    {
        if (uiSource != null && clickClip != null)
            uiSource.PlayOneShot(clickClip, volume);

        if (PauseMenu.Instance != null)
        {
            PauseMenu.Instance.OpenMenuFromButton();
        }
        else
        {
            Debug.LogWarning("PauseMenu.Instance not found in scene.");
        }
    }
}