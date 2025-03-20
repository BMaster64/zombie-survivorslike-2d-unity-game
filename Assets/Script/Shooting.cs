using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePos;

    public GameObject bullet;
    public Transform bulletTransform;
    public Transform player;  // Tham chiếu đến nhân vật để kiểm tra hướng
    public SpriteRenderer crosshairSprite;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
    private PlayerStats playerStats;

    // Shotgun parameters
    public bool isShotgun = true;
    public int bulletsPerShot = 5;        // Number of bullets per shotgun blast
    public float spreadAngle = 30f;       // Total angle of the spread in degrees
    public float minSpreadVariation = 0.7f; // Minimum random spread multiplier (0-1)
    public float maxSpreadVariation = 1.3f; // Maximum random spread multiplier (>1)

    // Speed variation parameters (shotgun only)
    public float minSpeedMultiplier = 0.8f; // Minimum speed multiplier for shotgun bullets
    public float maxSpeedMultiplier = 1.2f; // Maximum speed multiplier for shotgun bullets

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Cursor.visible = false; // Hide default cursor

        playerStats = player.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            playerStats = player.GetComponentInParent<PlayerStats>();
        }
    }

    void Update()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure crosshair is in the same plane

        // Update crosshair sprite position
        if (crosshairSprite != null)
        {
            crosshairSprite.transform.position = mousePos;
        }

        // Tính hướng từ rotatePoint đến chuột
        Vector3 direction = mousePos - transform.position;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Kiểm tra hướng nhân vật (player) để điều chỉnh xoay cho đúng
        bool isFlipped = player.localScale.x < 0;
        if (isFlipped)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotZ + 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        float adjustedTimeBetweenFiring = timeBetweenFiring;
        if (playerStats != null)
        {
            adjustedTimeBetweenFiring /= playerStats.attackSpeedMultiplier; // Lower time = faster firing
        }

        // Cơ chế bắn tự động
        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > adjustedTimeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (canFire)
        {
            canFire = false;
            if (isShotgun)
            {
                FireShotgun();
            }
            else
            {
                // Original single bullet firing behavior - no speed modification
                Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            }
        }
    }

    void FireShotgun()
    {
        // Calculate the base direction (used as the center of the spread)
        Vector3 baseDirection = mousePos - bulletTransform.position;
        baseDirection.Normalize();

        // Calculate the base angle for the spread
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            // Calculate the angle for this bullet
            float angleOffset;

            if (bulletsPerShot > 1)
            {
                // Distribute bullets evenly across the spread angle
                float normalizedIndex = (float)i / (bulletsPerShot - 1);
                angleOffset = Mathf.Lerp(-spreadAngle / 2, spreadAngle / 2, normalizedIndex);

                // Add random variation
                float randomVariation = Random.Range(minSpreadVariation, maxSpreadVariation);
                angleOffset *= randomVariation;
            }
            else
            {
                // If only one bullet, just add a small random deviation
                angleOffset = Random.Range(-5f, 5f);
            }

            // Convert angle to radians and calculate the new direction
            float angleRad = (baseAngle + angleOffset) * Mathf.Deg2Rad;
            Vector3 bulletDirection = new Vector3(
                Mathf.Cos(angleRad),
                Mathf.Sin(angleRad),
                0
            );

            // Generate a random speed multiplier for this bullet
            float speedMultiplier = Random.Range(minSpeedMultiplier, maxSpeedMultiplier);

            // Instantiate the bullet
            GameObject newBullet = Instantiate(bullet, bulletTransform.position, Quaternion.identity);

            // Set the custom direction and speed for this bullet
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(bulletDirection);
                bulletScript.SetSpeedMultiplier(speedMultiplier);
            }
        }
    }
}