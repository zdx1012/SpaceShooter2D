using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Assets.Scripts.GamePlay;
using DG.Tweening;

public class Init : MonoBehaviour
{
    [Header("长时间未操作，自动播放视频（S）")]
    public int waitVideoTime = 10;

    private Text insertCoinTip;
    private Text startGameTip;

    private GameObject videoPlayer;
    private VideoPlayer vp;
    private float pressKeyTime;

    private int currentCoinNum = -1;

    private int startGameCoin = 1;
    private Text coinNumText;

    private RectTransform insertCoinTipTransform;

    // Start is called before the first frame update
    void Start()
    {
        pressKeyTime = Time.time;
        videoPlayer = GameObject.FindGameObjectWithTag("VideoPlay");
        Debug.Assert(videoPlayer != null, "can't find VideoPlay");
        vp = videoPlayer.GetComponent<VideoPlayer>();

        coinNumText = GameObject.Find("CoinNum").GetComponent<Text>();

        startGameCoin = LocalConfig.instance.gameConfig.GetValueGame();

        insertCoinTipTransform = GameObject.Find("InsertCoin").GetComponent<RectTransform>();

        currentCoinNum = GameData.Instance.GetCurrentGameCoin();
        if (currentCoinNum <= 0)
        {
            StartCoroutine(ShowInsertCoin());
        }

        // 设置声音等信息
        SetParams();
    }

    public void LogErrorFromAndroid(string errorMessage)
    {
        // Debug.LogError("Android Error: " + errorMessage);
    }

    // Update is called once per frame
    void Update()
    {
        // 获取币数
        currentCoinNum = GameData.Instance.GetCurrentGameCoin();
        if (GameData.Instance.GetShowCoinString() != coinNumText.text.ToString())
        {
            Debug.Log("update coin num to: " + currentCoinNum);
            coinNumText.text = GameData.Instance.GetShowCoinString();
            showVideoPlayer(false);
            pressKeyTime = Time.time;
        }


        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            SceneManager.LoadScene("SettingMain");
        }

        if (GameData.Instance.CanPlayGame())
        {
            insertCoinTipTransform.gameObject.SetActive(false);
            if (InputUtil.instance.IsStartOnceClicked())
            {
                GameData.Instance.ReduceGameCoin();
                SceneManager.LoadScene(1);
            }
        }

        if (InputUtil.instance.AnyAxisPressed())
        {
            pressKeyTime = Time.time;
            showVideoPlayer(false);
        }
        else if (Time.time - pressKeyTime > waitVideoTime && !vp.isPlaying)
        {
            showVideoPlayer(true);
        }
    }



    private void showVideoPlayer(bool show)
    {
        if (LocalConfig.instance.gameConfig.GetBoolDemoVideo())
        {
            if (show)
            {
                videoPlayer.gameObject.SetActive(true);

                if (!vp.isPlaying) vp.Play();
            }
            else
            {
                if (vp.isPlaying) vp.Pause();
                videoPlayer.gameObject.SetActive(false);
            }
        }
    }

    private void setVolumeSilent()
    {
        vp.audioOutputMode = VideoAudioOutputMode.None;
    }

    IEnumerator ShowInsertCoin()
    {
        DOTween.Sequence()
            .Append(insertCoinTipTransform.DOScale(new Vector3(0.9f, 0.9f, 1f), 1f)) // 缩小到一半大小，持续1秒
            .AppendInterval(0.1f) // 等待0.5秒
            .Append(insertCoinTipTransform.DOScale(Vector3.one, 1f)) // 恢复原始大小，持续1秒)
            .SetLoops(-1) // 设置循环，-1表示无限循环
            .Play();
        yield return null;
    }

    void SetParams()
    {
        float globalVolume = LocalConfig.instance.gameConfig.GetVolume() / 10.0f; // 1.0表示全音量，0.0表示静音
        // AudioListener audioListener = Camera.main.GetComponent<AudioListener>();
        globalVolume = Mathf.Clamp01(globalVolume); // 限制在0到1之间
        AudioListener.volume = globalVolume;
    }
}
