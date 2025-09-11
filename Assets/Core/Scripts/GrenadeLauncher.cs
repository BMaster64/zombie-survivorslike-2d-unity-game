using UnityEngine;
using System.Collections.Generic;

public class GrenadeLauncher : MonoBehaviour
{
    public GameObject grenadePrefab;
    public float baseCooldown = 3f;
    public float grenadeRange = 7f;
    public int maxTargets = 1; // How many grenades to throw at once

    private float cooldownTimer;
    private PlayerStats playerStats;
    private Transform playerTransform;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerTransform = transform.parent;
        cooldownTimer = 0f;
    }

    void Update()
    {
        // Update cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // When cooldown is finished, try to throw grenades
        if (cooldownTimer <= 0)
        {
            ThrowGrenades();
        }
    }

    void ThrowGrenades()
    {
        // Find enemies within range
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, grenadeRange, LayerMask.GetMask("Enemy", "Boss"));

        // Sort by distance
        List<Collider2D> sortedEnemies = new List<Collider2D>(enemies);
        sortedEnemies.Sort((a, b) =>
            Vector2.Distance(transform.position, a.transform.position)
            .CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        // Target up to maxTargets enemies
        int targetCount = Mathf.Min(sortedEnemies.Count, maxTargets);

        // If no enemies in range, don't throw
        if (targetCount == 0)
        {
            return;
        }

        // Throw grenades at the closest enemies
        for (int i = 0; i < targetCount; i++)
        {
            Transform enemyTransform = sortedEnemies[i].transform;

            // Play grenade sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGrenadeSound();
            }

            // Create grenade at player position
            GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);

            // Set target position (slightly randomized to spread explosions)
            Vector3 targetPos = enemyTransform.position;
            targetPos += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            // Initialize grenade with target
            Grenade grenadeScript = grenade.GetComponent<Grenade>();
            if (grenadeScript != null)
            {
                grenadeScript.SetTarget(targetPos);
            }
        }

        // Apply cooldown (affected by attack speed)
        float adjustedCooldown = baseCooldown;
        if (playerStats != null)
        {
            adjustedCooldown /= playerStats.attackSpeedMultiplier;
        }

        cooldownTimer = adjustedCooldown;
    }

    // Draw targeting range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grenadeRange);
    }

    // Upgrade methods
    public void UpgradeRange(float rangeMultiplier)
    {
        grenadeRange *= rangeMultiplier;
    }

    public void UpgradeMaxTargets(int additionalTargets)
    {
        maxTargets += additionalTargets;
    }

    public void UpgradeDamage(float damageMultiplier)
    {
        // This will be applied to new grenades when they're created
        if (grenadePrefab != null)
        {
            Grenade grenadeScript = grenadePrefab.GetComponent<Grenade>();
            if (grenadeScript != null)
            {
                grenadeScript.baseDamage *= damageMultiplier;
            }
        }
    }
}