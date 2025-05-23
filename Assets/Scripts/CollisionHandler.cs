using UnityEngine;
[RequireComponent(typeof(Collider))]
public class CollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "human" && tag == "human")
            return;
        if(tag=="policeCar")
        {
            GameManager.Instance.UpdateHealth(1.5f);//每次碰撞-0.5f
            return;
        }
        GameManager.Instance.UpdateHealth(-0.5f);//每次碰撞-0.5f
        MyObjectPool.ReleaseGameObject(gameObject); // 释放到对象池
        AudioManager.Instance.PlayCarBoom();
    }
}