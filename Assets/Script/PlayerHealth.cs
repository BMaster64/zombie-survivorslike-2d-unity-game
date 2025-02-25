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

    private void Awake()
    {
        instance = this;
        stats = GetComponent<PlayerStats>();

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
        
    }

    public void TakeDamage(float damage)
    {
        float damageReduction = stats != null ? stats.currentDefense : 0f;
        float actualDamage = damage * (1f - damageReduction);

        currentHealth -= actualDamage;

        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

        healthSlider.value = currentHealth;
    }
}