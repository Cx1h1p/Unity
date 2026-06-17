using UnityEngine;
using UnityEngine.UI;

public class IntroPopupClose : MonoBehaviour
{
    [Header("What to close")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GameObject optionalDim;

    [Header("Button")]
    [SerializeField] private Button closeButton;

    [Header("UI Click Sound")]
    [SerializeField] private AudioClip clickClip;
    [Range(0f, 1f)][SerializeField] private float clickVolume = 0.8f;

    [Header("Optional: also close with key")]
    [SerializeField] private bool allowEscape = true;

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    void Update()
    {
        if (allowEscape && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Close()
    {
        // Play click sound through global UI_SFX object
        if (clickClip != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(clickClip, clickVolume);

        if (optionalDim != null)
            optionalDim.SetActive(false);

        if (panelRoot != null)
            panelRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }
}