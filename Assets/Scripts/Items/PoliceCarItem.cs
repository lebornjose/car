using System;
using UnityEngine;

public class PoliceCarItem : IItem
{
    public ItemType Type => ItemType.PoliceCar;
    public void Use(GameObject target)
    {
        // 从对象池获取车辆
        GameObject car = MyObjectPool.Instance.GetPoolObject(5);
        VehicleMovement vehicle = car.GetComponent<VehicleMovement>();
        // 随机选择方向和移动类型
        DirectionType randomDir = GetRandomEnum<DirectionType>();
        MovementType randomMove = GetRandomEnum<MovementType>();
        Debug.Log(vehicle+" " + randomDir + " " + randomMove);
        vehicle.InitializeVehicle(randomDir, randomMove);
    }
    private T GetRandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}