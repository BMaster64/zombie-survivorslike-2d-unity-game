using System;
using System.Collections;
using UnityEngine;

public class Character2Special : MonoBehaviour, AbilityCooldownUI.ISpecialAbility
{
    public float attackSpeedBoost = 3f;
    public float attackSpeedDuration = 5f;
    public float attackSpeedCooldown = 10f;
    public GameObject attackSpeedEffect;
    public Sprite abilityIcon;

    // Private variables
    private bool isAbilityReady = true;
    private PlayerStats playerStats;
    private float originalAttackSpeed;
    private float remainingCooldown = 0f;

    // Event for ability activation
    public event Action OnAbilityActivated;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        originalAttackSpeed = playerStats.attackSpeedMultiplier;

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
            StartCoroutine(BoostAttackSpeed());
        }
    }

    IEnumerator BoostAttackSpeed()
    {
        isAbilityReady = false;
        remainingCooldown = attackSpeedCooldown;

        // Play skill sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.char2SkillSound);
        }

        // Notify listeners that ability was activated
        OnAbilityActivated?.Invoke();
        
        // Store original attack speed
        originalAttackSpeed = playerStats.attackSpeedMultiplier;

        // Boost attack speed
        playerStats.attackSpeedMultiplier *= attackSpeedBoost;

        // Instantiate effect if available
        if (attackSpeedEffect != null)
        {
            GameObject effect = Instantiate(attackSpeedEffect, transform.position, Quaternion.identity);
            effect.transform.SetParent(transform);
            effect.transform.localPosition = new Vector3((float)0.21, (float)0.6, 0);
            Destroy(effect, attackSpeedDuration);
        }

        // Wait for duration
        yield return new WaitForSeconds(attackSpeedDuration);

        // Reset attack speed
        playerStats.attackSpeedMultiplier = originalAttackSpeed;

        // We're just waiting for the cooldown to finish naturally now
        // The Update method will track the remaining cooldown and set isAbilityReady = true when done
    }

    #region ISpecialAbility Implementation

    public bool IsAbilityReady()
    {
        return isAbilityReady;
    }

    public float GetCooldownDuration()
    {
        return attackSpeedCooldown;
    }

    public float GetRemainingCooldown()
    {
        return remainingCooldown;
    }

    #endregion
}