using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody2D theRigidbody;
    public float moveSpeed, damage;
    private GameObject player;
    private Transform target;

    public float hitWaitTime = 0.5f;
    private float hitCounter;

    public float health = 10f;

    public float knockBackTime = 0.5f;
    private float knockBackCounter;

    public GameObject drop;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.GetComponent<Transform>();
        }

        moveSpeed = Random.Range(moveSpeed * 0.8f, moveSpeed * 1.2f);
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDirection = (target.position - transform.position).normalized;
        theRigidbody.linearVelocity = (target.position - transform.position).normalized * moveSpeed;
        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3((float)-1.44766, (float)1.44766, 1);  // Face right
        }
        else if (moveDirection.x < 0)
        {
            transform.localScale = new Vector3((float)1.44766, (float)1.44766, 1); // Face left
        }
        if (hitCounter > 0)
        {
            hitCounter -= Time.deltaTime;
        }
    }



    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0)
        {
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
            hitCounter = hitWaitTime;  // Đặt lại bộ đếm thời gian để tránh mất máu liên tục quá nhanh
        }
    }
}