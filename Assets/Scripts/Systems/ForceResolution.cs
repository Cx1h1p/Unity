using UnityEngine;

public class ForceResolution : MonoBehaviour
{
    public static ForceResolution Instance;

    [Header("Forced Resolution")]
    [SerializeField] private int width = 1920;
    [SerializeField] private int height = 1080;
    [SerializeField] private bool fullscreen = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Screen.SetResolution(width, height, fullscreen);
    }

    private void Start()
    {
        Screen.SetResolution(width, height, fullscreen);
    }
}