using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    private float startTime;
    private float endTime;
    public Text text = null;
    public AudioSource audioSource = null;
    public CountDownCallBack countdownFinishedCallback;

    private bool activate = false;

    void Start()
    {
        text = GetComponentInChildren<Text>();
        if (text == null)
        {
            Debug.LogError("没有找到倒计时的显示组件");
        }
        else
        {
            audioSource = text.GetComponentInChildren<AudioSource>();
        }
    }
    void PlaySound()
    {
        if (audioSource != null) { audioSource.Play(); }
    }

    public void StartCountDown(float countDownTime, CountDownCallBack callback)
    {
        startTime = Time.unscaledTime;
        endTime = startTime + countDownTime;
        countdownFinishedCallback = callback;
        activate = true;
    }

    void Update()
    {
        if (!activate) return;

        if (endTime >= Time.unscaledTime)
        {
            float remainingTime = endTime - Time.unscaledTime;
            string tmpText = Mathf.CeilToInt(remainingTime).ToString();
            if (tmpText != text.text)
            {
                text.text = tmpText;
                PlaySound();
            }
        }
        else
        {
            if (countdownFinishedCallback != null)
            {
                countdownFinishedCallback.Invoke();
            }
        }
    }

}