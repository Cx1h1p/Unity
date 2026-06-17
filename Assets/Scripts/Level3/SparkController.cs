using UnityEngine;
using System.Collections;

public class SparkController : MonoBehaviour
{
    [Header("Spark Prefab")]
    [SerializeField] private GameObject sparkPrefab;

    [Header("Position Settings")]
    [SerializeField] private float horizontalOffset = 150f;
    [SerializeField] private float distanceFromCamera = 2f;

    [Header("Explosion Sound")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip explosionClip;
    [Range(0f, 1f)][SerializeField] private float explosionVolume = 1f;

    private bool isPlaying = false;

    public void PlayEffect()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        if (isPlaying) return;

        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        isPlaying = true;

        if (sfxSource != null && explosionClip != null)
            sfxSource.PlayOneShot(explosionClip, explosionVolume);

        Camera cam = Camera.main;

        Vector3 screenCenter = new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            distanceFromCamera
        );

        Vector3[] screenPositions = new Vector3[]
        {
            new Vector3(screenCenter.x - horizontalOffset, screenCenter.y, distanceFromCamera),
            screenCenter,
            new Vector3(screenCenter.x + horizontalOffset, screenCenter.y, distanceFromCamera)
        };

        foreach (Vector3 sp in screenPositions)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(sp);

            GameObject spark = Instantiate(sparkPrefab, worldPos, Quaternion.identity);

            ParticleSystem ps = spark.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();

                StartCoroutine(ForceStopAndDestroy(spark, ps, 2f));
            }
            else
            {
                Destroy(spark, 2f);
            }
        }

        yield return new WaitForSecondsRealtime(2f);

        isPlaying = false;
        gameObject.SetActive(false);
    }

    IEnumerator ForceStopAndDestroy(GameObject obj, ParticleSystem ps, float time)
    {
        yield return new WaitForSecondsRealtime(time);

        if (ps != null)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        Destroy(obj);
    }
}