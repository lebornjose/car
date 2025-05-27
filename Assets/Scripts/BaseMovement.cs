using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
public abstract class BaseMovement : MonoBehaviour
{
    public MovementType movementType;    // 运动类型枚举（具体类型需在子类中实现）
    protected Vector3 moveDirection;    // 移动方向向量（归一化值）
    public float moveSpeed = 5f;        // 移动速度
    public float rotateSpeed = 360f;    // 旋转速度（度/秒）
    public List<Vector3> pathPoints;          // 路径点集合
    protected int currentTargetIndex = 0;     // 当前目标点索引
    protected bool isTurning = false;         // 转向状态标记
    protected Quaternion targetRotation;      // 目标朝向四元数
    public bool isMoving = false;          // 移动状态标记
    protected Coroutine enduranceCoroutine;
    public float enduranceTime = 3f;
    public bool IsBoosted { get; protected set; }
    public float speedMultiplier = 1f;
    [SerializeField] protected Vector3 enduranceTextOffset;
    
    protected TextMeshPro txtEnduranceTime;
    protected virtual void Awake()
    {
        txtEnduranceTime = GetComponentInChildren<TextMeshPro>();
    }
    public virtual void StartEnduranceTimer()
    {
        if (!this.isActiveAndEnabled) return;
        if (enduranceCoroutine != null)
            StopCoroutine(enduranceCoroutine);
        enduranceCoroutine = StartCoroutine(EnduranceCountdown());
    }
    protected virtual void ResetEnduranceUI()
    {
        if (enduranceCoroutine != null)
            StopCoroutine(enduranceCoroutine);
        txtEnduranceTime.text = "";
        txtEnduranceTime.transform.localScale = Vector3.one;
        txtEnduranceTime.transform.localPosition = enduranceTextOffset;
    }
    protected virtual IEnumerator EnduranceCountdown()
    {
        float startTime = Time.time;
        bool warningTriggered = false;
        Coroutine scaleCoroutine = null;
        while (Time.time - startTime < enduranceTime)
        {
            float remainingTime = enduranceTime - (Time.time - startTime);
            if (remainingTime <= 1f && !warningTriggered)
            {
                warningTriggered = true;
                txtEnduranceTime.text = "!";
                scaleCoroutine = StartCoroutine(WarningScaleAnimation());
            }
            else if (remainingTime > 1&&remainingTime<=3)
                txtEnduranceTime.text = Mathf.CeilToInt(remainingTime).ToString();
            yield return null;
        }
        if (scaleCoroutine != null) 
            StopCoroutine(scaleCoroutine);
        isMoving = true;
        ResetEnduranceUI();
    }

    protected virtual IEnumerator WarningScaleAnimation()
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 maxScale = Vector3.one * 1.5f;
        txtEnduranceTime.color = Color.red;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            txtEnduranceTime.transform.localScale = Vector3.Lerp(
                Vector3.one,
                maxScale,
                Mathf.PingPong(t * 2f, 1f)
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        txtEnduranceTime.color = Color.white;
    }
    public virtual void ChangeMoveState()
    {
        isMoving = !isMoving;
        if(isMoving==false)
            StartEnduranceTimer();
        else
            ResetEnduranceUI();
    }
    protected virtual void StartTurn(Vector3 targetPoint)
    {
        isTurning = true;
        Vector3 targetDirection = (targetPoint - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(targetDirection);//计算目标朝向
    }
    protected virtual void UpdateMovement()
    {
        if (!isMoving) return;
        if (currentTargetIndex < pathPoints.Count)
        {
            Vector3 targetPoint = pathPoints[currentTargetIndex];
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)//到达当前路径点
            {
                currentTargetIndex++;
                if (currentTargetIndex < pathPoints.Count)//切换到下个路径点
                {
                    targetPoint = pathPoints[currentTargetIndex];
                    moveDirection = (targetPoint - transform.position).normalized;
                    StartTurn(targetPoint);
                }
            }
            else
            {
                transform.position += moveDirection * moveSpeed * Time.deltaTime * speedMultiplier;
                if (isTurning)//平滑的转向
                {
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
                    if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                        isTurning = false;
                }
            }
        }
        else
        {
            MyObjectPool.ReleaseGameObject(this.gameObject);
            // GameManager.Instance.UpdateScore(1);
            // UIManager.Instance.UpdateUICount();
            // AudioManager.Instance.PlayCarWin(); 
            isMoving = false;
        }
    }
    protected abstract void Initialize();
    void Update() => UpdateMovement();

    public virtual void ApplySpeedBoost(float multiplier, float duration)
    {
        if (!IsBoosted)
            StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    protected virtual IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        IsBoosted = true;
        speedMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1;
        IsBoosted = false;
    }
}
