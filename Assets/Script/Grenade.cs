using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    public float explosionDelay = 1.5f;
    public float explosionRadius = 2f;
    public float baseDamage = 15f;
    public GameObject explosionEffectPrefab;

    private Vector3 targetPosition;
    private float speed = 5f;
    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;
    private float throwHeight = 1.5f; // How high the grenade arcs
    private Vector3 startPosition;
    private float journeyTime;
    private float journeyLength;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats = PlayerHealth.instance.GetComponent<PlayerStats>();
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        journeyTime = 0f;

        // Start explosion countdown
        StartCoroutine(ExplosionCountdown());
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            // Calculate journey progress
            journeyTime += Time.deltaTime * speed;
            float percentComplete = journeyTime / (journeyLength / speed);

            // Calculate arc movement
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, percentComplete);

            // Add arc height using Sine curve
            currentPos.y += Mathf.Sin(percentComplete * Mathf.PI) * throwHeight;

            // Apply position
            transform.position = currentPos;

            // Rotate the grenade while it's flying
            transform.Rotate(0, 0, 360 * Time.deltaTime);

            // If reached destination, stop moving
            if (percentComplete >= 1f)
            {
                targetPosition = Vector3.zero;
            }
        }
    }

    IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    void Explode()
    {
        // Apply damage to all enemies within radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));

        // Calculate final damage considering player stats
        float finalDamage = baseDamage * (playerStats != null ? playerStats.attackMultiplier : 1f);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Apply damage to each enemy in explosion radius
            enemy.GetComponent<EnemyMovement>()?.TakeDamage(finalDamage);
        }

        // Spawn explosion effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the grenade
        Destroy(gameObject);
    }

    // Draw explosion radius in editor for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}