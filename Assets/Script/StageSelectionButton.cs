using UnityEngine;

public class StageSelectionManager : MonoBehaviour
{
    public static int SelectedStage { get; private set; }

    public void SelectStage(int stageId)
    {
        // Lưu stage được chọn 
        SelectedStage = stageId;

        // Chuyển sang Scene chọn nhân vật
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
    }
}