using UnityEngine;

public class WeaponsContainer : MonoBehaviour
{
    private Transform playerTransform;

    private void Start()
    {
        // Find the player
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // If not found, try finding PlayerStats
        if (playerTransform == null)
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerTransform = playerStats.transform;
            }
        }

        // Position the container at the player's position initially
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    private void LateUpdate()
    {
        // Follow the player without being a child - immune to player's scale changes
        if (playerTransform != null)
        {
            // Only update position, not rotation or scale
            transform.position = playerTransform.position;
        }
        else
        {
            // Try to find the player again if reference is lost
            Start();
        }
    }
}