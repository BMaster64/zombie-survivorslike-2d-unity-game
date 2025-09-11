using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{

    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSprite;
    private int selectedOption = 0;
    private int selectedStage = 0;
    private void Start()
    {
        // Always load saved data to get the correct selections
        Load();
        UpdateCharacter(selectedOption);
        
        // Ensure cursor is visible in character selection
        CursorManager.SetCursorForGameState(GameState.CharacterSelection);
    }
    public void NextCharacter()
    {
        selectedOption++;
        if (selectedOption >= characterDB.characters.Length)
        {
            selectedOption = 0;
        }
        UpdateCharacter(selectedOption);
        Save();

    }
    public void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);
        artworkSprite.sprite = character.characterSprite;
    }
    public void PreviousCharacter()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = characterDB.characters.Length - 1;
        }
        UpdateCharacter(selectedOption);
        Save();
    }
    public void SetSelectedStage(int stageId)
    {
        selectedStage = stageId;
        PlayerPrefs.SetInt("SelectedStage", stageId);
    }
    // Phương thức để bắt đầu game với nhân vật và stage đã chọn
    public void StartGame()
    {
        // Lưu stage và nhân vật được chọn
        PlayerPrefs.SetInt("SelectedStage", StageSelectionManager.SelectedStage);
        PlayerPrefs.SetInt("SelectedCharacter", selectedOption);

        // Chuyển sang scene game tương ứng với stage
        switch (StageSelectionManager.SelectedStage)
        {
            case 1:
                SceneManager.LoadScene("Stage01");
                break;
            case 2:
                SceneManager.LoadScene("Stage02");
                break;
            default:
                Debug.LogError("Invalid Stage Selected");
                break;
        }
    }
    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Lấy stage được chọn từ StageSelectionManager (nếu có)
        // hoặc từ PlayerPrefs (nếu đã được lưu trước đó)
        if (StageSelectionManager.SelectedStage != 0)
        {
            selectedStage = StageSelectionManager.SelectedStage;
        }
        else if (PlayerPrefs.HasKey("SelectedStage"))
        {
            selectedStage = PlayerPrefs.GetInt("SelectedStage", 1);
        }
        else
        {
            selectedStage = 1; // Mặc định stage 1
        }
    }
    private void Save()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedOption);
        // Chỉ lưu character selection khi thay đổi, stage sẽ được lưu trong StartGame()
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được lưu
    }

    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}


