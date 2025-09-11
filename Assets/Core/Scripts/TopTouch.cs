using UnityEngine;

public class CollisionController : MonoBehaviour
{
    void Start()
    {
        // Lấy danh sách tất cả các đối tượng trong layer Top
        GameObject[] topObjects = GameObject.FindGameObjectsWithTag("Top");

        // Nếu đối tượng là Zombie, bỏ qua va chạm với layer Top
        if (gameObject.CompareTag("Zombie"))
        {
            foreach (GameObject topObject in topObjects)
            {
                Collider2D topCollider = topObject.GetComponent<Collider2D>();
                Collider2D zombieCollider = GetComponent<Collider2D>();

                if (topCollider != null && zombieCollider != null)
                {
                    Physics2D.IgnoreCollision(zombieCollider, topCollider, true);
                }
            }
        }
    }
}
