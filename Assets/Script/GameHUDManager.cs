using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    public static GameHUDManager instance;

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public float gameTime = 0f;

    [Header("XP System")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    [Header("Score System")]
    public TextMeshProUGUI scoreText;
    private int playerScore = 0;

    void Awake()
    {
        // Set up singleton instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize score display
        UpdateScoreUI();
    }

    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        gameTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateXPBar(float currentXP, float maxXP)
    {
        xpBar.value = currentXP / maxXP;
    }

    public void UpdateLevelUI(int level)
    {
        levelText.text = "Lvl " + level.ToString();
    }

    public void AddScore(int points)
    {
        playerScore += points;
        UpdateScoreUI();

        // Store current score for end game screen
        PlayerPrefs.SetInt("CurrentScore", playerScore);
    }

    public void UpdateScoreUI()
    {
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void TriggerLevelUpMenu()
    {
        LevelUpMenu.instance.OpenLevelUpMenu();
        Cursor.visible = true;
    }
}