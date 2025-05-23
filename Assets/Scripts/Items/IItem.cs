
using UnityEngine;
public enum ItemType
{
    RemoveCollision,   // 消除碰撞
    RoadBlock,         // 封路卡
    SpeedUp,           // 加速
    PoliceCar,       // 警车
}
public interface IItem
{
    ItemType Type { get; }
    void Use(GameObject target);
}