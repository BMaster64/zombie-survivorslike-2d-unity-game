using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private int minObstacles = 20;
    [SerializeField] private int maxObstacles = 50;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.5f;

    [Header("Map Settings")]
    [SerializeField] private float mapWidth = 50f;
    [SerializeField] private float mapHeight = 50f;
    [SerializeField] private Transform mapCenter;
    [SerializeField] private float playerSafeRadius = 5f;
    
    [Header("Placement Settings")]
    [SerializeField] private int maxPlacementAttempts = 50;
    [SerializeField] private float minDistanceBetweenObstacles = 2f;

    [Header("Navigation Settings")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    private List<Collider2D> placedObstacleColliders = new List<Collider2D>();
    
    private void Start()
    {
        if (navMeshSurface == null)
        {
            navMeshSurface = FindObjectOfType<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                Debug.LogError("No NavMeshSurface found in the scene. Navigation will not work.");
            }
        }
        GenerateObstacles();
    }
    
    public void GenerateObstacles()
    {
        // Clear any existing obstacles from a previous run
        ClearObstacles();
        
        // Determine how many obstacles to spawn this time
        int numObstacles = Random.Range(minObstacles, maxObstacles + 1);
        
        for (int i = 0; i < numObstacles; i++)
        {
            if (!TryPlaceObstacle(maxPlacementAttempts))
            {
                Debug.Log("Reached maximum placement attempts, stopping obstacle generation");
                break;
            }
        }
        
        Debug.Log($"Successfully placed {placedObstacleColliders.Count} obstacles");
        RebakeNavMesh();
    }
    private void RebakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            Debug.Log("Rebaking navigation mesh asynchronously...");
            StartCoroutine(RebakeNavMeshAsync());
        }
        else
        {
            Debug.LogError("Cannot rebake navigation mesh: NavMeshSurface is null");
        }
    }

    private IEnumerator RebakeNavMeshAsync()
    {
        AsyncOperation asyncOperation = navMeshSurface.BuildNavMeshAsync();

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("Navigation mesh async rebake complete");
    }

    private bool TryPlaceObstacle(int maxAttempts)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Select a random obstacle prefab
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            
            // Generate random position within map bounds
            float xPos = Random.Range(-mapWidth/2, mapWidth/2);
            float yPos = Random.Range(-mapHeight/2, mapHeight/2);
            Vector3 position = new Vector3(xPos, yPos, 0) + mapCenter.position;
            
            // Check if position is within player safe zone
            if (Vector3.Distance(position, mapCenter.position) < playerSafeRadius)
            {
                continue; // Skip this position, it's too close to the player start
            }
            
            // Generate random rotation and scale
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            float scale = Random.Range(minScale, maxScale);
            Vector3 scaleVector = new Vector3(scale, scale, 1f);
            
            // Check for overlap with existing obstacles
            bool overlapping = false;
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(position, minDistanceBetweenObstacles);
            foreach (var collider in nearbyColliders)
            {
                if (placedObstacleColliders.Contains(collider))
                {
                    overlapping = true;
                    break;
                }
            }
            
            if (!overlapping)
            {
                // Place the obstacle
                GameObject obstacle = Instantiate(obstaclePrefab, position, rotation);
                obstacle.transform.localScale = scaleVector;
                obstacle.transform.parent = transform; // Parent to this GameObject for organization
                
                // Add collider to our list
                Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    placedObstacleColliders.Add(obstacleCollider);
                }
                
                return true;
            }
        }
        
        return false; // Failed to place after maximum attempts
    }
    
    private void ClearObstacles()
    {
        placedObstacleColliders.Clear();
        
        // Destroy all child objects (obstacles from previous generation)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    // Optional: Visualize the map bounds in the editor
    private void OnDrawGizmosSelected()
    {
        if (mapCenter == null)
            mapCenter = transform;
            
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mapCenter.position, new Vector3(mapWidth, mapHeight, 0));
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mapCenter.position, playerSafeRadius);
    }
    // Optional: Add a button to regenerate obstacles in the editor
    [ContextMenu("Regenerate Obstacles")]
    public void RegenerateObstacles()
    {
        if (Application.isPlaying)
        {
            GenerateObstacles();
        }
    }
}