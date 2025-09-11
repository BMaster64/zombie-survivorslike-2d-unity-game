using System.Collections;
using UnityEngine;

public class GasBomb : MonoBehaviour
{
    // References
    private Transform target;
    private SpriteRenderer spriteRenderer;

    // Settings
    public float moveSpeed = 2f;
    public float lifetime = 5f;
    public float damage = 10f;
    public float damageRadius = 2f;
    public float fadeTime = 1.5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeBomb());
    }

    void Update()
    {
        if (target != null)
        {
            // Move toward target position
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    IEnumerator FadeBomb()
    {
        // Wait before starting to fade
        yield return new WaitForSeconds(lifetime - fadeTime);

        // Get initial color
        Color startColor = spriteRenderer.color;

        // Gradually fade out
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Destroy the bomb when fully faded
        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            // Apply damage - replace with your health system
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}