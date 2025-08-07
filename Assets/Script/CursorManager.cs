using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;
    
    [Header("Cursor Settings")]
    public bool showCursorInMenus = true;
    public bool hideCursorInGameplay = true;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Show cursor for menu navigation
    /// </summary>
    public static void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    /// <summary>
    /// Hide cursor for gameplay (but allow free movement)
    /// </summary>
    public static void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None; // Allow free movement, just invisible
    }
    
    /// <summary>
    /// Set cursor visibility based on game state
    /// </summary>
    /// <param name="gameState">The current game state</param>
    public static void SetCursorForGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
            case GameState.StageSelection:
            case GameState.CharacterSelection:
            case GameState.Paused:
            case GameState.LevelUp:
            case GameState.GameOver:
                ShowCursor();
                break;
                
            case GameState.Playing:
                HideCursor();
                break;
                
            default:
                ShowCursor(); // Default to visible for safety
                break;
        }
    }
    
    /// <summary>
    /// Force reset cursor state - useful when cursor gets stuck
    /// </summary>
    public static void ForceResetCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Get current cursor state for debugging
    /// </summary>
    public static string GetCursorState()
    {
        return $"Visible: {Cursor.visible}, LockState: {Cursor.lockState}";
    }
}

/// <summary>
/// Enum representing different game states for cursor management
/// </summary>
public enum GameState
{
    MainMenu,
    StageSelection,
    CharacterSelection,
    Playing,
    Paused,
    LevelUp,
    GameOver
}