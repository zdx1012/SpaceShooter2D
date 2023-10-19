using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

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

    private Vector3 btnOriginalScale;

    private float lastModifyTime = 0;

    private Vector2 _movement;
    private int gamePartNum = 0;
    private int curSelectBtnIndex = 0;
    private GameObject gameLevel;
    private GameObject videoPlayer;
    private VideoPlayer vp;
    private Button volumeButton;
    private bool hasVolume = true;

    private float pressKeyTime;
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

        btnList = GameObject.FindGameObjectsWithTag("ListButton").Select(go => go.GetComponent<Button>()).ToArray();


        btnList = GameObject.FindGameObjectsWithTag("ListButton")
       .Select(go => go.GetComponent<Button>())
       .OrderBy(btn => btn.name)
       .ToArray();

        gamePartNum = SceneManager.sceneCountInBuildSettings - 1;

        for (int i = 0; i < btnList.Length; i++)
        {
            int index = i; // 创建一个局部变量来捕获循环变量的值
            btnList[index].onClick.AddListener(() => OnClick(btnList[index], index));
            if (index + 1 > gamePartNum)
            {
                btnList[i].interactable = false;
                if (HiddenOtherPart) btnList[index].gameObject.SetActive(false);
            }
        }
        btnOriginalScale = btnList[0].transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
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
        // 如果没有任何可交互组件选中，设置默认选中音量
        if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(volumeButton.gameObject);
        EventSystem.current.currentSelectedGameObject.gameObject.transform.localScale = btnOriginalScale * scaleMultiplier;
        for (int i = 0; i < gamePartNum; i++)
        {
            btnList[i].transform.localScale = (EventSystem.current.currentSelectedGameObject == btnList[i].gameObject ? btnOriginalScale * scaleMultiplier : btnOriginalScale);
        }
        volumeButton.gameObject.transform.localScale = EventSystem.current.currentSelectedGameObject == volumeButton.gameObject ? btnOriginalScale * scaleMultiplier : btnOriginalScale;
    }
    void OnClick(Button clickedObject, int index)
    {
        // 第一个场景为选择关卡，后续的场景为关卡
        if (index + 1 <= gamePartNum)
        {
            Debug.Log("Button clicked: " + clickedObject.name + " index = " + index + " gamePartNum = " + gamePartNum);
            // 在这里可以执行特定的操作，例如打开一个窗口、加载一个场景、改变游戏状态等。
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
