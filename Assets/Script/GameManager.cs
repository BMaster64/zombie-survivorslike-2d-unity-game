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

    [Header("Stage Goal")]
    public string stageGoalText = "Survive!";
    public float goalMessageDuration = 3f;
    public GameObject goalMessagePanel;
    public TMPro.TextMeshProUGUI goalMessageText;

    [Header("End Game Screen")]
    public GameObject endGameScreen;
    public TMPro.TextMeshProUGUI endGameTitleText;
    public TMPro.TextMeshProUGUI endGameScoreText;
    public TMPro.TextMeshProUGUI endGameHighScoreText;
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
    private void Start()
    {
        // Show the goal message at the start of the game
        ShowGoalMessage();

        // Reset timescale in case it was set to 0 in a previous game
        Time.timeScale = 1f;
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
    private void ShowGoalMessage()
    {
        if (goalMessagePanel != null && goalMessageText != null)
        {
            // Format goal message based on game settings
            string formattedGoal = stageGoalText;

            // If using time limit, include the time in the message
            if (useTimeLimit && stageGoalText.Contains("{time}"))
            {
                int minutes = Mathf.FloorToInt(gameTimeLimit / 60f);
                int seconds = Mathf.FloorToInt(gameTimeLimit % 60f);
                string timeFormat = string.Format("{0}:{1:00}", minutes, seconds);
                formattedGoal = stageGoalText.Replace("{time}", timeFormat);
            }

            // Set the goal text
            goalMessageText.text = formattedGoal;

            // Show the goal panel
            goalMessagePanel.SetActive(true);

            // Hide after duration
            StartCoroutine(HideGoalMessage());
        }
    }

    private IEnumerator HideGoalMessage()
    {
        yield return new WaitForSeconds(goalMessageDuration);
        if (goalMessagePanel != null)
        {
            goalMessagePanel.SetActive(false);
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
    }
    public void ShowEndGameScreen(string title)
    {
        if (gameOver) return;

        gameOver = true;

        DestroyAllEnemies();

        Time.timeScale = 0f; // Pause the game
        CursorManager.SetCursorForGameState(GameState.GameOver);

        // Get data from GameHUDManager
        GameHUDManager hudManager = GameHUDManager.instance;
        // Get current score
        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);

        // Update high score if needed
        UpdateHighScore(currentScore);
        // Set up end game screen
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);

            if (endGameTitleText != null)
                endGameTitleText.text = title;

            if (endGameScoreText != null)
                endGameScoreText.text = "Score: " + currentScore.ToString();

            if (endGameHighScoreText != null)
                endGameHighScoreText.text = "High Score: " + GetHighScore().ToString();

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
    private void UpdateHighScore(int newScore)
    {
        int currentHighScore = GetHighScore();
        if (newScore > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save(); // Ensure the high score is saved immediately
        }
    }

    // Method to retrieve high score
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
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