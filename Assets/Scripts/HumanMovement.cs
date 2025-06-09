using UnityEngine;
public class HumanMovement : BaseMovement
{
    public DirectionHumanType startDirection;
    private Animator Anima;
    public GameObject[] humanModels;
    protected override void Awake()
    {
        base.Awake();
        Anima = GetComponent<Animator>();
    }
    
    public void Start()
    {
        // 初始化车辆时不直接调用Initialize
       InitializeHuman(startDirection, MovementType.Straight);
    }
    public override void ChangeMoveState()
    {
        isMoving = !isMoving;
        if (isMoving)
            Anima.SetBool("stand", false);
        else
            Anima.SetBool("stand", true);
        if (isMoving == false)
            StartEnduranceTimer();
        else
            ResetEnduranceUI();
    }
    public void InitializeHuman(DirectionHumanType directionType, MovementType moveType)
    {
        startDirection = directionType;
        movementType = moveType;
        Initialize();
    }

    protected override void Initialize()
    {
        currentTargetIndex = 0;
        isMoving = true;
        pathPoints = HumanPathPresentSystem.GetPath(startDirection, movementType);
        transform.position = pathPoints[0];
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // 重置缩放
        ResetEnduranceUI();
        int randModel = Random.Range(0, humanModels.Length);
        for (int i = 0; i < humanModels.Length; i++)
            humanModels[i].SetActive(i == randModel);
        GetComponent<Collider>().enabled = true;
        speedMultiplier = 1;
        enduranceTime = 3f;   
    }
}
