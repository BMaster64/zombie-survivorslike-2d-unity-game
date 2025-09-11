using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeItemUI : MonoBehaviour
{
    public Image upgradeIcon;
    public TextMeshProUGUI upgradeName;
    public TextMeshProUGUI upgradeLevel;

    public void SetUpgradeData(UpgradeData upgrade)
    {
        // Set icon
        if (upgradeIcon != null && upgrade.icon != null)
            upgradeIcon.sprite = upgrade.icon;

        // Set name
        if (upgradeName != null)
            upgradeName.text = upgrade.upgradeName;

        // Set level
        if (upgradeLevel != null)
            upgradeLevel.text = $"Lv. {upgrade.level}/{upgrade.maxLevel}";
    }
}