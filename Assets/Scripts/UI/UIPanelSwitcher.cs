using UnityEngine;

public class UIPanelSwitcher : MonoBehaviour
{
    [Header("Panels to hide")]
    [SerializeField] private GameObject[] panelsToHide;

    [Header("Panels to show")]
    [SerializeField] private GameObject[] panelsToShow;

    [Header("UI Click Sound")]
    [SerializeField] private AudioClip clickClip;
    [Range(0f, 1f)][SerializeField] private float clickVolume = 0.8f;

    public void SwitchPanels()
    {
        
        if (clickClip != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(clickClip, clickVolume);

        
        if (panelsToHide != null)
        {
            foreach (GameObject panel in panelsToHide)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        
        if (panelsToShow != null)
        {
            foreach (GameObject panel in panelsToShow)
            {
                if (panel != null)
                    panel.SetActive(true);
            }
        }
    }
}