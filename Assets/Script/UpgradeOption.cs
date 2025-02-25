using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeOption : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;
    public Button upgradeButton;

    private UpgradeData currentUpgrade;
    private LevelUpMenu menuManager;

    public void SetupUpgrade(UpgradeData upgrade, LevelUpMenu manager)
    {
        currentUpgrade = upgrade;
        menuManager = manager;

        // Update UI elements
        iconImage.sprite = upgrade.icon;

        // Show different title format based on whether it's a new unlock or an upgrade
        if (!upgrade.isUnlocked)
        {
            titleText.text = upgrade.upgradeName;
        }
        else
        {
            titleText.text = $"{upgrade.upgradeName}";
        }

        // Show current/max level
        levelText.text = $"Lvl {upgrade.level + 1}";

        // Create detailed description based on upgrade type
        string detailedDescription = upgrade.description;
        if (upgrade.type == UpgradeType.Stat)
        {
            // Add current and next level values for stat upgrades
            float currentValue = upgrade.GetCurrentValue();
            detailedDescription += $"\nCurrent: +{(currentValue * 100):F1}%";
            if (upgrade.level + 1 < upgrade.valuePerLevel.Length)
            {
                float nextValue = upgrade.valuePerLevel[upgrade.level + 1];
                detailedDescription += $"\nNext: +{(nextValue * 100):F1}%";
            }
        }
        descriptionText.text = detailedDescription;

        // Clear previous listeners and add new one
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeSelected);
    }

    void OnUpgradeSelected()
    {
        // Apply the upgrade through the UpgradeManager
        UpgradeManager.instance.ApplyUpgrade(currentUpgrade);

        // Close the menu
        menuManager.UpgradeSelected();

        // Hide cursor if it was visible
        Cursor.visible = false;
    }

    // Optional: Add hover effects
    public void OnPointerEnter()
    {
        // Add hover effects like scaling up the button or changing colors
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit()
    {
        // Reset hover effects
        transform.localScale = Vector3.one;
    }
}