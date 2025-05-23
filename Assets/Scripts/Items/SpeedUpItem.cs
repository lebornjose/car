using UnityEngine;

public class SpeedUpItem : IItem
{
    public ItemType Type => ItemType.SpeedUp;
    public float speedBoostMultiplier;
    public float boostDuration;
    public void Use(GameObject target)
    {
        if(ItemManager.Instance.SpeedUpState==true)
        {
            BaseMovement baseMove = target.GetComponent<BaseMovement>();
            baseMove.isMoving = true;
            baseMove.ApplySpeedBoost(speedBoostMultiplier,boostDuration);
            ItemManager.Instance.SpeedUpState = false;
        }
    }
}
