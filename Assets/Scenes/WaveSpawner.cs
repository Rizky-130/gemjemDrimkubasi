using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Pool")]
    public List<Enemy> enemies = new List<Enemy>();

    [Header("Wave Settings")]
    public int currWave = 1;
    public int baseWaveValue = 10;
    public int waveValueIncrease = 5;
    public int maxEnemiesPerWave = 50;

    private int waveValue;

    [Header("Spawn Settings")]
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public Transform[] spawnLocation;

    [Tooltip("Random area around each spawn point.")]
    public float randomSpawnRadius = 1.5f;

    [Header("Timing")]
    public float waveDuration = 20f;
    public float timeBetweenWaves = 3f;

    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    private float nextWaveTimer;

    [Header("Runtime Info")]
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public bool isSpawningWave = false;
    public bool waitingForNextWave = false;

    private void Start()
    {
        GenerateWave();
    }

    private void Update()
    {
        CleanEnemyList();

        if (waitingForNextWave)
        {
            nextWaveTimer -= Time.deltaTime;

            if (nextWaveTimer <= 0f)
            {
                currWave++;
                GenerateWave();
            }

            return;
        }

        if (isSpawningWave)
        {
            HandleSpawning();
        }

        if (!isSpawningWave && enemiesToSpawn.Count <= 0 && spawnedEnemies.Count <= 0)
        {
            StartNextWaveTimer();
        }
    }

    private void HandleSpawning()
    {
        waveTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && enemiesToSpawn.Count > 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }

        if (enemiesToSpawn.Count <= 0)
        {
            isSpawningWave = false;
            Debug.Log("Wave " + currWave + " finished spawning. Kill remaining enemies.");
        }

        if (waveTimer <= 0f)
        {
            isSpawningWave = false;
            enemiesToSpawn.Clear();
            Debug.Log("Wave timer ended. No more enemies will spawn this wave.");
        }
    }

    private void SpawnEnemy()
    {
        if (spawnLocation == null || spawnLocation.Length <= 0)
        {
            Debug.LogError("No spawn locations assigned!");
            return;
        }

        if (enemiesToSpawn.Count <= 0)
            return;

        GameObject enemyPrefab = enemiesToSpawn[0];
        enemiesToSpawn.RemoveAt(0);

        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is missing. Skipping spawn.");
            return;
        }

        Transform selectedSpawn = GetRandomSpawnPoint();

        if (selectedSpawn == null)
        {
            Debug.LogError("No valid spawn point found!");
            return;
        }

        Vector2 randomOffset = Random.insideUnitCircle * randomSpawnRadius;

        Vector3 spawnPosition = selectedSpawn.position + new Vector3(
            randomOffset.x,
            randomOffset.y,
            0f
        );

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedEnemies.Add(enemy);

        EnemyWaveTracker tracker = enemy.GetComponent<EnemyWaveTracker>();

        if (tracker == null)
        {
            tracker = enemy.AddComponent<EnemyWaveTracker>();
        }

        tracker.Setup(this);

        Debug.Log("Enemy spawned randomly. Remaining to spawn: " + enemiesToSpawn.Count);
    }

    private Transform GetRandomSpawnPoint()
    {
        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Range(0, spawnLocation.Length);

            if (spawnLocation[randomIndex] != null)
            {
                return spawnLocation[randomIndex];
            }
        }

        return null;
    }

    public void GenerateWave()
    {
        waitingForNextWave = false;
        isSpawningWave = true;

        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();

        waveValue = baseWaveValue + (currWave - 1) * waveValueIncrease;

        GenerateEnemies();

        if (enemiesToSpawn.Count <= 0)
        {
            Debug.LogWarning("No enemies generated. Check enemy prefabs and enemy costs.");
            isSpawningWave = false;
            StartNextWaveTimer();
            return;
        }

        spawnInterval = waveDuration / enemiesToSpawn.Count;
        spawnTimer = 0f;
        waveTimer = waveDuration;

        Debug.Log("Wave " + currWave + " started. Enemies: " + enemiesToSpawn.Count);
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();

        if (enemies == null || enemies.Count <= 0)
        {
            Debug.LogError("No enemy types assigned in WaveSpawner!");
            enemiesToSpawn = generatedEnemies;
            return;
        }

        int safetyLoop = 0;

        while (waveValue > 0 && generatedEnemies.Count < maxEnemiesPerWave)
        {
            safetyLoop++;

            if (safetyLoop > 500)
            {
                Debug.LogWarning("Enemy generation stopped by safety loop.");
                break;
            }

            List<Enemy> affordableEnemies = new List<Enemy>();

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].enemyPrefab != null && enemies[i].cost > 0 && enemies[i].cost <= waveValue)
                {
                    affordableEnemies.Add(enemies[i]);
                }
            }

            if (affordableEnemies.Count <= 0)
            {
                break;
            }

            int randEnemyId = Random.Range(0, affordableEnemies.Count);
            Enemy selectedEnemy = affordableEnemies[randEnemyId];

            generatedEnemies.Add(selectedEnemy.enemyPrefab);
            waveValue -= selectedEnemy.cost;
        }

        enemiesToSpawn = generatedEnemies;
    }

    private void StartNextWaveTimer()
    {
        waitingForNextWave = true;
        nextWaveTimer = timeBetweenWaves;

        Debug.Log("Wave " + currWave + " cleared! Next wave soon.");
    }

    private void CleanEnemyList()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost = 1;
}