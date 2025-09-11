using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public SpriteRenderer playerCharacterSprite;

    private void Start()
    {
        // Lấy stage và nhân vật đã chọn
        int selectedStage = PlayerPrefs.GetInt("SelectedStage", 1); // Mặc định stage 1 nếu không có
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0); // Mặc định nhân vật đầu tiên

        // Thiết lập nhân vật trong game
        Character currentCharacter = characterDB.GetCharacter(selectedCharacter);
        playerCharacterSprite.sprite = currentCharacter.characterSprite;

        // Bạn có thể thêm logic để khởi tạo stage tương ứng
        InitializeStage(selectedStage);
    }

    private void InitializeStage(int stageNumber)
    {
        // Logic khởi tạo stage dựa trên số stage
        switch (stageNumber)
        {
            case 1:
                // Khởi tạo stage 1
                Debug.Log("Initializing Stage 1");
                break;
            case 2:
                // Khởi tạo stage 2
                Debug.Log("Initializing Stage 2");
                break;
            default:
                Debug.LogError("Invalid stage number");
                break;
        }
    }
}
