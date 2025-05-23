using UnityEngine;

public class LightCycle : MonoBehaviour
{
    public Light directionalLight; // 场景中的 Directional Light
    public float xRotation = 30;
    public GameObject[] LuDeng; // 路灯对象数组
    private bool LudengState = false; // 路灯状态
    public float timeSpeed = 0.5f;
    private float updateInterval = 0.1f; // 每秒更新10次 (1 / 10秒)
    private float timer = 0f; // 计时器
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer -= updateInterval; // 重置计时器
            xRotation += updateInterval * timeSpeed; // 更新光源角度
            // 判断是否超过 180°，更新路灯状态
            if (xRotation > 180 && !LudengState)
            {
                LudengState = true; // 更新状态为路灯开启
                UpdateLuDengState(LudengState);
            }
            else if (xRotation <= 180 && LudengState)
            {
                LudengState = false; // 更新状态为路灯关闭
                UpdateLuDengState(LudengState);
            }
            if (xRotation >= 360f)
                xRotation -= 360f;
            directionalLight.transform.rotation = Quaternion.Euler(xRotation, 184f, 0f);
        }
    }
    private void UpdateLuDengState(bool state)
    {
        foreach (var light in LuDeng)
        {
            light.SetActive(state);
        }
    }
}
