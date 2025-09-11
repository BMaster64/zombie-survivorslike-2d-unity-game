using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;
    public float baseDamage;
    private PlayerStats playerStats;

    // Added fields to store custom direction and speed multiplier
    private Vector3? customDirection = null;
    private float speedMultiplier = 1f;  // Default to 1 (normal speed)

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        playerStats = PlayerHealth.instance.GetComponent<PlayerStats>();

        Vector3 direction;

        // Use custom direction if provided, otherwise use mouse position
        if (customDirection.HasValue)
        {
            direction = customDirection.Value;
        }
        else
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePos - transform.position;
        }

        // Apply the speed multiplier to the force
        float adjustedForce = force * speedMultiplier;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * adjustedForce;

        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Add this method to set a custom direction
    public void SetDirection(Vector3 direction)
    {
        customDirection = direction;
    }

    // Add this method to set a custom speed multiplier
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            float finalDamage = baseDamage * (playerStats != null ? playerStats.attackMultiplier : 1f);
            collision.GetComponent<EnemyMovement>().TakeDamage(finalDamage);
            Destroy(gameObject);
        }
        else if (collision.tag == "Boss")
        {
            float finalDamage = baseDamage * (playerStats != null ? playerStats.attackMultiplier : 1f);
            collision.GetComponent<BossEnemy>().TakeDamage(finalDamage);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}