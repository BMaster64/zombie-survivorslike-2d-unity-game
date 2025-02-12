using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header("Timer")]
    public TextMeshProUGUI timerText;
    private float gameTime = 0f;

    [Header("XP System")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

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

    public void TriggerLevelUpMenu()
    {
        LevelUpMenu.instance.OpenLevelUpMenu();
        Cursor.visible = true;
    }
}
