using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource uiSFX;
    [SerializeField] private AudioClip clickClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    void Awake()
    {
        Button btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (clickClip != null && uiSFX != null)
        {
            uiSFX.PlayOneShot(clickClip, volume);
        }
    }
}