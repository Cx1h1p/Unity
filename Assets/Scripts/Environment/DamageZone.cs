using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage")]
    public float damagePerSecond = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // SHOW RED DIMMER
        DamageHealDimmer.Instance?.ShowDamageHold();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // HIDE DIMMER
        DamageHealDimmer.Instance?.Hide();
    }
}