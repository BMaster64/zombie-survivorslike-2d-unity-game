using UnityEngine;
 
public class XPLevelController : MonoBehaviour
{
    public static XPLevelController instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100; // Base XP needed
    public float experienceMultiplier = 1.2f; // How much more XP needed per level

    [Header("UI References")]
    public GameHUDManager hudManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Initialize UI
        if (hudManager != null)
        {
            hudManager.UpdateLevelUI(currentLevel);
            hudManager.UpdateXPBar(currentExperience, experienceToNextLevel);
        }
    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;

        // Check for level up
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }

        // Update UI
        if (hudManager != null)
        {
            hudManager.UpdateXPBar(currentExperience, experienceToNextLevel);
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * experienceMultiplier);

        // Update UI and trigger level up menu
        if (hudManager != null)
        {
            hudManager.UpdateLevelUI(currentLevel);
            hudManager.TriggerLevelUpMenu();
        }
    }
}
