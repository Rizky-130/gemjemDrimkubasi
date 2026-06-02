using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Pool")]
    public List<EnemySpawnOption> enemies = new List<EnemySpawnOption>();

    [Header("Fixed 10 Waves")]
    public List<FixedWave> waves = new List<FixedWave>();

    [Header("Runtime Wave Info")]
    public int currWave = 1;
    public bool isSpawningWave = false;
    public bool waitingForNextWave = false;
    public bool allWavesCleared = false;

    [Header("Spawn Settings")]
    public Transform[] spawnLocations;
    public float randomSpawnRadius = 1.5f;
    public float waveSpawnDuration = 10f;

    [Header("Delay Between Waves")]
    public float timeBetweenWaves = 3f;

    [Header("Runtime Lists")]
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    [Header("Events")]
    public UnityEvent onAllWavesCleared;

    private float spawnInterval;
    private float spawnTimer;
    private float nextWaveTimer;

    private FixedWave currentWaveData;

    private void Awake()
    {
        CreateDefaultWavesIfEmpty();
    }

    private void Start()
    {
        currWave = 1;
        isSpawningWave = false;
        waitingForNextWave = false;
        allWavesCleared = false;

        StartWave(currWave);
    }

    private void Update()
    {
        CleanEnemyList();

        if (allWavesCleared)
            return;

        if (waitingForNextWave)
        {
            HandleNextWaveDelay();
            return;
        }

        if (isSpawningWave)
        {
            HandleSpawning();
        }

        if (!isSpawningWave && enemiesToSpawn.Count <= 0 && spawnedEnemies.Count <= 0)
        {
            FinishCurrentWave();
        }
    }

    private void StartWave(int waveNumber)
    {
        if (waveNumber > waves.Count)
        {
            FinishAllWaves();
            return;
        }

        currentWaveData = waves[waveNumber - 1];

        currWave = waveNumber;
        isSpawningWave = true;
        waitingForNextWave = false;

        enemiesToSpawn.Clear();
        spawnedEnemies.Clear();

        GenerateEnemiesForWave(currentWaveData.enemyQuantity);

        if (enemiesToSpawn.Count <= 0)
        {
            Debug.LogWarning("Wave " + currWave + " has no enemies to spawn.");
            isSpawningWave = false;
            FinishCurrentWave();
            return;
        }

        spawnInterval = waveSpawnDuration / enemiesToSpawn.Count;
        spawnTimer = 0f;

        Debug.Log(
            currentWaveData.waveName +
            " started | Quantity: " + currentWaveData.enemyQuantity +
            " | HP: " + currentWaveData.enemyHP +
            " | Damage: " + currentWaveData.enemyDamage
        );

        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayWaveStart();
        }
    }

    private void GenerateEnemiesForWave(int quantity)
    {
        enemiesToSpawn.Clear();

        if (enemies == null || enemies.Count <= 0)
        {
            Debug.LogError("WaveSpawner has no enemy prefabs assigned!");
            return;
        }

        for (int i = 0; i < quantity; i++)
        {
            EnemySpawnOption selectedEnemy = GetRandomAvailableEnemy();

            if (selectedEnemy == null)
                continue;

            if (selectedEnemy.enemyPrefab == null)
                continue;

            enemiesToSpawn.Add(selectedEnemy.enemyPrefab);
        }
    }

    private EnemySpawnOption GetRandomAvailableEnemy()
    {
        List<EnemySpawnOption> availableEnemies = new List<EnemySpawnOption>();

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemySpawnOption enemy = enemies[i];

            if (enemy == null)
                continue;

            if (enemy.enemyPrefab == null)
                continue;

            if (currWave < enemy.unlockWave)
                continue;

            availableEnemies.Add(enemy);
        }

        if (availableEnemies.Count <= 0)
        {
            Debug.LogWarning("No available enemy type for Wave " + currWave);
            return null;
        }

        int totalWeight = 0;

        for (int i = 0; i < availableEnemies.Count; i++)
        {
            totalWeight += Mathf.Max(1, availableEnemies[i].spawnWeight);
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < availableEnemies.Count; i++)
        {
            currentWeight += Mathf.Max(1, availableEnemies[i].spawnWeight);

            if (randomValue < currentWeight)
            {
                return availableEnemies[i];
            }
        }

        return availableEnemies[0];
    }

    private void HandleSpawning()
    {
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
    }

    private void SpawnEnemy()
    {
        if (spawnLocations == null || spawnLocations.Length <= 0)
        {
            Debug.LogError("No spawn locations assigned in WaveSpawner!");
            return;
        }

        if (enemiesToSpawn.Count <= 0)
            return;

        GameObject enemyPrefab = enemiesToSpawn[0];
        enemiesToSpawn.RemoveAt(0);

        if (enemyPrefab == null)
            return;

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

        ApplyWaveStatsToEnemy(enemy, currentWaveData);

        spawnedEnemies.Add(enemy);

        EnemyWaveTracker tracker = enemy.GetComponent<EnemyWaveTracker>();

        if (tracker == null)
        {
            tracker = enemy.AddComponent<EnemyWaveTracker>();
        }

        tracker.Setup(this);

        Debug.Log(
            "Spawned enemy for Wave " + currWave +
            " | Remaining to spawn: " + enemiesToSpawn.Count
        );
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnLocations == null || spawnLocations.Length <= 0)
            return null;

        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Range(0, spawnLocations.Length);

            if (spawnLocations[randomIndex] != null)
            {
                return spawnLocations[randomIndex];
            }
        }

        return null;
    }

    private void ApplyWaveStatsToEnemy(GameObject enemy, FixedWave waveData)
    {
        if (enemy == null || waveData == null)
            return;

        int finalHP = waveData.enemyHP;
        int finalDamage = waveData.enemyDamage;

        EnemyTank tank = enemy.GetComponent<EnemyTank>();

        if (tank == null)
            tank = enemy.GetComponentInChildren<EnemyTank>();

        if (tank != null)
        {
            finalHP = Mathf.RoundToInt(waveData.enemyHP * tank.hpMultiplier);
            finalDamage = Mathf.RoundToInt(waveData.enemyDamage * tank.damageMultiplier);
        }

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
            enemyHealth = enemy.GetComponentInChildren<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.maxHP = finalHP;
            enemyHealth.currentHP = finalHP;
        }

        EnemyMelle melee = enemy.GetComponent<EnemyMelle>();

        if (melee == null)
            melee = enemy.GetComponentInChildren<EnemyMelle>();

        if (melee != null)
        {
            melee.attackDamage = finalDamage;
        }

        EnemyRanged ranged = enemy.GetComponent<EnemyRanged>();

        if (ranged == null)
            ranged = enemy.GetComponentInChildren<EnemyRanged>();

        if (ranged != null)
        {
            ranged.attackDamage = finalDamage;
        }

        EnemyBomber bomber = enemy.GetComponent<EnemyBomber>();

        if (bomber == null)
            bomber = enemy.GetComponentInChildren<EnemyBomber>();

        if (bomber != null)
        {
            bomber.explosionDamage = finalDamage;
        }

        if (tank != null)
        {
            tank.attackDamage = finalDamage;
        }

        Debug.Log(
            enemy.name +
            " stats applied | HP: " +
            finalHP +
            " | Damage: " +
            finalDamage
        );
    }

    private void FinishCurrentWave()
    {
        Debug.Log("Wave " + currWave + " cleared!");

        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayWaveOver();
        }

        if (currWave >= waves.Count)
        {
            FinishAllWaves();
            return;
        }

        waitingForNextWave = true;
        nextWaveTimer = timeBetweenWaves;

        Debug.Log("Next wave starts in " + timeBetweenWaves + " seconds.");
    }

    private void HandleNextWaveDelay()
    {
        nextWaveTimer -= Time.deltaTime;

        if (nextWaveTimer <= 0f)
        {
            waitingForNextWave = false;
            StartWave(currWave + 1);
        }
    }

    private void FinishAllWaves()
    {
        allWavesCleared = true;
        isSpawningWave = false;
        waitingForNextWave = false;

        Debug.Log("All 10 waves cleared!");

        if (onAllWavesCleared != null)
        {
            onAllWavesCleared.Invoke();
        }
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
        if (enemy == null)
            return;

        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    private void CreateDefaultWavesIfEmpty()
    {
        if (waves != null && waves.Count > 0)
            return;

        waves = new List<FixedWave>
        {
            new FixedWave("Wave 1", 5, 30, 10),
            new FixedWave("Wave 2", 8, 40, 15),
            new FixedWave("Wave 3 - Tier 2 Starts", 10, 60, 25),
            new FixedWave("Wave 4", 12, 80, 30),
            new FixedWave("Wave 5", 15, 100, 60),
            new FixedWave("Wave 6 - Tier 3 Starts", 15, 100, 60),
            new FixedWave("Wave 7", 15, 120, 80),
            new FixedWave("Wave 8", 15, 140, 100),
            new FixedWave("Wave 9 - Pre-Endgame", 15, 165, 120),
            new FixedWave("Wave 10 - Final Wave", 18, 180, 150)
        };
    }
}

[System.Serializable]
public class FixedWave
{
    public string waveName;
    public int enemyQuantity;
    public int enemyHP;
    public int enemyDamage;

    public FixedWave(string waveName, int enemyQuantity, int enemyHP, int enemyDamage)
    {
        this.waveName = waveName;
        this.enemyQuantity = enemyQuantity;
        this.enemyHP = enemyHP;
        this.enemyDamage = enemyDamage;
    }
}

[System.Serializable]
public class EnemySpawnOption
{
    public GameObject enemyPrefab;

    [Header("Wave Availability")]
    public int unlockWave = 1;

    [Header("Spawn Chance Weight")]
    public int spawnWeight = 1;
}