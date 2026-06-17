using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip levelMusic;

    void Start()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.ChangeMusic(levelMusic);
    }
}