using UnityEngine;

public class StageSelectionManager : MonoBehaviour
{
    public static int SelectedStage { get; private set; }

    public void SelectStage(int stageId)
    {
        // Lưu stage được chọn 
        SelectedStage = stageId;
        
        // Cũng lưu vào PlayerPrefs để đảm bảo dữ liệu không bị mất
        PlayerPrefs.SetInt("SelectedStage", stageId);
        PlayerPrefs.Save();

        // Chuyển sang Scene chọn nhân vật
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
    }
    
    // Load SelectedStage từ PlayerPrefs khi game khởi động
    private void Awake()
    {
        if (SelectedStage == 0 && PlayerPrefs.HasKey("SelectedStage"))
        {
            SelectedStage = PlayerPrefs.GetInt("SelectedStage", 1);
        }
    }
    
    private void Start()
    {
        // Ensure cursor is visible in stage selection
        CursorManager.SetCursorForGameState(GameState.StageSelection);
    }
}