using UnityEngine;
using UnityEngine.Events;

public class PooledObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }

    [Header("Pool Source")]
    [SerializeField] private ObjectPool objectPool;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxActiveObjects = 10;
    [SerializeField] private float spawnInterval = 1f;

    [Header("Spawn Count")]
    [Tooltip("How many spawns are needed to trigger OnSpawnTargetReached.")]
    [SerializeField] private int spawnTargetCount = 10;

    [Header("Events")]
    [Tooltip("Invoked after each successful spawn. Passes total spawned so far.")]
    [SerializeField] private IntUnityEvent OnSpawnCountChanged;
    [Tooltip("Invoked once when TotalSpawned reaches SpawnTargetCount.")]
    [SerializeField] private UnityEvent OnSpawnTargetReached;

    private float spawnTimer;
    private int totalSpawned;
    private bool targetReachedInvoked;

    public int TotalSpawned => totalSpawned;

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
        GameObject spawned = objectPool.GetInstance(spawnPoint);

        // Count only successful activation (pooled object is active in hierarchy)
        if (spawned != null && spawned.activeInHierarchy)
        {
            totalSpawned++;
            OnSpawnCountChanged?.Invoke(totalSpawned);

            if (!targetReachedInvoked && totalSpawned >= spawnTargetCount)
            {
                targetReachedInvoked = true;
                OnSpawnTargetReached?.Invoke();
            }
        }
    }

    /// <summary>
    /// Resets the internal spawn counter and allows the target event to fire again.
    /// </summary>
    public void ResetSpawnCount()
    {
        totalSpawned = 0;
        targetReachedInvoked = false;
        OnSpawnCountChanged?.Invoke(totalSpawned);
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
