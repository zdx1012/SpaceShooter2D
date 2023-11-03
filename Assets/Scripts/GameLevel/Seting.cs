using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GamePlay;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class Seting : MonoBehaviour
{

    [Header("音效控制")]
    public GameObject sound;

    [Header("币数配置")]
    public GameObject coinStartGame;

    [Header("玩家生命数")]
    public GameObject healthyCount;

    [Header("关卡的预制体")]
    public GameObject prefab; // 预制体

    [Header("ScrollView的Content对象")]
    public Transform content; // ScrollView的Content对象

    private List<GameObject> allDifficultyValuesObject;
    private List<GameObject> allEnemyCountValuesObject;

    private List<GameObject> allValues;

    private int currentSelectIndex = 0;

    private GameObject currentSelectedObject;

    void Awake()
    {
        // 初始化可交互的对象
        allValues = new List<GameObject>();
        // 初始化存储各关卡设置的对象
        allDifficultyValuesObject = new List<GameObject>();
        allEnemyCountValuesObject = new List<GameObject>();

    }

    void Start()
    {
        // 创建预制体实例
        for (int i = 0; i < LocalConfig.instance.gameConfig.data.Length; i++)
        {
            GameObject newItem = Instantiate(prefab, content);
            Text tip = newItem.transform.Find("tip").GetComponent<Text>();
            tip.text = "关卡" + (i + 1).ToString();

            // EnemyValue
            GameObject enemyCountObject = newItem.transform.Find("EnemyCount/EnemyValue").gameObject;
            allEnemyCountValuesObject.Add(enemyCountObject);
            // diffcultyValue
            GameObject diffcultyObject = newItem.transform.Find("Diffculty/DiffcultyValue").gameObject;
            allDifficultyValuesObject.Add(diffcultyObject);
        }
        // 添加到所有可交互的对象
        allValues.Add(coinStartGame);
        allValues.Add(sound);
        allValues.Add(healthyCount);
        for (int i = 0; i < allEnemyCountValuesObject.Count; i++)
        {
            allValues.Add(allEnemyCountValuesObject[i]);
            allValues.Add(allDifficultyValuesObject[i]);
        }
    }


    // 更新UI
    void UpdateUI()
    {
        if (LocalConfig.instance.gameConfig.playVideoSound)
        {
            sound.GetComponent<Text>().text = "是";
        }
        else
        {
            sound.GetComponent<Text>().text = "否";
        }
        healthyCount.GetComponent<Text>().text = LocalConfig.instance.gameConfig.initHealth.ToString();
        coinStartGame.GetComponent<Text>().text = GetCoinDesc(LocalConfig.instance.gameConfig.coinsStartGame);


        // 
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(coinStartGame);
        }

        // 当前选中的是关卡
        if (currentSelectIndex >= allValues.Count - allEnemyCountValuesObject.Count)
        {
            // 设置scrollview的位置
            Vector3 newPosition = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z); ; // 分配新位置给Transform.position
            if (EventSystem.current.currentSelectedGameObject.transform.position.x > 800)
            {
                newPosition.x = content.transform.position.x - 10;
            }
            else if (EventSystem.current.currentSelectedGameObject.transform.position.x < 300)
            {
                newPosition.x = content.transform.position.x + 10;
            }
            content.transform.position = newPosition;
        }


        // 设置当前选中 和 选中缩放
        EventSystem.current.SetSelectedGameObject(allValues[currentSelectIndex]);
        for (int i = 0; i < allValues.Count; i++)
        {
            if (EventSystem.current.currentSelectedGameObject == allValues[i])
            {
                // 设置对象的缩放 倍数
                allValues[i].GetComponent<RectTransform>().localScale = new Vector3(1.4f, 1.4f, 1);
                // 更改文字的颜色为红色
                allValues[i].GetComponent<Text>().color = Color.red;
            }
            else
            {
                allValues[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                // 更改文字的颜色为 FFAE00
                allValues[i].GetComponent<Text>().color = Color.white;
            }
        }


        // 设置值
        for (int i = 0; i < allDifficultyValuesObject.Count; i++)
        {
            allDifficultyValuesObject[i].GetComponent<Text>().text = GetDifficultyDesc(LocalConfig.instance.gameConfig.data[i].difficultyIndex);
            allEnemyCountValuesObject[i].GetComponent<Text>().text = LocalConfig.instance.gameConfig.data[i].EnemyCount.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

        UpdateUI();
        // 下移
        if (InputUtil.instance.IsSettingDownOnceClicked())
        {
            currentSelectIndex = (currentSelectIndex + 1) % allValues.Count;
        }

        // 上移
        if (InputUtil.instance.IsSettingUpOnceClicked())
        {
            currentSelectIndex = (currentSelectIndex + allValues.Count - 1) % allValues.Count;
        }

        int key = 0;
        if (InputUtil.instance.IsSettingLeftOnceClicked()) key = 1;
        if (InputUtil.instance.IsSettingRightOnceClicked()) key = 2;
        // 检测按键
        if (key > 0)
        {
            bool isAdd = false;
            if (key == 2) isAdd = true;
            currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedObject == sound)
            {
                UpdateAudio();
            }
            else if (currentSelectedObject == healthyCount)
            {
                UpdatePlayerHealthy(isAdd);
            }
            else if (currentSelectedObject == coinStartGame)
            {
                UpdateCoinGame(isAdd);
            }
            else
            {
                for (int i = 0; i < allDifficultyValuesObject.Count; i++)
                {
                    if (currentSelectedObject == allDifficultyValuesObject[i])
                    {
                        UpdateDifficulty(i, isAdd);
                    }
                }
                for (int i = 0; i < allEnemyCountValuesObject.Count; i++)
                {
                    if (currentSelectedObject == allEnemyCountValuesObject[i])
                    {
                        UpdateEnemyCount(i, isAdd);
                    }
                }
            }
        }

        if (InputUtil.instance.IsStartOnceClicked())
        {
            SceneManager.LoadScene(0);
            LocalConfig.instance.ReLoadConfig();
        }

        if (InputUtil.instance.IsSettingKeyHold())
        {
            LocalConfig.instance.SaveGameConfig();
            SceneManager.LoadScene(0);
        }


    }

    private void UpdateAudio()
    {

        LocalConfig.instance.gameConfig.UpdateDemoSound();
    }

    // 更新玩家生命值
    private void UpdatePlayerHealthy(bool isAdd)
    {
        if (isAdd)
        {
            LocalConfig.instance.gameConfig.Addhealthy();
        }
        else
        {
            LocalConfig.instance.gameConfig.Reducehealthy();
        }

    }

    // 更新玩家生命值
    private void UpdateCoinGame(bool isAdd)
    {
        if (isAdd)
        {
            LocalConfig.instance.gameConfig.AddCoinStartGame();
        }
        else
        {
            LocalConfig.instance.gameConfig.ReduceCoinStartGame();
        }

    }

    // 更新敌人数量
    private void UpdateEnemyCount(int index, bool isAdd)
    {
        if (isAdd)
        {
            LocalConfig.instance.gameConfig.data[index].AddEnemyCount();
        }
        else
        {
            LocalConfig.instance.gameConfig.data[index].ReduceEnemyCount();
        }
    }

    // 更新难度等级
    private void UpdateDifficulty(int index, bool isAdd)
    {
        if (isAdd)
        {
            LocalConfig.instance.gameConfig.data[index].AddDifficulty();
        }
        else
        {
            LocalConfig.instance.gameConfig.data[index].ReduceDifficulty();
        }

    }

    // 将下标转换成文字
    private String GetDifficultyDesc(int index)
    {
        String desc = "";
        switch (index)
        {
            case 0:
                desc = "简单";
                break;
            case 1:
                desc = "正常";
                break;
            case 2:
                desc = "困难";
                break;
            default:
                break;
        }
        return desc;
    }

    private String GetCoinDesc(int index)
    {
        String text = "";
        switch (index)
        {
            case 1:
                text = "一币一玩";
                break;
            case 2:
                text = "两币一玩";
                break;
            case 3:
                text = "三币一玩";
                break;
            case 4:
                text = "四币一玩";
                break;
            case 5:
                text = "五币一玩";
                break;
            case 6:
                text = "六币一玩";
                break;
            default:
                break;
        }
        return text;
    }
}
