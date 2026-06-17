using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageHealDimmer : MonoBehaviour
{
    public static DamageHealDimmer Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image image;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;

    [Header("Colors")]
    [SerializeField]
    private Color damageColor =
        new Color(1f, 0.25f, 0.25f, 0.22f);

    [SerializeField]
    private Color healColor =
        new Color(0.25f, 0.55f, 1f, 0.22f);

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 6f;

    private Coroutine routine;

    void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (image == null)
            image = GetComponent<Image>();

        canvasGroup.alpha = 0f;
    }

    void Update()
    {
       
        if (deathScreen != null && deathScreen.activeInHierarchy)
        {
            ForceHide();
        }
    }

    public void ShowDamageHold()
    {
        if (deathScreen != null && deathScreen.activeInHierarchy)
            return;

        SetHold(damageColor);
    }

    public void ShowHealHold()
    {
        if (deathScreen != null && deathScreen.activeInHierarchy)
            return;

        SetHold(healColor);
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeOut());
    }

    void SetHold(Color color)
    {
        if (routine != null)
            StopCoroutine(routine);

        image.color = color;
        canvasGroup.alpha = color.a;
    }

    void ForceHide()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        canvasGroup.alpha = 0f;
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;

            yield return null;
        }

        canvasGroup.alpha = 0f;
        routine = null;
    }
}