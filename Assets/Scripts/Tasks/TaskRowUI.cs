using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image status;

    [Header("Status Sprites")]
    [SerializeField] private Sprite doneSprite;     // ✓
    [SerializeField] private Sprite notDoneSprite;  // ✗

    public void Init(string displayName, bool completed)
    {
        if (nameText)
            nameText.text = displayName;

        SetCompleted(completed);
    }

    public void SetCompleted(bool completed)
    {
        if (status)
        {
            status.sprite = completed ? doneSprite : notDoneSprite;
            status.color = completed ? Color.green : Color.red;
        }

        if (nameText)
            nameText.color = Color.black;   
    }
}