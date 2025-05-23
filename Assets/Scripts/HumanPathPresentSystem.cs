using UnityEngine;
using System.Collections.Generic;
public enum DirectionHumanType { NorthL,NorthR, EastL,EastR, SouthL,SourthR,WestL, WestR }

[System.Serializable]
public class HumanPathPreset
{
    public DirectionHumanType startDirection;
    public MovementType movementType;
    // 直行段参数
    public Transform startPoint;
    public Transform endPoint;
    public Transform turnCenter;
}
public class HumanPathPresentSystem : MonoBehaviour
{
    [SerializeField] private List<HumanPathPreset> HumanPaths = new List<HumanPathPreset>();
    private static readonly Dictionary<string, List<Vector3>> pathCache
        = new Dictionary<string, List<Vector3>>();
    void Awake()
    {
        PrecalculateAllPaths();
    }
    void PrecalculateAllPaths()
    {
        foreach (var preset in HumanPaths)
        {
            List<Vector3> pathPoints = new List<Vector3>
            {
                preset.startPoint.position,
                preset.turnCenter.position,
                preset.endPoint.position
            };
            string key = GetPathKey(preset.startDirection, preset.movementType);
            pathCache[key] = pathPoints;
        }
    }
    static string GetPathKey(DirectionHumanType dir, MovementType moveType)
    {
        return $"{dir}_{moveType}";
    }
    public static List<Vector3> GetPath(DirectionHumanType startDir, MovementType moveType)
    {
        string key = GetPathKey(startDir, moveType);
        return pathCache.ContainsKey(key) ? pathCache[key] : null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (HumanPaths == null || HumanPaths.Count == 0)
            return;

        foreach (var preset in HumanPaths)
        {
            // 直线路径
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(preset.startPoint.position, preset.turnCenter.position);
            Gizmos.DrawLine(preset.turnCenter.position, preset.endPoint.position);
        }
    }
#endif
}