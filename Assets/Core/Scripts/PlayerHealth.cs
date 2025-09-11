using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public float currentHealth, maxHealth;

    public Slider healthSlider;

    private PlayerStats stats;
    private Character1Special character1Special;
    private void Awake()
    {
        instance = this;
        stats = GetComponent<PlayerStats>();
        character1Special = GetComponent<Character1Special>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        RegenerateHealth();
    }

    public void TakeDamage(float damage)
    {
        if (character1Special != null && character1Special.IsInvulnerable())
        {
            return; // Skip damage if invulnerable
        }
        float damageReduction = stats != null ? stats.currentDefense : 0f;
        float actualDamage = damage * (1f - damageReduction);

        currentHealth -= actualDamage;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamageSound();
        }

        if (currentHealth <= 0)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.defeatSound);
            }

            // Notify GameManager of player death
            if (GameManager.instance != null)
            {
                GameManager.instance.ShowEndGameScreen("Game Over");
            }

            gameObject.SetActive(false);
        }

        healthSlider.value = currentHealth;
    }

    private void RegenerateHealth()
    {
        if (stats != null && stats.healthRegenAmount > 0)
        {
            // Only regenerate if not at max health
            if (currentHealth < maxHealth)
            {
                currentHealth += stats.healthRegenAmount * Time.deltaTime;
                // Cap health at maximum
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                // Update slider
                healthSlider.value = currentHealth;
            }
        }
    }
}