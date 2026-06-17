using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupPopupUI : MonoBehaviour
{
    public static PickupPopupUI Instance { get; private set; }

    [SerializeField] private RectTransform root; // panel/group
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float showTime = 1.2f;

    private CanvasGroup cg;
    private Coroutine routine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cg = root.GetComponent<CanvasGroup>() ?? root.gameObject.AddComponent<CanvasGroup>();
        root.gameObject.SetActive(false);
    }

    public void Show(Item item)
    {
        if (item == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(item));
    }

    IEnumerator ShowRoutine(Item item)
    {
        root.gameObject.SetActive(true);
        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;
        text.text = $"Picked up: {item.displayName}";

        cg.alpha = 0f;
        float t = 0f;
        while (t < 0.15f) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(0f, 1f, t / 0.15f); yield return null; }

        yield return new WaitForSeconds(showTime);

        t = 0f;
        while (t < 0.25f) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(1f, 0f, t / 0.25f); yield return null; }

        root.gameObject.SetActive(false);
    }
}
