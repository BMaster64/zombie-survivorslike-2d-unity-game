using System;
using System.Collections;
using UnityEngine;

public class Character1Special : MonoBehaviour, AbilityCooldownUI.ISpecialAbility
{
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 10f;
    public bool invulnerableDuringDash = true;
    public bool phasesThroughEnemies = true;
    public GameObject dashEffect;
    public Sprite abilityIcon;

    // Private variables
    private bool isAbilityReady = true;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private Rigidbody2D rb;
    private float originalDefense;
    private bool isInvulnerable = false;
    private float remainingCooldown = 0f;
    // Store original collision layer settings
    private int playerLayer;
    private int enemyLayer;
    private bool originalCollisionSetting;
    // Event for ability activation
    public event Action OnAbilityActivated;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        // Store layer information
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        originalCollisionSetting = Physics2D.GetIgnoreLayerCollision(playerLayer, enemyLayer);

        // Register with UI if it exists
        AbilityCooldownUI cooldownUI = FindObjectOfType<AbilityCooldownUI>();
        if (cooldownUI != null)
        {
            cooldownUI.RegisterAbility(this);
            if (abilityIcon != null)
            {
                cooldownUI.SetAbilityIcon(abilityIcon);
            }
        }
    }

    void Update()
    {
        // Update remaining cooldown
        if (!isAbilityReady && remainingCooldown > 0)
        {
            remainingCooldown -= Time.deltaTime;
            if (remainingCooldown <= 0)
            {
                isAbilityReady = true;
            }
        }

        // Check for space bar input to activate special ability
        if (Input.GetKeyDown(KeyCode.Space) && isAbilityReady)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isAbilityReady = false;
        remainingCooldown = dashCooldown;

        // Notify listeners that ability was activated
        OnAbilityActivated?.Invoke();

        // Store current movement state
        Vector3 moveDirection = rb.linearVelocity.normalized;

        // If player isn't moving, dash in facing direction
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        }

        // Activate invulnerability if enabled
        if (invulnerableDuringDash)
        {
            isInvulnerable = true;
            originalDefense = playerStats.currentDefense;
            playerStats.currentDefense = 1f; // 100% damage reduction
        }
        // Enable phasing through enemies if enabled
        if (phasesThroughEnemies)
        {
            // Ignore collisions between player and enemy layers
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }
        // Instantiate dash effect if available
        if (dashEffect != null)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Instantiate the effect with the correct rotation
            GameObject effect = Instantiate(dashEffect, transform.position, rotation);

            // Apply vertical flip when moving left (horizontally flipped)
            // This is done by scaling the effect's y component to -1 when x is negative
            if (moveDirection.x < 0)
            {
                // Get the current scale of the effect
                Vector3 effectScale = effect.transform.localScale;
                // Flip the y component to create a vertical flip
                effectScale.y *= -1;
                // Apply the new scale
                effect.transform.localScale = effectScale;
            }
        }

        // Calculate dash distance and speed
        float dashSpeed = dashDistance / dashDuration;

        // Freeze normal movement during dash
        float originalMoveSpeed = playerMovement.moveSpeed;
        playerMovement.enabled = false;

        // Apply dash movement
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = moveDirection * dashSpeed;
            yield return null;
        }

        // Reset movement
        rb.linearVelocity = Vector2.zero;
        playerMovement.enabled = true;
        playerMovement.moveSpeed = originalMoveSpeed;

        // Remove invulnerability
        if (invulnerableDuringDash)
        {
            isInvulnerable = false;
            playerStats.currentDefense = originalDefense;
        }

        // Reset enemy collisions
        if (phasesThroughEnemies)
        {
            // Restore original collision setting
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, originalCollisionSetting);
        }
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    #region ISpecialAbility Implementation

    public bool IsAbilityReady()
    {
        return isAbilityReady;
    }

    public float GetCooldownDuration()
    {
        return dashCooldown;
    }

    public float GetRemainingCooldown()
    {
        return remainingCooldown;
    }

    #endregion
}