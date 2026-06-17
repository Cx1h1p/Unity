using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSource;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.03f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplySettings();

        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (musicSource == null || newClip == null)
            return;

        if (musicSource.clip == newClip)
        {
            ApplySettings();
            return;
        }

        musicSource.clip = newClip;
        ApplySettings();
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void StopAndDestroyMusic()
    {
        if (musicSource != null)
            musicSource.Stop();

        Instance = null;
        Destroy(gameObject);
    }

    private void ApplySettings()
    {
        if (musicSource == null) return;

        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.playOnAwake = true;
    }
}