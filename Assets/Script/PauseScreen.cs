using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Upgrade Display")]
    public Transform upgradeListContainer;
    public GameObject upgradeItemPrefab;

    [Header("UI Elements")]
    public TextMeshProUGUI pauseTitle;
    public CanvasGroup pauseCanvasGroup;

    private bool isPaused = false;
    private List<GameObject> spawnedUpgradeItems = new List<GameObject>();

    void Start()
    {
        // Set initial state
        SetPauseScreenActive(false);

        // Add button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        // Toggle pause on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        // Pause time
        Time.timeScale = 0f;

        // Show cursor when paused
        CursorManager.SetCursorForGameState(GameState.Paused);

        // Display pause screen
        SetPauseScreenActive(true);

        // Display player upgrades
        DisplayPlayerUpgrades();
    }

    void ResumeGame()
    {
        // Resume time
        Time.timeScale = 1f;

        // Hide cursor when resuming gameplay
        CursorManager.SetCursorForGameState(GameState.Playing);

        // Hide pause screen
        SetPauseScreenActive(false);
        isPaused = false;
        
        // Force refresh cursor state after a small delay to prevent glitches
        StartCoroutine(RefreshCursorAfterResume());
    }
    
    private System.Collections.IEnumerator RefreshCursorAfterResume()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        CursorManager.SetCursorForGameState(GameState.Playing);
    }

    void RestartGame()
    {
        // Resume time before restarting
        Time.timeScale = 1f;

        // Hide cursor since we're restarting gameplay
        CursorManager.SetCursorForGameState(GameState.Playing);

        // Call the restart method from your GameManager
        GameManager.instance.RestartGame();
    }

    void ReturnToMainMenu()
    {
        // Resume time before returning to menu
        Time.timeScale = 1f;

        // Show cursor for main menu navigation
        CursorManager.SetCursorForGameState(GameState.MainMenu);

        // Call the main menu method from your GameManager
        GameManager.instance.ReturnToMainMenu();
    }

    void SetPauseScreenActive(bool active)
    {
        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = active ? 1f : 0f;
            pauseCanvasGroup.interactable = active;
            pauseCanvasGroup.blocksRaycasts = active;
        }
        else
        {
            // Fallback if no CanvasGroup is assigned
            gameObject.SetActive(active);
        }
    }

    void DisplayPlayerUpgrades()
    {
        // Clear previous upgrade items
        ClearUpgradeItems();

        if (upgradeListContainer == null || upgradeItemPrefab == null || UpgradeManager.instance == null)
            return;

        // Display weapon upgrades
        foreach (var weapon in UpgradeManager.instance.unlockedWeapons)
        {
            CreateUpgradeItem(weapon);
        }

        // Display stat upgrades that have at least 1 level
        foreach (var stat in UpgradeManager.instance.statUpgrades)
        {
            if (stat.level > 0)
            {
                CreateUpgradeItem(stat);
            }
        }
    }

    void CreateUpgradeItem(UpgradeData upgrade)
    {
        GameObject item = Instantiate(upgradeItemPrefab, upgradeListContainer);
        spawnedUpgradeItems.Add(item);

        // Get references to UI elements in the prefab
        TextMeshProUGUI upgradeName = item.transform.Find("UpgradeName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI upgradeLevel = item.transform.Find("UpgradeLevel")?.GetComponent<TextMeshProUGUI>();
        Image upgradeIcon = item.transform.Find("UpgradeIcon")?.GetComponent<Image>();

        // Set values
        if (upgradeName != null)
            upgradeName.text = upgrade.upgradeName;

        if (upgradeLevel != null)
            upgradeLevel.text = $"Lv. {upgrade.level}/{upgrade.maxLevel}";

        if (upgradeIcon != null && upgrade.icon != null)
            upgradeIcon.sprite = upgrade.icon;
    }

    void ClearUpgradeItems()
    {
        foreach (GameObject item in spawnedUpgradeItems)
        {
            Destroy(item);
        }

        spawnedUpgradeItems.Clear();
    }
}