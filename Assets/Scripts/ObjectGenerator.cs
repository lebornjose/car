using UnityEngine;
using System.Collections;
using System;

public class ObjectGenerator : MonoBehaviour
{
    public static ObjectGenerator Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] public float initialSpawnInterval = 1f;    // 初始生成间隔
    [SerializeField] private float minSpawnInterval = 0.2f;      // 最小生成间隔
    [SerializeField] private float difficultyIncreaseInterval = 5f; // 难度增加间隔
    [SerializeField][Range(0.1f, 0.99f)] private float spawnIntervalMultiplier = 0.9f; // 间隔缩减系数
    public float currentSpawnInterval;
    private float difficultyTimer;
    public bool ifStopSpawn = false; // 是否停止生成
    public int MaxCarType = 1;
    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        // StartCoroutine(SpawnRoutine());
    }
    private void Update()
    {
        if(GameManager.Instance.ifGameStart==false)
            return;
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            difficultyTimer = 0f;
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval, 
                currentSpawnInterval * spawnIntervalMultiplier
            );
        }
    }
   public void stopSpawn()
    {
        // 添加协程停止逻辑
        if (ifStopSpawn==false) 
            StartCoroutine(stopSpawnCoroutine());
    }
    IEnumerator stopSpawnCoroutine()
    {
        ifStopSpawn = true;
        yield return new WaitForSeconds(5f);
        ifStopSpawn = false;
    }
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (ifStopSpawn)
            {
                yield return null;
                continue;
            };
            GenerateCar();
            if(MaxCarType>=2)
            {
                if(UnityEngine.Random.Range(0,8)==0)
                    GenerateHuman();
            }
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
    private void GenerateHuman()
    {
        GameObject human = MyObjectPool.Instance.GetPoolObject(0);
        HumanMovement humanMove = human.GetComponent<HumanMovement>();
        // 随机选择方向和移动类型
        DirectionHumanType randomDir = GetRandomEnum<DirectionHumanType>();
        MovementType randomMove = GetRandomEnum<MovementType>();
        humanMove.InitializeHuman(randomDir, randomMove);
    }
    private void GenerateCar()
    {
        int carType = UnityEngine.Random.Range(1, MaxCarType); // 随机选择车的类型
        if(carType == 4)
            carType = UnityEngine.Random.Range(1, MaxCarType);
        GameObject car = MyObjectPool.Instance.GetPoolObject(carType);
        VehicleMovement vehicle = car.GetComponent<VehicleMovement>();
        // 随机选择方向和移动类型
        DirectionType randomDir = GetRandomEnum<DirectionType>();
        MovementType randomMove = GetRandomEnum<MovementType>();
        vehicle.InitializeVehicle(randomDir, randomMove);
    }

    // 通用枚举随机获取方法
    private T GetRandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    public void StartSpwawn()
    {
        ifStopSpawn = false;
        currentSpawnInterval = initialSpawnInterval; // 重置生成间隔
        StartCoroutine(SpawnRoutine()); // 重新启动生成协程
        MaxCarType = 1;
    }
}