using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Health UI")]
    public Slider healthSlider;
    public Image healthFillImage;
    public TextMeshProUGUI healthText;

    [Header("Health Colors")]
    public Color fullColor = Color.green;
    public Color lowColor = Color.red;

    [Header("Inventory UI")]
    public Transform inventoryIconsParent;
    public GameObject inventoryIconPrefab;

    [Header("Quest / Tips")]
    public TextMeshProUGUI questText;

    [Header("Interaction Prompt")]
    public TextMeshProUGUI interactionText;

    [Header("Pickup Popup (Text Only)")]
    public TextMeshProUGUI pickupPopupText;         
    [SerializeField] private float pickupShowTime = 1.2f;
    [SerializeField] private float pickupFadeIn = 0.15f;
    [SerializeField] private float pickupFadeOut = 0.25f;

    private CanvasGroup pickupCg;
    private Coroutine pickupRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

       
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        if (pickupPopupText != null)
        {
            pickupCg = pickupPopupText.GetComponent<CanvasGroup>();
            if (pickupCg == null) pickupCg = pickupPopupText.gameObject.AddComponent<CanvasGroup>();

            pickupPopupText.gameObject.SetActive(false);
            pickupCg.alpha = 0f;
        }
    }

   
    public void SetHealth(float current, float max)
    {
        if (healthSlider != null)
        {
            float clampedCurrent = Mathf.Clamp(current, 0f, max);
            healthSlider.maxValue = max;
            healthSlider.value = clampedCurrent;
        }

        if (healthFillImage != null && healthSlider != null)
        {
            float pct = (healthSlider.maxValue > 0f) ? healthSlider.value / healthSlider.maxValue : 0f;
            healthFillImage.color = Color.Lerp(lowColor, fullColor, pct);
        }

        if (healthText != null)
        {
            int intHP = Mathf.RoundToInt(Mathf.Clamp(current, 0f, max));
            healthText.text = $"HP : {intHP}";
        }
    }

   
    public void RefreshInventoryUI(List<Item> items)
    {
        if (inventoryIconsParent == null || inventoryIconPrefab == null) return;

        foreach (Transform t in inventoryIconsParent)
            Destroy(t.gameObject);

        foreach (var item in items)
        {
            GameObject slot = Instantiate(inventoryIconPrefab, inventoryIconsParent);
            Image img = slot.GetComponent<Image>();
            if (img != null) img.sprite = item.icon;
        }
    }

 
    public void ShowQuest(string text)
    {
        if (questText != null)
            questText.text = text;
    }

    
    public void ShowInteractionText(string message)
    {
        if (interactionText == null) return;

        interactionText.text = message;
        interactionText.gameObject.SetActive(true);
    }

    public void HideInteractionText()
    {
        if (interactionText == null) return;

        interactionText.gameObject.SetActive(false);
    }

   
    public void ShowPickupPopup(string itemName)
    {
        if (pickupPopupText == null) return;

        if (pickupRoutine != null) StopCoroutine(pickupRoutine);
        pickupRoutine = StartCoroutine(PickupPopupRoutine(itemName));
    }

    private IEnumerator PickupPopupRoutine(string itemName)
    {
        pickupPopupText.gameObject.SetActive(true);
        pickupPopupText.text = $"Item picked up: <b>{itemName}</b>";

        // Fade in
        float t = 0f;
        while (t < pickupFadeIn)
        {
            t += Time.deltaTime;
            pickupCg.alpha = Mathf.Lerp(0f, 1f, t / pickupFadeIn);
            yield return null;
        }
        pickupCg.alpha = 1f;

        yield return new WaitForSeconds(pickupShowTime);

        // Fade out
        t = 0f;
        while (t < pickupFadeOut)
        {
            t += Time.deltaTime;
            pickupCg.alpha = Mathf.Lerp(1f, 0f, t / pickupFadeOut);
            yield return null;
        }
        pickupCg.alpha = 0f;

        pickupPopupText.gameObject.SetActive(false);
        pickupRoutine = null;
    }
}