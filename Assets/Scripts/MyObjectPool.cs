using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MyObjectPool : MonoBehaviour
{
    // 单例实例
    public static MyObjectPool Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            InitializePools();
        }
    }
    
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
    }
    [SerializeField] private PoolConfig[] poolConfigs;
    private List<IObjectPool<GameObject>> pools;

    void InitializePools()
    {
        pools = new List<IObjectPool<GameObject>>();
        foreach (var config in poolConfigs)
        {
            IObjectPool<GameObject> tempPool = null;
            tempPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(config.prefab, transform);
                    obj.AddComponent<PooledObject>().Pool = tempPool;
                    return obj;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Destroy,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );
            pools.Add(tempPool);
        }
    }
    public void ClearAll()
    {
        foreach (var pool in pools)
        {
            pool.Clear();
        }
        InitializePools();
    }
    public GameObject GetPoolObject(int typeIndex)
    {
        if(typeIndex < 0 || typeIndex >= pools.Count)
        {
            Debug.LogError($"无效的索引: {typeIndex}");
            return null;
        }
        return pools[typeIndex].Get();
    }
    public static void ReleaseGameObject(GameObject GetPoolObject)
    {
        var pooledObj = GetPoolObject.GetComponent<PooledObject>();
        if (pooledObj != null && pooledObj.Pool != null&&pooledObj.isActiveAndEnabled)
        {
            pooledObj.Pool.Release(GetPoolObject);
        }
        else
        {
            Destroy(GetPoolObject);
        }
    }
}
// 池对象标记组件
public class PooledObject : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }
}