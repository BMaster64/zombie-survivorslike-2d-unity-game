using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpMenu : MonoBehaviour
{
    public static LevelUpMenu instance;

    [Header("Menu Settings")]
    public GameObject menuPanel;
    public UpgradeOption[] upgradeOptionSlots;
    public int upgradeOptionsToShow = 3;

    private void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }

    public void OpenLevelUpMenu()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        CursorManager.SetCursorForGameState(GameState.LevelUp);

        List<UpgradeData> availableUpgrades = UpgradeManager.instance.GetAvailableUpgrades();
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

    void ShuffleUpgrades(List<UpgradeData> upgrades)
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
        
        // Force a cursor state refresh to prevent glitches
        StartCoroutine(RefreshCursorAfterUpgrade());
    }
    
    private System.Collections.IEnumerator RefreshCursorAfterUpgrade()
    {
        // Wait a frame to ensure UI is fully closed
        yield return null;
        
        // Set cursor for gameplay
        CursorManager.SetCursorForGameState(GameState.Playing);
        
        // Additional safety check after a small delay
        yield return new WaitForSecondsRealtime(0.1f);
        CursorManager.SetCursorForGameState(GameState.Playing);
    }
}