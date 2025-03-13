using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float lifetime = 0.5f;
    public bool expandEffect = true;
    public float maxSize = 2f;

    private float timer;
    private Vector3 originalScale;

    void Start()
    {
        timer = 0f;
        originalScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (expandEffect)
        {
            float scaleRatio = Mathf.Clamp01(timer / lifetime);
            transform.localScale = originalScale * Mathf.Lerp(0.1f, maxSize, scaleRatio);
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}