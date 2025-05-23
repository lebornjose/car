using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockItem : IItem
{
    public ItemType Type => ItemType.RoadBlock;
    public void Use(GameObject targetRoad)//暂停场上所有的车辆的行动，并停止出怪5s
    {
        var vehicles = Object.FindObjectsOfType<BaseMovement>();
        foreach (var vehicle in vehicles)
        {
            vehicle.isMoving = false; // 暂停车辆
            vehicle.StartEnduranceTimer();
        }
        var spawner = Object.FindObjectOfType<ObjectGenerator>();
        spawner.stopSpawn(); // 停止生成
    }
}