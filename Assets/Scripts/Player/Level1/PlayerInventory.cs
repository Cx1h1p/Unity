using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<string> collected = new HashSet<string>();

    public event Action<string> OnCollected;

    [Header("Pickup Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private float pickupVolume = 0.8f;

    public bool Has(string itemId) => collected.Contains(itemId);

    public bool TryAdd(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.id))
            return false;

        if (collected.Add(item.id))
        {
            //  PLAY SOUND
            if (audioSource != null && pickupClip != null)
                audioSource.PlayOneShot(pickupClip, pickupVolume);

            OnCollected?.Invoke(item.id);
            return true;
        }

        return false;
    }
}