using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Combat Stats")]
    public float attackMultiplier = 1f;
    public float currentDefense = 0f;
    public float moveSpeedMultiplier = 1f;
    public float healthMultiplier = 1f;

    [Header("Other Stats")]
    public float pickupRangeMultiplier = 1f;

    private PlayerHealth playerHealth;
    // You can add more stats here as needed

    private void Start()
    {
        // Initialize any starting stats
        UpdateMovementSpeed();
        UpdatePickupRange();

        playerHealth = GetComponent<PlayerHealth>();
        UpdateMaxHealth();
    }

    public void UpdateMovementSpeed()
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.moveSpeed *= moveSpeedMultiplier;
        }
    }

    public void UpdatePickupRange()
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.pickupRange *= pickupRangeMultiplier;
        }
    }

    public void UpdateMaxHealth()
    {
        if (playerHealth != null)
        {
            float newMaxHealth = playerHealth.maxHealth * healthMultiplier;
            // Keep the same health percentage when increasing max health
            float healthPercentage = playerHealth.currentHealth / playerHealth.maxHealth;
            playerHealth.maxHealth = newMaxHealth;
            playerHealth.currentHealth = newMaxHealth * healthPercentage;
            // Update the health slider
            playerHealth.healthSlider.maxValue = newMaxHealth;
            playerHealth.healthSlider.value = playerHealth.currentHealth;
        }
    }

}