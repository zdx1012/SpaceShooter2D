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
    public int autoSelectPlaneTime = 10;

    public GameObject logo;

    private Text insertCoinTip;
    private Text startGameTip;

    private GameObject videoPlayer;
    private VideoPlayer vp;
    private float pressKeyTime;

    private bool loadGame = false;
    private int currentCoinNum = -1;

    private Text coinNumText;

    private bool showSelectPlane = false;
    public GameObject selectPlaneObject;

    public GameObject[] allPlanes;

    private int currentSelectPlan = 0;

    private RectTransform insertCoinTipTransform;

    // Start is called before the first frame update
    void Start()
    {
        pressKeyTime = Time.time;
        videoPlayer = GameObject.FindGameObjectWithTag("VideoPlay");
        Debug.Assert(videoPlayer != null, "can't find VideoPlay");
        vp = videoPlayer.GetComponent<VideoPlayer>();

        coinNumText = GameObject.Find("CoinNum").GetComponent<Text>();

        insertCoinTipTransform = GameObject.Find("InsertCoin").GetComponent<RectTransform>();

        currentCoinNum = GameData.Instance.GetCurrentGameCoin();
        Debug.Log("start currentCoinNum = " + currentCoinNum);
        if (currentCoinNum <= 0)
        {
            StartCoroutine(ShowInsertCoin());
        }

        Transform transform = GameObject.Find("Canvas").transform.Find("SelectPlan").transform.Find("Planes").transform;
        allPlanes = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            allPlanes[i] = transform.GetChild(i).gameObject;
        }

        // 播放背景声音
        AudioManage.Instance.PlayBgm(AudioManage.Instance.initBgmClip);
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
            coinNumText.text = GameData.Instance.GetShowCoinString();
            showVideoPlayer(false);
            pressKeyTime = Time.time;
        }

        // 跳转设置页面
        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            SceneManager.LoadScene("SettingMain");
        }
        // 隐藏 请投币
        if (GameData.Instance.CanPlayGame())
        {
            insertCoinTipTransform.gameObject.SetActive(false);
            if (InputUtil.instance.IsStartOnceClicked())
            {
                PlayGame();
            }

            if (!loadGame) selectPlaneObject.SetActive(true);
        }
        // 显示飞机选择界面
        else
        {
            selectPlaneObject.SetActive(false);
        }
        // 自动播放视频
        if (Time.time - pressKeyTime > waitVideoTime && !vp.isPlaying)
        {
            showVideoPlayer(true);
        }

        // 处理飞机选择逻辑
        showPlane();
    }



    private void showVideoPlayer(bool show)
    {
        if (LocalConfig.instance.gameConfig.GetBoolDemoVideo() && !showSelectPlane)
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


    // 闪烁 - 请投币
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

    void showPlane()
    {
        // 避免一直切换
        if (Time.time - pressKeyTime < 0.5f || !selectPlaneObject.activeInHierarchy)
        {
            return;
        }
        currentSelectPlan = GameData.Instance.GetCurrentPlane();
        // 显示飞机
        for (int i = 0; i < allPlanes.Length; i++)
        {
            if (i == currentSelectPlan)
            {
                allPlanes[i].SetActive(true);
            }
            else
            {
                allPlanes[i].SetActive(false);
            }
        }

        if (InputUtil.instance.GetHorizontalAxis() > 0)
        {
            pressKeyTime = Time.time;
            currentSelectPlan += 1;
            if (currentSelectPlan >= allPlanes.Length) { currentSelectPlan = 0; }
        }
        else if (InputUtil.instance.GetHorizontalAxis() < 0)
        {
            pressKeyTime = Time.time;
            currentSelectPlan -= 1;
            if (currentSelectPlan < 0) { currentSelectPlan = allPlanes.Length - 1; }
        }
        GameData.Instance.SetCurrentPlane(currentSelectPlan);

        if (Time.time - pressKeyTime > autoSelectPlaneTime)
        {
            PlayGame();
        }
    }

    void PlayGame()
    {
        loadGame = true;
        GameData.Instance.ReduceGameCoin();
        logo.GetComponent<ObjectMovement>().StartMovement();
        StartCoroutine(startGame());
    }

    IEnumerator startGame()
    {
        selectPlaneObject.SetActive(value: false);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }
}
