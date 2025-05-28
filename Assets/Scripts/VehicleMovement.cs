using UnityEngine;
public class VehicleMovement : BaseMovement
{
    public DirectionType startDirection;
    public Sprite[] directionSprites;
    public SpriteRenderer directionSprite;
    protected override void Awake()
    {
        base.Awake();
    }
    public void Start()
    {
        // 初始化车辆时不直接调用Initialize
        InitializeVehicle(startDirection, movementType);
    }
    public void InitializeVehicle(DirectionType directionType, MovementType moveType)
    {
        Debug.Log($"Vehicle initialized with direction: {directionType}, movement type: {moveType}");
        startDirection = directionType;
        movementType = moveType;
        Initialize();
    }
    protected override void Initialize()
    {
        currentTargetIndex = 0;
        isMoving = true;
        pathPoints = VehiclePathPresetSystem.GetPath(startDirection, movementType);
        Debug.Log($"{pathPoints.Count}");
        transform.position = pathPoints[0];
        // 直接设置初始旋转
        Vector3 initialDirection = (pathPoints[1] - pathPoints[0]).normalized;
        transform.rotation = Quaternion.LookRotation(initialDirection);
        // 同步转向状态
        targetRotation = transform.rotation;
        isTurning = false; // 立即完成转向
        transform.localScale = new Vector3(1, 1, 1); // 重置缩放
        ResetEnduranceUI();
        directionSprite.sprite = directionSprites[(int)movementType];
        Color randomColor = new Color(Random.Range(0f, 0.8f),Random.Range(0f, 0.8f),Random.Range(0f, 0.8f));
        if(tag!="policeCar")
            GetComponent<MeshRenderer>().materials[0].SetColor("_Color", randomColor);
        GetComponent<MeshRenderer>().materials[0].SetColor("_Color", randomColor);
        GetComponent<Collider>().enabled = true; // 设置为触发器
        speedMultiplier = 1;
        enduranceTime = 3f;   
    }
}
