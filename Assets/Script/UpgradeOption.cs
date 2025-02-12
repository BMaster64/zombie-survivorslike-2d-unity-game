// UpgradeOption.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Upgrade
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public int level = 0;
    public int maxLevel = 5;
}

public class UpgradeOption : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button upgradeButton;

    private Upgrade currentUpgrade;
    private LevelUpMenu menuManager;

    public void SetupUpgrade(Upgrade upgrade, LevelUpMenu manager)
    {
        currentUpgrade = upgrade;
        menuManager = manager;

        iconImage.sprite = upgrade.icon;
        titleText.text = $"{upgrade.upgradeName} Lvl {upgrade.level + 1}";
        descriptionText.text = upgrade.description;

        upgradeButton.onClick.AddListener(OnUpgradeSelected);
    }

    void OnUpgradeSelected()
    {
        currentUpgrade.level++;
        menuManager.UpgradeSelected();
        Cursor.visible = false;
    }
}
