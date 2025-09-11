using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public CharacterDatabase characterDB;
    public Transform spawnPoint;

    private void Start()
    {
        Debug.Log("CharacterSpawner Start method called");

        // Lấy nhân vật đã chọn từ PlayerPrefs
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Debug.Log($"Selected Character Index: {selectedCharacterIndex}");

        // Lấy character từ database
        Character selectedCharacter = characterDB.GetCharacter(selectedCharacterIndex);

        Debug.Log(selectedCharacter != null ? "Character found" : "Character is null");

        // Spawn nhân vật tại điểm spawn
        if (selectedCharacter?.characterPrefab != null)
        {
            Debug.Log("Instantiating character prefab");
            GameObject spawnedCharacter = Instantiate(selectedCharacter.characterPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Ensure the spawned character has the "Player" tag
            if (spawnedCharacter != null)
            {
                spawnedCharacter.tag = "Player";
                Debug.Log($"Character spawned at position: {spawnPoint.position} with tag: {spawnedCharacter.tag}");
            }
        }
        else
        {
            Debug.LogError("Character or Character Prefab not found!");
        }
    }
}