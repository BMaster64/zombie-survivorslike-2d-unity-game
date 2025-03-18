using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Transform target;
    private GameObject player;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public Transform minSpawn, maxSpawn;
    public List<WaveInfo> waves;

    private int currentWave;
    private float waveCounter;
    private List<EnemySpawnCounter> enemySpawnCounters = new List<EnemySpawnCounter>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }

        currentWave = -1;
        GoToNextWave();
    }

    // Update is called once per frame
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

                            GameObject newEnemy = Instantiate(counter.info.enemyPrefab, SelectSpawnPoint(), Quaternion.identity);
                            spawnedEnemies.Add(newEnemy);
                        }
                    }
                }
            }
        }

        // Keep the spawner at the player's position
        if (target != null)
        {
            transform.position = target.position;
        }
    }

    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        bool spawnVerticalEdge = Random.Range(0f, 1f) > 0.5f;

        if (spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            }
            else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        }
        else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);

            if (Random.Range(0f, 1f) > 0.5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
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