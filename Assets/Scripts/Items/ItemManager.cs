using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    private Dictionary<ItemType, IItem> itemDictionary = new Dictionary<ItemType, IItem>();
    public Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();
    public bool RemoveCollisionState = false;
    public bool SpeedUpState = false;
    [SerializeField] private float speedBoostMultiplier;
    [SerializeField] private float boostDuration;

    public void ClearAllState()
    {
        SpeedUpState = false;
        RemoveCollisionState = false;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        initData();
    }
    void initData()
    {
        itemDictionary[ItemType.RemoveCollision] = new RemoveCollisionItem();
        itemDictionary[ItemType.RoadBlock] = new RoadBlockItem();
        var speedUpItem = new SpeedUpItem
        {
            speedBoostMultiplier = speedBoostMultiplier,
            boostDuration = boostDuration
        };
        itemDictionary[ItemType.SpeedUp] = speedUpItem;
        itemDictionary[ItemType.PoliceCar] = new PoliceCarItem();
        itemCounts[ItemType.RemoveCollision] = 0;
        itemCounts[ItemType.RoadBlock] = 0;
        itemCounts[ItemType.SpeedUp] = 0;
        itemCounts[ItemType.PoliceCar] = 0;
    }

    public void UseItem(ItemType type, GameObject target)
    {
        if (!itemCounts.ContainsKey(type) || itemCounts[type] <= 0)
        {
            Debug.LogWarning($"道具 {type} 数量不足！");
            return;
        }

        if (itemDictionary.TryGetValue(type, out var item))
        {
            item.Use(target);
            itemCounts[type]--;
            UIManager.Instance.UpdateUICount();
            Debug.Log($"使用了道具 {type}，剩余：{itemCounts[type]} 个");
            UIManager.Instance.UpdateUICount(); // 自动更新 UI
            UIManager.Instance.ClearSelection();
        }
    }

    public int GetItemCount(ItemType type)
    {
        return itemCounts.TryGetValue(type, out var count) ? count : 0;
    }

    public void AddItem(ItemType type, int amount)
    {
        UIManager.Instance.UpdateUICount();
        if (itemCounts.ContainsKey(type))
            itemCounts[type] += amount;
        else
            itemCounts[type] = amount;
    }
}
