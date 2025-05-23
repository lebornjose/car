using UnityEngine;

public class RemoveCollisionItem : IItem
{
    public ItemType Type => ItemType.RemoveCollision;
    public void Use(GameObject target)
    {
        var collider = target.GetComponent<Collider>();
        collider.enabled = false;
        target.GetComponent<BaseMovement>().isMoving=true;
        ItemManager.Instance.RemoveCollisionState = false;
    }
}