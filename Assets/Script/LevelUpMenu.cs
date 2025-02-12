using UnityEngine;
using System.Collections.Generic;

public class LevelUpMenu : MonoBehaviour
{
    public static LevelUpMenu instance;

    [Header("Menu Settings")]
    public GameObject menuPanel;
    public UpgradeOption[] upgradeOptionSlots;
    public int upgradeOptionsToShow = 3;

    [Header("Available Upgrades")]
    public List<Upgrade> allUpgrades = new List<Upgrade>();

    private void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }

    public void OpenLevelUpMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;

        List<Upgrade> availableUpgrades = GetAvailableUpgrades();
        ShuffleUpgrades(availableUpgrades);

        // Show upgrade options
        for (int i = 0; i < upgradeOptionsToShow; i++)
        {
            if (i < availableUpgrades.Count)
            {
                upgradeOptionSlots[i].gameObject.SetActive(true);
                upgradeOptionSlots[i].SetupUpgrade(availableUpgrades[i], this);
            }
            else
            {
                upgradeOptionSlots[i].gameObject.SetActive(false);
            }
        }
    }

    List<Upgrade> GetAvailableUpgrades()
    {
        return allUpgrades.FindAll(upgrade => upgrade.level < upgrade.maxLevel);
    }

    void ShuffleUpgrades(List<Upgrade> upgrades)
    {
        for (int i = upgrades.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = upgrades[i];
            upgrades[i] = upgrades[j];
            upgrades[j] = temp;
        }
    }

    public void UpgradeSelected()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
