using UnityEngine;

public class PooledObjectSpawner : MonoBehaviour
{
    [Header("Pool Source")]
    [SerializeField] private ObjectPool objectPool;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxActiveObjects = 10;
    [SerializeField] private float spawnInterval = 1f;

    private float spawnTimer;

    private void Start()
    {
        objectPool.Init();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        int activeCount = GetActiveCount();

        if (activeCount >= maxActiveObjects)
            return;

        Transform spawnPoint = GetRandomSpawnPoint();
        objectPool.GetInstance(spawnPoint);
    }

    private int GetActiveCount()
    {
        int count = 0;

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.activeInHierarchy && obj.name.Contains(objectPool.prefabe.name))
            {
                count++;
            }
        }

        return count;
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return this.transform;

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
