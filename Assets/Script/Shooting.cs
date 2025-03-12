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
        if (player.localScale.x < 0)
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
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
        }
    }

}
