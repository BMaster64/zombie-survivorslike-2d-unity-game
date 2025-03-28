using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private GameObject player;
    private Camera mainCamera;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // Fixed map boundaries
    public Transform minSpawn, maxSpawn;
    public float minSpawnDistanceFromCamera = 2f; // Minimum distance outside camera view
    public List<WaveInfo> waves;

    private int currentWave;
    private float waveCounter;
    private List<EnemySpawnCounter> enemySpawnCounters = new List<EnemySpawnCounter>();
    public Transform enemySpawnPoint; // Thêm trường này



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;

        // Nếu không gán enemySpawnPoint, sử dụng transform hiện tại
        if (enemySpawnPoint == null)
        {
            enemySpawnPoint = transform;
        }

        currentWave = -1;
        GoToNextWave();
    }

    void Update()
    {
        if (PlayerHealth.instance.gameObject.activeSelf)
        {
            if (currentWave < waves.Count)
            {
                // Count down the current wave time
                waveCounter -= Time.deltaTime;
                if (waveCounter <= 0)
                {
                    GoToNextWave();
                }

                // Process each enemy type in the current wave
                for (int i = 0; i < enemySpawnCounters.Count; i++)
                {
                    EnemySpawnCounter counter = enemySpawnCounters[i];

                    // Check if we still need to spawn more of this enemy type
                    if (counter.remainingSpawns > 0)
                    {
                        counter.spawnTimer -= Time.deltaTime;
                        if (counter.spawnTimer <= 0)
                        {
                            // Reset timer and spawn enemy
                            counter.spawnTimer = counter.info.timeBetweenSpawns;
                            counter.remainingSpawns--;

                            GameObject newEnemy = Instantiate(counter.info.enemyPrefab, SelectSpawnPointOutsideCamera(), Quaternion.identity);
                            spawnedEnemies.Add(newEnemy);
                        }
                    }
                }
            }
        }

        // REMOVED: No longer updating spawner position to match player
    }

    public Vector3 SelectSpawnPointOutsideCamera()
    {
        if (mainCamera == null || player == null)
            return Vector3.zero;

        // Calculate visible camera bounds
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Get the camera's position (which should be centered on the player)
        Vector3 cameraPosition = mainCamera.transform.position;

        // Calculate visible bounds with the minimum spawn distance
        float visibleMinX = cameraPosition.x - (cameraWidth / 2) - minSpawnDistanceFromCamera;
        float visibleMaxX = cameraPosition.x + (cameraWidth / 2) + minSpawnDistanceFromCamera;
        float visibleMinY = cameraPosition.y - (cameraHeight / 2) - minSpawnDistanceFromCamera;
        float visibleMaxY = cameraPosition.y + (cameraHeight / 2) + minSpawnDistanceFromCamera;

        // Calculate fixed map bounds (these don't move with the player)
        float mapMinX = minSpawn.position.x;
        float mapMaxX = maxSpawn.position.x;
        float mapMinY = minSpawn.position.y;
        float mapMaxY = maxSpawn.position.y;

        // Attempt to find a valid spawn point
        Vector3 spawnPoint = Vector3.zero;
        bool validPointFound = false;
        int maxAttempts = 30;
        int attempts = 0;

        while (!validPointFound && attempts < maxAttempts)
        {
            attempts++;

            // Random position within map bounds
            float randomX = Random.Range(mapMinX, mapMaxX);
            float randomY = Random.Range(mapMinY, mapMaxY);
            spawnPoint = new Vector3(randomX, randomY, 0);

            // Check if point is outside camera view
            if (randomX < visibleMinX || randomX > visibleMaxX ||
                randomY < visibleMinY || randomY > visibleMaxY)
            {
                validPointFound = true;
            }
        }

        // If we couldn't find a valid point, use fallback method (spawn at map edge)
        if (!validPointFound)
        {
            return GetMapEdgeSpawnPoint(mapMinX, mapMaxX, mapMinY, mapMaxY);
        }

        return spawnPoint;
    }

    // Fallback method that spawns at map edge
    private Vector3 GetMapEdgeSpawnPoint(float minX, float maxX, float minY, float maxY)
    {
        Vector3 spawnPoint = Vector3.zero;

        // Choose a random edge (0=top, 1=right, 2=bottom, 3=left)
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Top edge
                spawnPoint.x = Random.Range(minX, maxX);
                spawnPoint.y = maxY;
                break;
            case 1: // Right edge
                spawnPoint.x = maxX;
                spawnPoint.y = Random.Range(minY, maxY);
                break;
            case 2: // Bottom edge
                spawnPoint.x = Random.Range(minX, maxX);
                spawnPoint.y = minY;
                break;
            case 3: // Left edge
                spawnPoint.x = minX;
                spawnPoint.y = Random.Range(minY, maxY);
                break;
        }

        return spawnPoint;
    }

    public void GoToNextWave()
    {
        currentWave++;

        if (currentWave >= waves.Count)
        {
            currentWave = waves.Count - 1;
        }

        // Set the wave time
        waveCounter = waves[currentWave].waveLength;

        // Initialize spawn counters for each enemy type in this wave
        enemySpawnCounters.Clear();
        foreach (EnemySpawnInfo enemyInfo in waves[currentWave].enemies)
        {
            EnemySpawnCounter counter = new EnemySpawnCounter
            {
                info = enemyInfo,
                spawnTimer = enemyInfo.initialDelay,
                remainingSpawns = enemyInfo.numberOfEnemies
            };

            enemySpawnCounters.Add(counter);
        }
    }

    // Helper class to track spawning of each enemy type
    private class EnemySpawnCounter
    {
        public EnemySpawnInfo info;
        public float spawnTimer;
        public int remainingSpawns;
    }

    // Gizmo to visualize map boundaries in the editor
    private void OnDrawGizmos()
    {
        if (minSpawn != null && maxSpawn != null)
        {
            Gizmos.color = Color.red;
            Vector3 size = maxSpawn.position - minSpawn.position;
            Vector3 center = minSpawn.position + size * 0.5f;
            Gizmos.DrawWireCube(center, size);
        }
    }
}

[System.Serializable]
public class WaveInfo
{
    public float waveLength = 30f;
    public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public float timeBetweenSpawns = 1f;
    public float initialDelay = 0f; // Delay before first spawn of this enemy type
}