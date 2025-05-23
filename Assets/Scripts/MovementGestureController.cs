using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementGestureController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float animationDuration = 0.2f;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    void HandleClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) 
            return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;
        AudioManager.Instance.PlayCarClick();
        var target = hit.collider.GetComponent<BaseMovement>();
        AnimateClick(target.transform);
        if (!target||target.tag=="policeCar") 
            return;
        if(ItemManager.Instance.SpeedUpState)
        {
            ItemManager.Instance.UseItem(ItemType.SpeedUp,target.gameObject);
            ItemManager.Instance.SpeedUpState = false;
        }
        else if(ItemManager.Instance.RemoveCollisionState)
        {
            ItemManager.Instance.UseItem(ItemType.RemoveCollision,target.gameObject);
            ItemManager.Instance.RemoveCollisionState = false;
            AnimateClick(target.transform);
        }
        else
            target.ChangeMoveState();
    }


    IEnumerator ScaleAnimation(Transform target)
    {
        if(target== null) // Check if target is destroyed or null
                yield break;
        Vector3 originalScale = target.localScale;
        float halfDuration = animationDuration / 2f;
        
        // 放大
        yield return ScaleTo(target, originalScale * scaleMultiplier, halfDuration);
        // 缩小
        yield return ScaleTo(target, originalScale, halfDuration);
    }

    IEnumerator ScaleTo(Transform target, Vector3 targetScale, float duration)
    {
        if(target== null) // Check if target is destroyed or null
                yield break;
        float elapsed = 0f;
        Vector3 startScale = target.localScale;
        while (elapsed < duration)
        {
            if(target== null) // Check if target is destroyed or null
                yield break;
            target.localScale = Vector3.Lerp(
                startScale, 
                targetScale, 
                elapsed / duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        if(target== null) // Check if target is destroyed or null
                yield break;
        target.localScale = targetScale;
    }

    void AnimateClick(Transform target) => StartCoroutine(ScaleAnimation(target));
}