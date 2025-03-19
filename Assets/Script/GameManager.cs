using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public float gameTimeLimit = 600f; // 10 minutes default
    public bool useTimeLimit = true;
    public bool useBossDefeat = true;
    public string mainMenuScene = "Main Menu";
    public string gameplayScene = "Stage01";

    [Header("End Game Screen")]
    public GameObject endGameScreen;
    public TMPro.TextMeshProUGUI endGameTitleText;
    public TMPro.TextMeshProUGUI endGameScoreText;
    public TMPro.TextMeshProUGUI endGameTimeText;
    public TMPro.TextMeshProUGUI endGameLevelText;

    private bool gameOver = false;
    private bool bossDefeated = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Hide the end game screen at the start
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver) return;

        // Check for time limit
        if (useTimeLimit)
        {
            float currentTime = GameHUDManager.instance.gameTime;
            if (currentTime >= gameTimeLimit)
            {
                ShowEndGameScreen("Survived!");
            }
        }

        // Player death check
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || !player.activeInHierarchy)
        {
            ShowEndGameScreen("Game Over");
        }
    }

    public void BossDefeated()
    {
        if (useBossDefeat && !gameOver)
        {
            bossDefeated = true;
            ShowEndGameScreen("Victory!");
        }
    }
    private void DestroyAllEnemies()
    {
        // Find all enemies with tags
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");

        // Destroy all regular enemies
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        // Destroy all bosses (if they aren't already destroyed)
        foreach (GameObject boss in bosses)
        {
            Destroy(boss);
        }

        // Optional: You can also find enemies by their component types
        // This is useful if you have enemies without the "Enemy" tag
        BossEnemy[] bossEnemies = FindObjectsOfType<BossEnemy>();
        foreach (BossEnemy bossEnemy in bossEnemies)
        {
            Destroy(bossEnemy.gameObject);
        }

        // If you have a base Enemy class or other enemy types, you can destroy them here
        // Enemy[] normalEnemies = FindObjectsOfType<Enemy>();
        // foreach (Enemy enemy in normalEnemies)
        // {
        //     Destroy(enemy.gameObject);
        // }
    }
    public void ShowEndGameScreen(string title)
    {
        if (gameOver) return;

        gameOver = true;
        DestroyAllEnemies();

        Time.timeScale = 0f; // Pause the game
        Cursor.visible = true;

        // Get data from GameHUDManager
        GameHUDManager hudManager = GameHUDManager.instance;

        // Set up end game screen
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);

            if (endGameTitleText != null)
                endGameTitleText.text = title;

            if (endGameScoreText != null)
                endGameScoreText.text = "Score: " + PlayerPrefs.GetInt("CurrentScore", 0).ToString();

            if (endGameTimeText != null)
            {
                int minutes = Mathf.FloorToInt(hudManager.gameTime / 60f);
                int seconds = Mathf.FloorToInt(hudManager.gameTime % 60f);
                endGameTimeText.text = "Time survived: " + string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (endGameLevelText != null)
            {
                // Use your existing XPLevelController
                if (XPLevelController.instance != null)
                {
                    endGameLevelText.text = "Level reached: " + XPLevelController.instance.currentLevel.ToString();
                }
            }
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene(mainMenuScene);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale
        SceneManager.LoadScene(gameplayScene);
    }
}