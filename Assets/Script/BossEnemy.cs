using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    // References
    private GameObject player;
    private Transform target;
    public GameObject gasBombPrefab;
    private Animator animator;
    private Rigidbody2D rb;

    // Movement settings
    public float moveSpeed = 3f;
    public float damage = 40f;
    public float health = 500f;
    public float attackRange = 5f;
    public float attackCooldown = 3f;
    private float lastAttackTime;
    private bool isAttacking = false;
    public int scoreValue = 1;
    public float hitWaitTime = 0.5f;
    private float hitCounter;

    public GameObject? drop;
    public int xpDropAmount = 1;
    // Animation parameter names
    private const string ATTACK_TRIGGER = "attack";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.GetComponent<Transform>();
        }

        lastAttackTime = -attackCooldown; // Allow immediate attack
    }

    void Update()
    {
        if (player == null) return;

        if (hitCounter > 0)
        {
            hitCounter -= Time.deltaTime;
        }

        // Check if we should attack
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }

        // Move if not attacking
        if (!isAttacking)
        {
            MoveTowardPlayer();
        }
    }

    void MoveTowardPlayer()
    {
        if (player == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Flip sprite if needed
        if (direction.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0)
        {
            // Notify GameManager that boss is defeated
            if (GameManager.instance != null)
            {
                GameHUDManager.instance.AddScore(scoreValue);
                GameManager.instance.BossDefeated();
            }

            // Spawn XP drops
            SpawnXPDrops();
            Destroy(gameObject);
        }

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }
    private void SpawnXPDrops()
    {
        if (drop == null || xpDropAmount <= 0) return;

        for (int i = 0; i < xpDropAmount; i++)
        {
            // Calculate a random position within the drop radius
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Instantiate the XP prefab
            Instantiate(drop, dropPosition, Quaternion.identity);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player && hitCounter <= 0f)
        {
            player.TakeDamage(damage);
            hitCounter = hitWaitTime;
        }
    }
    IEnumerator PerformAttack()
    {
        // Set attacking state
        isAttacking = true;
        lastAttackTime = Time.time;

        // Stop moving
        rb.linearVelocity = Vector2.zero;

        // Trigger attack animation
        animator.SetTrigger(ATTACK_TRIGGER);

        // Wait for animation to reach spawn point (adjust time as needed)
        yield return new WaitForSeconds(0.5f);

        // Spawn gas bomb
        SpawnGasBomb();

        // Wait for attack animation to finish (adjust based on your animation length)
        yield return new WaitForSeconds(1.0f);

        // Return to moving state
        isAttacking = false;
    }

    void SpawnGasBomb()
    {
        if (gasBombPrefab != null && player != null)
        {
            Vector2 spawnOffset = new Vector2(1.5f, 0);
            if (transform.localScale.x > 0)
            {
                spawnOffset.x = -spawnOffset.x;
            }
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

            GameObject bomb = Instantiate(gasBombPrefab, spawnPosition, Quaternion.identity);
            GasBomb bombScript = bomb.GetComponent<GasBomb>();

            if (bombScript != null)
            {
                bombScript.SetTarget(target);
            }
        }
    }
}