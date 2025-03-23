using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AbilityCooldownUI : MonoBehaviour
{
    [Header("UI References")]
    public Image cooldownFill;
    public TextMeshProUGUI cooldownText;
    public KeyCode abilityKey = KeyCode.Space;

    [Header("Optional Icons")]
    public Image abilityIcon;
    public Sprite defaultIcon;

    // Interface for any special ability to implement
    public interface ISpecialAbility
    {
        bool IsAbilityReady();
        float GetCooldownDuration();
        float GetRemainingCooldown();
        event Action OnAbilityActivated;
    }

    private ISpecialAbility currentAbility;
    private float cooldownDuration;
    private float cooldownTimer;
    private bool isOnCooldown = false;

    void Start()
    {
        // Initialize UI
        cooldownFill.fillAmount = 0;
        cooldownText.text = abilityKey.ToString();

        if (abilityIcon != null && defaultIcon != null)
        {
            abilityIcon.sprite = defaultIcon;
        }
    }

    void OnEnable()
    {
        FindAndRegisterAbility();
    }

    void OnDisable()
    {
        UnregisterCurrentAbility();
    }

    public void FindAndRegisterAbility()
    {
        // Unsubscribe from previous ability if any
        UnregisterCurrentAbility();

        // Find all MonoBehaviours in the scene
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();

        // Find the first one that implements our interface
        foreach (MonoBehaviour behaviour in allMonoBehaviours)
        {
            if (behaviour is ISpecialAbility ability)
            {
                RegisterAbility(ability);
                break;
            }
        }
    }

    public void RegisterAbility(ISpecialAbility ability)
    {
        // Unsubscribe from previous ability if any
        UnregisterCurrentAbility();

        // Set new ability
        currentAbility = ability;

        // Subscribe to the ability activation event
        if (currentAbility != null)
        {
            currentAbility.OnAbilityActivated += HandleAbilityActivated;
            cooldownDuration = currentAbility.GetCooldownDuration();

            // Reset UI state
            if (currentAbility.IsAbilityReady())
            {
                EndCooldown();
            }
            else
            {
                StartCooldown(currentAbility.GetRemainingCooldown());
            }
        }
    }

    private void UnregisterCurrentAbility()
    {
        if (currentAbility != null)
        {
            currentAbility.OnAbilityActivated -= HandleAbilityActivated;
            currentAbility = null;
        }
    }

    void Update()
    {
        if (currentAbility == null)
        {
            return;
        }

        // Update cooldown display
        if (isOnCooldown)
        {
            // If using the ability's own cooldown tracker
            if (currentAbility != null)
            {
                cooldownTimer = currentAbility.GetRemainingCooldown();
                cooldownFill.fillAmount = cooldownTimer / cooldownDuration;
            }
            // Fallback to our own cooldown tracking
            else
            {
                cooldownTimer -= Time.deltaTime;
                cooldownFill.fillAmount = cooldownTimer / cooldownDuration;
            }

            // Show remaining seconds
            if (cooldownTimer > 0)
            {
                cooldownText.text = Mathf.Ceil(cooldownTimer).ToString();
            }

            // Check if cooldown is complete
            if (cooldownTimer <= 0 || (currentAbility != null && currentAbility.IsAbilityReady()))
            {
                EndCooldown();
            }
        }
    }

    private void HandleAbilityActivated()
    {
        StartCooldown(cooldownDuration);
    }

    void StartCooldown(float duration = -1)
    {
        isOnCooldown = true;
        cooldownTimer = duration > 0 ? duration : cooldownDuration;
        cooldownFill.fillAmount = 1;
    }

    void EndCooldown()
    {
        isOnCooldown = false;
        cooldownFill.fillAmount = 0;
        cooldownText.text = abilityKey.ToString();
    }

    // Optionally set a specific ability icon
    public void SetAbilityIcon(Sprite icon)
    {
        if (abilityIcon != null && icon != null)
        {
            abilityIcon.sprite = icon;
        }
    }
}