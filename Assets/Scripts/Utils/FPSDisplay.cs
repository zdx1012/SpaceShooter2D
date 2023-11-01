using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float updateInterval = 1f; // 更新间隔（秒）
    private float lastInterval; // 上次更新时间
    private int frames; // 统计帧数
    private float fps; // 当前 FPS

    private GUIStyle style;

    private void Start()
    {
        Application.targetFrameRate = 60;
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperRight;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        frames++;

        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }

    private void OnGUI()
    {
        // 显示 FPS
        string fpsText = $"FPS: {Mathf.RoundToInt(fps)}";

        Rect fpsRect = new Rect(Screen.width - 100, 0, 100, 20);
        GUI.Label(fpsRect, fpsText, style);
    }
}