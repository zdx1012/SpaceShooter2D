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

public class Init : MonoBehaviour
{
    [Header("长时间未操作，自动播放视频（S）")]
    public int waitVideoTime = 10;

    [Header("音量控制按钮")]
    public Button volumeButton;


    [Header("静音控制的图标")]
    public Sprite silentSprite;


    private Text insertCoinTip;
    private Text startGameTip;

    private GameObject videoPlayer;
    private VideoPlayer vp;
    private float pressKeyTime;

    private int gamePartNum;

    private int currentCoinNum = -1;

    private int startGameCoin = 1;
    private Text coinNumText;

    // Start is called before the first frame update
    void Start()
    {
        pressKeyTime = Time.time;
        videoPlayer = GameObject.FindGameObjectWithTag("VideoPlay");
        Debug.Assert(videoPlayer != null, "can't find VideoPlay");
        vp = videoPlayer.GetComponent<VideoPlayer>();

        gamePartNum = LocalConfig.instance.gameConfig.data.Length;


        coinNumText = GameObject.Find("CoinNum").GetComponent<Text>();
        insertCoinTip = GameObject.Find("InsertCoinTip").GetComponent<Text>();
        startGameTip = GameObject.Find("StartGameTip").GetComponent<Text>();

        startGameCoin = LocalConfig.instance.gameConfig.coinsStartGame;

        // 读取配置显示图标《静音状态》
        if (!LocalConfig.instance.gameConfig.playVideoSound) { setVolumeSilent(); }
        // if (Config.isAndroid) InputUtil.instance.SetSound(LocalConfig.instance.gameConfig.playVideoSound);

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
        if (currentCoinNum > 0)
        {
            insertCoinTip.transform.gameObject.SetActive(false);
            startGameTip.transform.gameObject.SetActive(true);
        }
        else
        {
            insertCoinTip.transform.gameObject.SetActive(true);
            startGameTip.transform.gameObject.SetActive(false);
        }

        if (currentCoinNum.ToString() != coinNumText.text.ToString())
        {
            Debug.Log("update coin num to: " + currentCoinNum);
            coinNumText.text = currentCoinNum.ToString();
            showVideoPlayer(false);
            pressKeyTime = Time.time;
        }


        if (InputUtil.instance.IsSettingKeyHold())
        {
            SceneManager.LoadScene("Set");
        }

        if (currentCoinNum - startGameCoin >= 0 && InputUtil.instance.IsStartOnceClicked())
        {
            InputUtil.instance.CutCoin(startGameCoin);
            SceneManager.LoadScene(1);
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

    private void setVolumeSilent()
    {
        volumeButton.GetComponent<Image>().sprite = silentSprite;
        volumeButton.GetComponent<AudioSource>().volume = 0;
        vp.audioOutputMode = VideoAudioOutputMode.None;
    }
}
