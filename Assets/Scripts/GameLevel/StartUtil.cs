using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.IO;
using System;
using Assets.Scripts.GamePlay;

public class StartUtil : MonoBehaviour
{
    [Header("隐藏未开发的关卡显示")]
    public bool HiddenOtherPart = true;

    [Header("长时间未操作，自动播放视频（S）")]
    public int waitVideoTime = 10;

    [Header("关卡选中时的放大倍数")]
    public float scaleMultiplier = 1.2f; // 放大倍数

    [Header("声音控制的图标")]
    public Sprite soundSprite;

    [Header("静音控制的图标")]
    public Sprite silentSprite;

    private Button[] btnList;

    // 可点击的对象
    private List<GameObject> canClickGameObject = new List<GameObject>();
    private int currentActivateIndex = 0;

    // 默认图标的缩放倍数
    private Vector3 btnOriginalScale;

    private Vector2 _movement;
    private int gamePartNum = 0;
    // private int curSelectBtnIndex = 0;
    private GameObject gameLevel;
    private GameObject videoPlayer;
    private VideoPlayer vp;
    private Button volumeButton;
    private bool hasVolume = true;

    private float pressKeyTime;

    private int currentCoinNum = -1;
    private Text coinNumText;

    // Start is called before the first frame update
    void Start()
    {
        pressKeyTime = Time.time;
        videoPlayer = GameObject.FindGameObjectWithTag("VideoPlay");
        Debug.Assert(videoPlayer != null, "can't find VideoPlay");
        vp = videoPlayer.GetComponent<VideoPlayer>();


        gameLevel = GameObject.Find("GameLevels");
        Debug.Assert(gameLevel != null, "can't find gameLevels");

        volumeButton = GameObject.Find("Volume").GetComponent<Button>();
        Debug.Assert(volumeButton != null, "can't find volumeButton");
        volumeButton.onClick.AddListener(setVolumeStatus);


        btnList = GameObject.FindGameObjectsWithTag("ListButton")
       .Select(go => go.GetComponent<Button>())
       .OrderBy(btn => btn.name)
       .ToArray();

        Debug.Assert(volumeButton.gameObject != null, "can't find volumeButton.gameObject");
        canClickGameObject.Add(volumeButton.gameObject);

        gamePartNum = LocalConfig.instance.gameConfig.data.Length;

        for (int i = 0; i < btnList.Length; i++)
        {
            int index = i; // 创建一个局部变量来捕获循环变量的值
            btnList[index].onClick.AddListener(() => OnClick(btnList[index], index));
            canClickGameObject.Add(btnList[i].gameObject);
        }
        btnOriginalScale = btnList[0].transform.localScale;

        coinNumText = GameObject.Find("CoinNum").GetComponent<Text>();

        // 读取配置显示图标《静音状态》
        if(!LocalConfig.instance.gameConfig.playVideoSound) setVolumeStatus();
        
    }

    public void LogErrorFromAndroid(string errorMessage)
    {
        // Debug.LogError("Android Error: " + errorMessage);
    }

    // Update is called once per frame
    void Update()
    {
        // 获取币数
        currentCoinNum = InputUtil.instance.GetCoinNum();
        if (currentCoinNum.ToString() != coinNumText.text.ToString())
        {
            Debug.Log("update coin num to: " + currentCoinNum);
            coinNumText.text = currentCoinNum.ToString();
        }

        if (InputUtil.instance.IsSettingKeyHold(10))
        {
            SceneManager.LoadScene("Set");
        }

        if (InputUtil.instance.IsStartOnceClicked())
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }

        if (InputUtil.instance.AnyAxisPressed())
        {
            pressKeyTime = Time.time;
            showVideoPlayer(false);
            setBtnStatus();
        }
        else if (Time.time - pressKeyTime > waitVideoTime && !vp.isPlaying)
        {
            showVideoPlayer(true);
        }
    }

    void setBtnStatus()
    {
        currentActivateIndex += 1;
        currentActivateIndex = currentActivateIndex % canClickGameObject.Count;
        for (int i = 0; i < canClickGameObject.Count; i++)
        {
            if (i == currentActivateIndex)
            {
                canClickGameObject[i].transform.localScale = btnOriginalScale * scaleMultiplier;
            }
            else
            {
                canClickGameObject[i].transform.localScale = btnOriginalScale;
            }
        }
        EventSystem.current.SetSelectedGameObject(canClickGameObject[currentActivateIndex]);

    }
    void OnClick(Button clickedObject, int index)
    {
        // 第一个场景为选择关卡，后续的场景为关卡
        if (index + 1 <= gamePartNum)
        {
            Debug.Log("Button clicked: " + clickedObject.name + " index = " + index + " gamePartNum = " + gamePartNum);
            Game.Current.currentGameLevel = index;
            Game.Current.totalGameLevel = gamePartNum;
            SceneManager.LoadScene(index + 1);
        }
    }


    private void showVideoPlayer(bool show)
    {
        if (show)
        {
            gameLevel.SetActive(false);
            videoPlayer.gameObject.SetActive(true);
            
            if (!vp.isPlaying) vp.Play();
        }
        else
        {
            if (vp.isPlaying) vp.Pause();
            gameLevel.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
        }
    }

    private void setVolumeStatus()
    {
        hasVolume = !hasVolume;
        if (hasVolume)
        {
            volumeButton.GetComponent<Image>().sprite = soundSprite;
            volumeButton.GetComponent<AudioSource>().volume = 1;
            vp.audioOutputMode = VideoAudioOutputMode.Direct;
        }
        else
        {
            volumeButton.GetComponent<Image>().sprite = silentSprite;
            volumeButton.GetComponent<AudioSource>().volume = 0;
            vp.audioOutputMode = VideoAudioOutputMode.None;
        }
    }
}
