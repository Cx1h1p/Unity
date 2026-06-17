using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsAdditiveFix : MonoBehaviour
{
    [Header("Only needed for normal Settings opening")]
    [SerializeField] private GameObject settingsCameraObject;

    void Awake()
    {
        bool openedAdditively = SceneManager.sceneCount > 1;

        if (openedAdditively)
        {
            if (settingsCameraObject != null)
                settingsCameraObject.SetActive(false);
        }
        else
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }
    }
}