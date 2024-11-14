using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Blink : MonoBehaviour
{
    public Text textComponent;
    public float blinkSpeed = 1.0f;  // 控制闪烁速度
    public float maxAlpha = 1.0f;     // 文字最大透明度
    public float minAlpha = 0.2f;     // 文字最小透明度

    private void OnEnable()
    {
        StartBlinking();
    }

    private void OnDisable()
    {
        StopBlinking();
    }

    private void StartBlinking()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<Text>();
        }

        // 启动协程来处理闪烁
        StartCoroutine(BlinkText());
    }

    private void StopBlinking()
    {
        // StopAllCoroutines();
        StopCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            // 在透明度最大和最小之间进行插值，创建闪烁效果
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, maxAlpha - minAlpha) + minAlpha;

            // 更新文本透明度
            Color textColor = textComponent.color;
            textColor.a = alpha;
            textComponent.color = textColor;

            yield return null;
        }
    }
}
