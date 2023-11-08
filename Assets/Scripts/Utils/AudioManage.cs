using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManage : MonoBehaviour
{
    [Header("投币")]
    public AudioClip coinAudioClip;
    [Header("初始化界面背景音乐")]
    public AudioClip initBgmClip;
    [Header("正常游戏背景音乐")]
    public AudioClip gameNormalClip;
    [Header("打BOSS背景音乐")]
    public AudioClip gameBossClip;
    [Header("boss出现警示音")]
    public AudioClip bossAppearClip;
    [Header("游戏开始提示音")]
    public AudioClip gameStartClip;
    [Header("游戏成功提示音")]
    public AudioClip gameSuccessClip;
    [Header("游戏失败提示音")]
    public AudioClip gameOverClip;


    [HideInInspector]
    public AudioSource audioBGMSource;

    private AudioSource audioSource;

    private AudioSource audioTip;

    public static AudioManage Instance;

    private static bool isCreated = false;


    void Awake()
    {
        if (!isCreated)
        {
            DontDestroyOnLoad(gameObject);
            isCreated = true;
        }
        else
        {
            Destroy(gameObject);
        }
        audioBGMSource = new GameObject("AudioBgm", typeof(AudioSource)).GetComponent<AudioSource>();
        audioBGMSource.transform.SetParent(base.transform);
        audioBGMSource.loop = true;

        audioTip = new GameObject("AudioTip", typeof(AudioSource)).GetComponent<AudioSource>();
        audioTip.transform.SetParent(base.transform);
        audioTip.loop = false;

    }
    void Start()
    {
        GameData.Instance.insertCoinAction = delegate
        {
            PlayClip(coinAudioClip);
        };
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBgm(AudioClip clip)
    {
        audioBGMSource.clip = clip;
        audioBGMSource.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        audioTip.clip = clip;
        audioTip.Play();
    }
}
