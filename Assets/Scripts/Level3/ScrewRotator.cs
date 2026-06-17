using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrewRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationPerClick = 90f;
    [SerializeField] private float rotateSpeed = 300f;
    [SerializeField] private int requiredClicks = 3;

    [Header("References")]
    [SerializeField] private WireTaskManager wireTaskManager;
    [SerializeField] private SparkController sparkController;
    [SerializeField] private DimmerFeedback dimmer;

    [Header("Sound")]
    [SerializeField] private AudioClip screwSound;
    [SerializeField] private float screwVolume = 0.7f;

    [Header("Manager")]
    [SerializeField] private ScrewTaskManager manager;

    private RectTransform rect;
    private Button button;

    private int currentClicks = 0;
    private bool isRotating = false;
    private bool isDone = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        if (isRotating || isDone)
            return;

        if (wireTaskManager == null || !wireTaskManager.IsCompleted())
        {
            PlayWrongFeedback();
            return;
        }

        PlayScrewSound();
        StartCoroutine(RotateRoutine());
    }

    void PlayWrongFeedback()
    {
        if (sparkController != null)
            sparkController.PlayEffect();

        if (dimmer != null)
            dimmer.ShowWrong();

        float damage = Random.Range(10f, 26f);

        if (GlobalHP.Instance != null)
            GlobalHP.Instance.TakeDamage(damage);

        Debug.Log("Blocked: wires not completed → damage applied");
    }

    void PlayScrewSound()
    {
        if (screwSound != null && UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.Play(screwSound, screwVolume);
    }

    IEnumerator RotateRoutine()
    {
        isRotating = true;

        float rotated = 0f;

        while (rotated < rotationPerClick)
        {
            float step = rotateSpeed * Time.deltaTime;

            rect.Rotate(0f, 0f, -step);
            rotated += step;

            yield return null;
        }

        currentClicks++;

        if (currentClicks >= requiredClicks)
        {
            isDone = true;

            if (button != null)
                button.interactable = false;

            if (manager != null)
                manager.CheckCompletion();
        }

        isRotating = false;
    }

    public bool IsDone()
    {
        return isDone;
    }
}