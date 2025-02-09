using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D theRigidbody;

    public float moveSpeed;

    public Animator animator;

    public float pickupRange = 2.5f;

    private Transform healthbarTransform;

    private void Awake()
    {
        theRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthbarTransform = transform.Find("Healthbar");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(0f, 0f, 0f);

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        theRigidbody.linearVelocity = moveInput * moveSpeed;

        if (moveInput != Vector3.zero)
        {
            animator.SetBool("Move", true);
            if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1); // Facing right
                if (healthbarTransform != null)
                    healthbarTransform.localScale = new Vector3((float).01, (float).01, 1);
            }
            else if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Facing left
                if (healthbarTransform != null)
                    healthbarTransform.localScale = new Vector3((float) -.01, (float).01, 1);
            }
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }
}
