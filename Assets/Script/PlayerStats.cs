using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Combat Stats")]
    public float attackMultiplier = 1f;
    public float currentDefense = 0f;
    public float moveSpeedMultiplier = 1f;
    public float healthMultiplier = 1f;
    public float healthRegenAmount = 0f;
    public float attackSpeedMultiplier = 1f;

    [Header("Other Stats")]
    public float pickupRangeMultiplier = 1f;
    public float xpGainMultiplier = 1f;  

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    // You can add more stats here as needed

    private void Start()
    {
        // Initialize any starting stats
        playerMovement = GetComponent<PlayerMovement>();
        UpdateMovementSpeed();
        UpdatePickupRange();

        playerHealth = GetComponent<PlayerHealth>();
        UpdateMaxHealth();
    }

    public void UpdateMovementSpeed()
    {
        if (playerMovement != null)
        {
            playerMovement.moveSpeed *= moveSpeedMultiplier;
        }
    }

    public void UpdatePickupRange()
    {
        if (playerMovement != null)
        {
            playerMovement.pickupRange *= pickupRangeMultiplier;
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