using System.Collections;
using UnityEngine;

public class PlayerSpawnEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;

    private SpriteRenderer sr;
    private Vector3 originalScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        float t = 0f;

        Vector3 startScale = originalScale * 0.9f;
        transform.localScale = startScale;

        Color c = sr.color;
        c.a = 0f;
        sr.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            transform.localScale = Vector3.Lerp(startScale, originalScale, p);

            c.a = Mathf.Lerp(0f, 1f, p);
            sr.color = c;

            yield return null;
        }

        transform.localScale = originalScale;
        c.a = 1f;
        sr.color = c;
    }
}