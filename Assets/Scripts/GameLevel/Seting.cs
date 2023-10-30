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

    [Header("玩家生命数")]
    public GameObject healthyCount;

    [Header("第一关敌人数量")]
    public GameObject game1EnemyCount;

    [Header("第一关难度")]
    public GameObject game1Difficulty;

    [Header("第二关敌人数量")]
    public GameObject game2EnemyCount;

    [Header("第二关难度")]
    public GameObject game2Difficulty;



    private List<GameObject> allValues;

    private int currentSelectIndex = 0;

    private GameObject currentSelectedObject;

    void Awake()
    {
        // 初始化可交互的对象
        allValues = new List<GameObject>();

        allValues.Add(sound);
        allValues.Add(healthyCount);
        allValues.Add(game1Difficulty);
        allValues.Add(game1EnemyCount);
        allValues.Add(game2Difficulty);
        allValues.Add(game2EnemyCount);

    }
    void Start()
    {

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
                // Debug.Log(" curSelctedGameObject.parent.position = " + EventSystem.current.currentSelectedGameObject.transform.parent.transform.position);
                Renderer parentRenderer = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<Renderer>();
                if (parentRenderer != null)
                {
                    Color newColor = new Color(0f, 0.5f, 1f, 0.5f); // 浅蓝色，透明度为50%
                    parentRenderer.material.color = newColor;
                }
            }
            else
            {
                allValues[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                // 更改文字的颜色为 FFAE00
                allValues[i].GetComponent<Text>().color = Color.white;
            }
        }

        // 设置值
        game1Difficulty.GetComponent<Text>().text = GetDifficultyDesc(LocalConfig.instance.gameConfig.data[0].difficultyIndex);
        game1EnemyCount.GetComponent<Text>().text = LocalConfig.instance.gameConfig.data[0].EnemyCount.ToString();

        game2Difficulty.GetComponent<Text>().text = GetDifficultyDesc(LocalConfig.instance.gameConfig.data[1].difficultyIndex);
        game2EnemyCount.GetComponent<Text>().text = LocalConfig.instance.gameConfig.data[1].EnemyCount.ToString();

    }

    // Update is called once per frame
    void Update()
    {

        UpdateUI();
        // 下移
        if (InputUtil.instance.isSettingDownOnceClicked())
        {
            currentSelectIndex = (currentSelectIndex + 1) % allValues.Count;
        }

        // 上移
        if (InputUtil.instance.isSettingUpOnceClicked())
        {
            currentSelectIndex = (currentSelectIndex + allValues.Count - 1) % allValues.Count;
        }

        int key = 0;
        if (InputUtil.instance.isSettingLeftOnceClicked()) key = 1;
        if (InputUtil.instance.isSettingRightOnceClicked()) key = 2;
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
            else if (currentSelectedObject == game1Difficulty)
            {
                UpdateDifficulty(0, isAdd);
            }
            else if (currentSelectedObject == game1EnemyCount)
            {
                UpdateEnemyCount(0, isAdd);
            }
            else if (currentSelectedObject == game2Difficulty)
            {
                UpdateDifficulty(1, isAdd);
            }
            else if (currentSelectedObject == game2EnemyCount)
            {
                UpdateEnemyCount(1, isAdd);
            }
        }

        if (InputUtil.instance.isStartOnceClicked())
        {
            SceneManager.LoadScene("Start");
        }

        if (InputUtil.instance.isSettingCenterOnceClicked())
        {
            LocalConfig.instance.SaveGameConfig();
            SceneManager.LoadScene("Set");
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

}
