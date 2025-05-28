using UnityEngine;
using System.Collections.Generic;
public enum DirectionType { North, East, South, West }
public enum MovementType { Straight, LeftTurn, RightTurn }
[System.Serializable]
public class PathPreset
{
    public DirectionType startDirection;
    public MovementType movementType;
    // 直行段参数
    public Transform startPoint;
    public Transform endPoint;
    public float radius; // 转弯半径
    public Transform turnCenter;     // 转弯圆心
}
public class VehiclePathPresetSystem : MonoBehaviour
{
    [SerializeField] private List<PathPreset> VehiclePaths = new List<PathPreset>();
    [SerializeField] private int curveResolution = 20; // 弧线细分精度
    private static readonly Dictionary<string, List<Vector3>> pathCache = new Dictionary<string, List<Vector3>>();
    void Awake()
    {
        PrecalculateAllPaths();
    }
    void PrecalculateAllPaths()
    {
        foreach (var preset in VehiclePaths)
        {
            List<Vector3> pathPoints = new List<Vector3>();
            if (preset.movementType == MovementType.Straight)
            {
                // 直线路径
                pathPoints.Add(preset.startPoint.position);
                pathPoints.Add(preset.endPoint.position);
            }
            else
            {
                // 转弯路径生成
                pathPoints = CalculateTurnPath(
                    preset.startPoint.position,
                    preset.turnCenter.position,
                    preset.endPoint.position,
                    preset.radius,
                    curveResolution
                );
            }
            string key = GetPathKey(preset.startDirection, preset.movementType);
            pathCache[key] = pathPoints;
        }
    }

    List<Vector3> CalculateTurnPath(Vector3 start, Vector3 mid, Vector3 end, float radius, int resolution)
    {
        List<Vector3> pts = new List<Vector3>();
        // 1. 在平面上计算方向向量（忽略 Y 轴）
        Vector3 v1 = mid - start;
        v1.y = 0; v1.Normalize();
        Vector3 v2 = end - mid;
        v2.y = 0; v2.Normalize();
        // 2. 计算两个直线段与转折圆的切点
        Vector3 turnStart = mid - v1 * radius;
        Vector3 turnEnd   = mid + v2 * radius;
        // 3. 计算圆心：在 turnStart 点沿 v1 法线方向移动 radius
        float crossZ = v1.x * v2.z - v1.z * v2.x;
        // 根据 crossZ 正负决定转弯方向（<0 顺时针，>0 逆时针）
        // 将 v1 旋转 ±90° 得到指向内角区域的法线
        Vector3 perp;
        if (crossZ < 0f)
            perp = new Vector3(v1.z, 0, -v1.x);   // 顺时针内侧
        else
            perp = new Vector3(-v1.z, 0, v1.x);   // 逆时针内侧
        Vector3 center = turnStart + perp * radius;
        // 4. 添加起点直线到切点
        pts.Add(start);
        pts.Add(turnStart);
        // 5. 生成圆弧
        float startAngle = Mathf.Atan2(turnStart.z - center.z, turnStart.x - center.x);
        float endAngle   = Mathf.Atan2(turnEnd.z   - center.z, turnEnd.x   - center.x);
        if (crossZ < 0f) // 顺时针
        {
            if (endAngle > startAngle)
                endAngle -= 2f * Mathf.PI;
        }
        else            // 逆时针
        {
            if (endAngle < startAngle)
                endAngle += 2f * Mathf.PI;
        }
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float ang = Mathf.Lerp(startAngle, endAngle, t);
            float x = Mathf.Cos(ang) * radius + center.x;
            float z = Mathf.Sin(ang) * radius + center.z;
            pts.Add(new Vector3(x, 0f, z));
        }

        // 6. 添加圆弧结束到终点的直线
        pts.Add(turnEnd);
        pts.Add(end);
        return pts;
    }
    static string GetPathKey(DirectionType dir, MovementType moveType)
    {
        return $"{dir}_{moveType}";
    }
    public static List<Vector3> GetPath(DirectionType startDir, MovementType moveType)
    {
        string key = GetPathKey(startDir, moveType);
        return pathCache.ContainsKey(key) ? pathCache[key] : null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (VehiclePaths == null || VehiclePaths.Count == 0)
            return;
        int index = 0;
        foreach (var preset in VehiclePaths)
        {
            // 区分直行和转弯
            if (preset.movementType == MovementType.Straight)
            {
                // 直线路径
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(preset.startPoint.position, preset.endPoint.position);
            }
            else
            {
                // 生成并绘制圆弧
                Gizmos.color = Color.green;
                var curve = CalculateTurnPath(
                    preset.startPoint.position,
                    preset.turnCenter.position,
                    preset.endPoint.position,
                    preset.radius,
                    curveResolution
                );
                for (int i = 0; i < curve.Count - 1; i++)
                {
                    Gizmos.DrawLine(curve[i], curve[i + 1]);
                }
            }
            index++;
        }
    }
#endif
}
