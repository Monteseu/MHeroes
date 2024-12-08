using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Spawns enemies in the game world using a dynamic pool.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private int worldIndex = 0;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private float minSpawnDistanceFromPlayer = 10f;
    [SerializeField] private float minDistanceBetweenEnemies = 5f;
    [SerializeField] private float respawnDelay = 2f;

    [Header("Map Settings")]
    [SerializeField] private Transform mapCenter;
    [SerializeField] private float spawningArea = 50f;

    private List<EnemyModelData> currentWorldEnemies;
    private DynamicPool<EnemyController, EnemyModelData> enemyPool;

    public void Initialize()
    {
        InitializeEnemyPool();
    }

    private void InitializeEnemyPool()
    {
        // Retrieve the list of enemy models for the current world index.
        currentWorldEnemies = ServicesManager.Get()
            .GetService<ModelDataService>()
            .GetLibrary<EnemiesModelLibrary>()
            .GetEnemiesByWorldIndex(worldIndex);

        // Initialize the dynamic pool with the appropriate functions.
        enemyPool = new DynamicPool<EnemyController, EnemyModelData>(
            CreateEnemyAsync,
            OnEnemyInstantiated,
            OnEnemyRelease);

        // Start spawning initial enemies.
        StartCoroutine(SpawnInitialEnemies());
    }

    private AsyncOperationHandle<GameObject> CreateEnemyAsync(EnemyModelData enemyData) => Addressables.InstantiateAsync(enemyData.prefabReference);

    private IEnumerator SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            EnemyModelData enemyData = GetRandomEnemyData();
            yield return enemyPool.GetOrCreate(enemyData);
        }
    }

    private IEnumerator SpawnEnemy()
    {
        EnemyModelData enemyData = GetRandomEnemyData();
        yield return enemyPool.GetOrCreate(enemyData);
    }

    private EnemyModelData GetRandomEnemyData()
    {
        // Select a random EnemyModelData from the list.
        int index = Random.Range(0, currentWorldEnemies.Count);
        return currentWorldEnemies[index];
    }

    private void OnEnemyInstantiated(EnemyController enemy, EnemyModelData enemyData)
    {
        if (enemy == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        enemy.transform.position = spawnPosition;
        enemy.SetupData(enemyData);
        enemy.OnEnemyDefeated += HandleEnemyDefeated;
        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy();
    }

    private void OnEnemyRelease(EnemyController enemy)
    {
        if (enemy == null) return;

        enemy.OnEnemyDefeated -= HandleEnemyDefeated;
        enemy.gameObject.SetActive(false);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        int attempts = 0;
        const int maxAttempts = 30;
        Vector3 spawnPosition = Vector3.zero;

        while (attempts < maxAttempts)
        {
            Vector3 randomPosition = GetRandomPositionWithinMap();
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                if (IsValidSpawnPosition(spawnPosition))
                {
                    return spawnPosition;
                }
            }
            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn position.");
        return spawnPosition;
    }

    private Vector3 GetRandomPositionWithinMap()
    {
        float halfSize = spawningArea / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        return mapCenter.position + new Vector3(x, 0, z);
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Avoid spawning too close to the map center.
        if (Vector3.Distance(position, mapCenter.position) < minSpawnDistanceFromPlayer)
            return false;

        // Ensure the spawn position is not too close to other active enemies.
        foreach (var enemy in enemyPool.ActiveObjects)
        {
            if (Vector3.Distance(position, enemy.transform.position) < minDistanceBetweenEnemies)
                return false;
        }

        return true;
    }

    private void HandleEnemyDefeated(EnemyController enemy)
    {
        if (enemy == null) return;
        StartCoroutine(RespawnEnemy(enemy, respawnDelay));
    }

    private IEnumerator RespawnEnemy(EnemyController enemy, float delay)
    {
        enemyPool.Release(enemy);
        yield return new WaitForSeconds(delay);

        yield return SpawnEnemy();
    }

    private void OnDestroy()
    {
        if (enemyPool != null)
            enemyPool.Dispose();
    }
}
