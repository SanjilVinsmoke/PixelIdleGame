using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Ensure there's a collider for detection
public class PowerUpItem : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType = PowerUpType.None;
    [SerializeField] private bool destroyOnPickup = true;
    [SerializeField] private GameObject pickupEffectPrefab; // Optional visual effect
    [SerializeField] private AudioClip pickupSound; // Optional sound effect

    private void Awake()
    {
        // Ensure the collider is a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError("PowerUpItem requires a Collider2D component.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Collect(player);
        }
    }

    private void Collect(Player player)
    {
        Debug.Log($"Player collected PowerUp: {powerUpType}");

        // Use the factory to create the power-up instance
        IPowerUp powerUp = PowerUpFactory.CreatePowerUp(powerUpType);

        if (powerUp != null)
        {
            // Apply the power-up effect
            powerUp.Apply(player);

            // Optional: Play effects
            PlayPickupEffects();

            // Optional: Destroy the item after pickup
            if (destroyOnPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                // Optionally disable the item instead of destroying
                // gameObject.SetActive(false);
            }
        }
        else
        {
             Debug.LogWarning($"Could not create power-up for type: {powerUpType}", this);
        }
    }

    private void PlayPickupEffects()
    {
        // Instantiate visual effect if assigned
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play sound effect if assigned (consider using an AudioManager)
        if (pickupSound != null && AudioManager.Instance != null) // Assuming an AudioManager singleton
        {
            // AudioManager.Instance.PlaySoundEffect(pickupSound);
        }
        else if (pickupSound != null)
        {
             // Fallback if no AudioManager
             AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }
}