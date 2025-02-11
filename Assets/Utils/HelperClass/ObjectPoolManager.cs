using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ObjectPoolManager : SingletonMonoBehavior<ObjectPoolManager>
{
    [System.Serializable]
    public class PoolInfo
    {
        public GameObject prefab;
        public int initialSize;
        public bool shouldExpand = true;
        public int maxSize = 100;
        [HideInInspector] public Transform poolParent;
        [HideInInspector] public Queue<GameObject> pool;
        [HideInInspector] public List<GameObject> activeObjects;
    }

    [SerializeField] private List<PoolInfo> poolsToPreload;
    private Dictionary<string, PoolInfo> pools = new Dictionary<string, PoolInfo>();

    protected override void InitializeSingleton()
    {
        foreach (var poolInfo in poolsToPreload)
        {
            CreatePool(poolInfo.prefab, poolInfo.initialSize, poolInfo.shouldExpand, poolInfo.maxSize);
        }
    }

    public void CreatePool(GameObject prefab, int initialSize = 10, bool shouldExpand = true, int maxSize = 100)
    {
        string poolKey = prefab.name;

        if (pools.ContainsKey(poolKey))
        {
            Debug.LogWarning($"Pool for prefab {poolKey} already exists!");
            return;
        }

        PoolInfo poolInfo = new PoolInfo
        {
            prefab = prefab,
            initialSize = initialSize,
            shouldExpand = shouldExpand,
            maxSize = maxSize,
            pool = new Queue<GameObject>(),
            activeObjects = new List<GameObject>(),
            poolParent = new GameObject($"Pool_{poolKey}").transform
        };

        poolInfo.poolParent.SetParent(transform);
        pools[poolKey] = poolInfo;

        // Create initial pool
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewInstance(poolInfo);
        }
    }

    private GameObject CreateNewInstance(PoolInfo poolInfo)
    {
        GameObject obj = Instantiate(poolInfo.prefab, poolInfo.poolParent);
        obj.SetActive(false);
        poolInfo.pool.Enqueue(obj);
        return obj;
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string poolKey = prefab.name;

        if (!pools.ContainsKey(poolKey))
        {
            Debug.LogWarning($"Pool for prefab {poolKey} doesn't exist. Creating new pool.");
            CreatePool(prefab);
        }

        PoolInfo poolInfo = pools[poolKey];
        GameObject obj = null;

        if (poolInfo.pool.Count == 0)
        {
            if (poolInfo.shouldExpand && (poolInfo.activeObjects.Count < poolInfo.maxSize))
            {
                obj = CreateNewInstance(poolInfo);
            }
            else
            {
                Debug.LogWarning($"Pool for {poolKey} is empty and cannot expand!");
                return null;
            }
        }

        obj = poolInfo.pool.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        poolInfo.activeObjects.Add(obj);

        // Call OnSpawn if the object implements IPoolable
        if (obj.TryGetComponent(out IPoolable poolable))
        {
            poolable.OnSpawn();
        }

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        string poolKey = obj.name.Replace("(Clone)", "").Trim();

        if (!pools.ContainsKey(poolKey))
        {
            Debug.LogError($"Trying to return object to non-existent pool: {poolKey}");
            return;
        }

        PoolInfo poolInfo = pools[poolKey];

        // Call OnDespawn if the object implements IPoolable
        if (obj.TryGetComponent(out IPoolable poolable))
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        poolInfo.pool.Enqueue(obj);
        poolInfo.activeObjects.Remove(obj);
    }

    public void ReturnAllToPool(string poolKey)
    {
        if (!pools.ContainsKey(poolKey))
        {
            Debug.LogError($"Pool doesn't exist: {poolKey}");
            return;
        }

        PoolInfo poolInfo = pools[poolKey];
        foreach (GameObject obj in poolInfo.activeObjects.ToArray())
        {
            ReturnToPool(obj);
        }
    }

    public void ReturnAllToPools()
    {
        foreach (var pool in pools.Values)
        {
            foreach (GameObject obj in pool.activeObjects.ToArray())
            {
                ReturnToPool(obj);
            }
        }
    }

    public bool HasPool(string poolKey)
    {
        return pools.ContainsKey(poolKey);
    }

    public int GetActiveCount(string poolKey)
    {
        return pools.TryGetValue(poolKey, out var pool) ? pool.activeObjects.Count : 0;
    }

    public int GetPooledCount(string poolKey)
    {
        return pools.TryGetValue(poolKey, out var pool) ? pool.pool.Count : 0;
    }
}