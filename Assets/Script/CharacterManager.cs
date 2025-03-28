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
        if (PlayerPrefs.HasKey("SelectedCharacter"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(selectedOption);
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
                SceneManager.LoadScene("Stage01 1");
                break;
            case 2:
                SceneManager.LoadScene("Stage02 1");
                break;
            default:
                Debug.LogError("Invalid Stage Selected");
                break;
        }
    }
    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("SelectedCharacter");

        // Nếu có stage được lưu trước đó
        if (PlayerPrefs.HasKey("SelectedStage"))
        {
            selectedStage = PlayerPrefs.GetInt("SelectedStage");
        }
    }
    private void Save()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedOption);
        PlayerPrefs.SetInt("SelectedStage", selectedStage);
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được lưu
    }

    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}


