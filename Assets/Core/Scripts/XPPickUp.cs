using UnityEngine;

public class XPPickUp : MonoBehaviour
{
    public int expValue;

    private bool movingToPlayer;
    public float moveSpeed;

    public float timeBetweenChecks = 0.2f;
    private float checkCounter;

    private PlayerMovement player;
    private PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerHealth.instance.GetComponent<PlayerMovement>();
        playerStats = player.GetComponent<PlayerStats>();

    }

    // Update is called once per frame
    void Update()
    {
        if (movingToPlayer == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            checkCounter -= Time.deltaTime;
            if(checkCounter <= 0)
            {
                checkCounter = timeBetweenChecks;
                float effectiveRange = player.pickupRange;
                if (Vector3.Distance(transform.position, player.transform.position) < effectiveRange)
                {
                    movingToPlayer = true;
                    moveSpeed += player.moveSpeed;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            int modifiedExpValue = expValue;
            if (playerStats != null)
            {
                modifiedExpValue = Mathf.RoundToInt(expValue * playerStats.xpGainMultiplier);
            }

            XPLevelController.instance.GetExp(modifiedExpValue);
            Destroy(gameObject);
        }
    }
}
