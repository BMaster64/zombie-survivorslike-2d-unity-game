using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody2D theRigidbody;
    public float moveSpeed, damage;
    private GameObject player;
    private Transform target;
    private NavMeshAgent agent;

    // Debug variables
    private bool isOnNavMesh = true;
    private bool hasValidPath = false;

    public float hitWaitTime = 0.5f;
    private float hitCounter;

    public float health = 10f;

    public float knockBackTime = 0.5f;
    private float knockBackCounter;

    // Add path recalculation timer
    public float pathRecalculationTime = 0.5f;
    private float pathTimer;
    public int scoreValue = 1;
    public GameObject drop;

    // Start is called before the first frame update
    void Start()
    {
        // Set up NavMeshAgent for pathfinding
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Configure NavMeshAgent for 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Critical: Set appropriate values for 2D
        agent.speed = moveSpeed;
        agent.acceleration = moveSpeed * 2f;
        agent.angularSpeed = 0;
        agent.stoppingDistance = 0.1f;

        // For 2D games, keep a small radius and height
        agent.radius = GetComponent<Collider2D>().bounds.extents.x * 0.5f;
        agent.height = 0.1f;

        // Make Nav Agent only calculate paths but not control movement
        agent.updatePosition = false;
        agent.updateRotation = false;

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.GetComponent<Transform>();
            // Set initial destination
            agent.SetDestination(target.position);
        }

        moveSpeed = Random.Range(moveSpeed * 0.8f, moveSpeed * 1.2f);

        // Initialize path timer
        pathTimer = pathRecalculationTime;
    }

    private void Update()
    {
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;

            if (moveSpeed > 0)
            {
                moveSpeed = -moveSpeed * 2f;
            }

            if (knockBackCounter <= 0)
            {
                moveSpeed = Mathf.Abs(moveSpeed * 0.5f);
            }
        }

        // Update hitCounter
        if (hitCounter > 0)
        {
            hitCounter -= Time.deltaTime;
        }

        // Check if enemy is on NavMesh and fix if not
        NavMeshHit hit;
        isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas);

        // Path recalculation timer
        pathTimer -= Time.deltaTime;
        if (pathTimer <= 0 && target != null)
        {
            // Recalculate path periodically
            agent.ResetPath();
            agent.SetDestination(target.position);
            pathTimer = pathRecalculationTime;
        }

        // Check if path is valid
        hasValidPath = agent.hasPath && agent.pathStatus != NavMeshPathStatus.PathInvalid;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 moveDirection;

        // When under knockback, we don't change direction
        if (knockBackCounter > 0)
        {
            // During knockback, just let the existing velocity continue
            return;
        }

        // Determine movement direction
        if (isOnNavMesh && hasValidPath)
        {
            // We have a valid NavMesh path - use it
            moveDirection = agent.desiredVelocity.normalized;

            // If the desired velocity is almost zero but we should be moving, use direct direction
            if (moveDirection.magnitude < 0.1f)
            {
                moveDirection = (target.position - transform.position).normalized;
            }
        }
        else
        {
            // Fallback to direct movement when no NavMesh path
            moveDirection = (target.position - transform.position).normalized;
        }

        // Apply movement using original velocity-based approach
        theRigidbody.linearVelocity = moveDirection * moveSpeed;

        // Update facing direction based on movement
        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3((float)-1.44766, (float)1.44766, 1);  // Face right
        }
        else if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3((float)1.44766, (float)1.44766, 1); // Face left
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0)
        {
            if (GameHUDManager.instance != null)
            {
                GameHUDManager.instance.AddScore(scoreValue);
            }
            Destroy(gameObject);
            Instantiate(drop, transform.position, drop.transform.rotation);
        }

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);

        if (shouldKnockBack)
        {
            knockBackCounter = knockBackTime;
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

    // Keep the agent's position synced with actual transform
    void LateUpdate()
    {
        if (agent != null && agent.enabled)
        {
            // Keep agent's position in sync with transform
            agent.nextPosition = transform.position;
        }
    }

    // Visualization for debugging
    void OnDrawGizmosSelected()
    {
        if (agent != null && agent.hasPath)
        {
            // Draw the path
            Gizmos.color = Color.blue;
            var agentPath = agent.path;
            Vector3 prevCorner = transform.position;

            if (agentPath != null && agentPath.corners.Length > 0)
            {
                foreach (Vector3 corner in agentPath.corners)
                {
                    Gizmos.DrawLine(prevCorner, corner);
                    Gizmos.DrawSphere(corner, 0.1f);
                    prevCorner = corner;
                }
            }
        }
    }
}