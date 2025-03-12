using UnityEngine;

public class FlightKnife : MonoBehaviour
{
    public float rotationSpeed = 100f;  // Tốc độ quay của dao bay
    private Transform containerTransform;  // Parent transform (player or player component)
    private Vector3 offset;
    private PlayerStats playerStats;  // Reference to player stats
    private float baseRotationSpeed;

    void Start()
    {
        // Get parent transform
        containerTransform = transform.parent;

        // Set initial position (random starting point around player)
        offset = transform.position - containerTransform.position;

        // Store the base rotation speed
        baseRotationSpeed = rotationSpeed;

        // Find player stats
        playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        containerTransform = transform.parent;
        // Quay dao bay xung quanh player, không bị ảnh hưởng bởi góc quay của player
        float adjustedRotationSpeed = baseRotationSpeed;
        if (playerStats != null)
        {
            adjustedRotationSpeed = baseRotationSpeed * playerStats.attackSpeedMultiplier;
        }

        transform.position = containerTransform.position + offset;  // Đảm bảo dao bay giữ khoảng cách với player
        transform.RotateAround(containerTransform.position, Vector3.forward, adjustedRotationSpeed * Time.deltaTime);  // Quay dao xung quanh 
    }
}