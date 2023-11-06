using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Scripts.GamePlay;
// using Microsoft.SqlServer.Server;
using UnityEngine;
using UnityEngine.Video;

public class GameConfig
{

    // 生命值
    public int initHealth = 0;

    // 一币几分
    public int coinValue = 1;
    // 几分一玩
    public int valueGame = 1;

    // 奖励累加
    public bool scoreAdd = false;

    // 游戏音量
    public int volume = 0;

    // 游戏难度
    public int difficultyLevel = 0;

    // 续玩时间
    public int waitCoinTime = 0;
    // 演示视频
    public bool demoVideo = true;
    // 自动射击
    public bool autoFire = false;

    public int language = 0; // 0-中文，1-英文


    private String[] languageChoice = { "中文", "English" };
    public int[] healthyChoice = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
    public int[] coinValueChoice = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public int[] valueGameChoice = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public String[] boolValueChoice = { "是", "否" };
    public String[] boolValueChoiceEn = { "Yes", "No" };
    public float[] volumeChoice = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
    public int[] diffcultyChoice = { 1, 2, 3 };
    public int[] waitCoinTimeChoice = { 5, 10, 15, 20, 25, 30 };



    public GameConfig(int _language, int _health, int _coinValue, int _valueGame, bool _scoreAdd, int _volume, int _difficultyLevel, int _waitCoinTime, bool _demoVideo, bool _autoFire)
    {
        language = _language;
        initHealth = _health;
        coinValue = _coinValue;
        valueGame = _valueGame;
        scoreAdd = _scoreAdd;
        volume = _volume;
        difficultyLevel = _difficultyLevel;
        waitCoinTime = _waitCoinTime;
        demoVideo = _demoVideo;
        autoFire = _autoFire;
    }

    /// <summary>
    /// 设置语言（自动根据值会做适应）
    /// </summary>
    /// <param name="_language">会自动处理，0-上一个，1-下一个</param>
    public void SetLanguage(bool isNext)
    {
        language = isNext ? language + 1 : language - 1;
        if (language >= languageChoice.Length) language = 0;
        if (language < 0) language = languageChoice.Length - 1;
    }
    public string GetLanguage()
    {
        Debug.Log("language=" + language);
        return languageChoice[language];
    }

    /// <summary>
    /// 设置生命值（自动根据值会做适应）
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个，1-下一个</param>
    public void SetHealthy(bool isNext)
    {
        initHealth = isNext ? initHealth + 1 : initHealth - 1;
        if (initHealth >= healthyChoice.Length) initHealth = 0;
        if (initHealth < 0) initHealth = healthyChoice.Length - 1;
    }
    public int GetHealthy()
    {
        Debug.Log("initHealth=" + healthyChoice[initHealth]);
        return healthyChoice[initHealth];
    }


    /// <summary>
    ///  设置一币几分（自动根据值会做适应）
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个候选值，1-下一个候选值</param>
    public void SetCoinValue(bool isNext)
    {
        coinValue = isNext ? coinValue + 1 : coinValue - 1;
        if (coinValue >= coinValueChoice.Length) coinValue = 0;
        if (coinValue < 0) coinValue = coinValueChoice.Length - 1;
    }
    public int GetCoinValue()
    {
        Debug.Log("coinValue=" + coinValueChoice[coinValue]);
        return coinValueChoice[coinValue];
    }

    /// <summary>
    ///  设置几分一次（自动根据值会做适应）
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个候选值，1-下一个候选值</param>
    public void SetValueGame(bool isNext)
    {
        valueGame = isNext ? valueGame + 1 : valueGame - 1;
        if (valueGame >= valueGameChoice.Length) valueGame = 0;
        if (valueGame < 0) valueGame = valueGameChoice.Length - 1;
    }

    public int GetValueGame()
    {
        Debug.Log("valueGame=" + valueGameChoice[valueGame]);
        return valueGameChoice[valueGame];
    }

    /// <summary>
    /// 设置是否奖励累加
    /// </summary>
    public void SetScoreAdd()
    {
        scoreAdd = !scoreAdd;
    }
    public String GetScoreAdd()
    {
        if (language == 0)
        {
            return boolValueChoice[scoreAdd ? 0 : 1];
        }
        else
        {
            return boolValueChoiceEn[scoreAdd ? 0 : 1];
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个候选值，1-下一个候选值</param>
    public void SetVolumeGame(bool isNext)
    {
        volume = isNext ? volume + 1 : volume - 1;
        if (volume >= volumeChoice.Length) volume = 0;
        if (volume < 0) volume = volumeChoice.Length - 1;
    }
    public float GetVolume()
    {
        Debug.Log("value=" + volumeChoice[valueGame]);
        return volumeChoice[volume];
    }

    /// <summary>
    /// 设置难度
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个候选值，1-下一个候选值</param>
    public void SetDifficulty(bool isNext)
    {
        difficultyLevel = isNext ? difficultyLevel + 1 : difficultyLevel - 1;
        if (difficultyLevel >= diffcultyChoice.Length) difficultyLevel = 0;
        if (difficultyLevel < 0) difficultyLevel = diffcultyChoice.Length - 1;
    }

    public int GetDifficulty()
    {
        Debug.Log("difficultyLevel=" + diffcultyChoice[difficultyLevel]);
        return diffcultyChoice[difficultyLevel];
    }

    /// <summary>
    /// 设置游戏结束等待时间
    /// </summary>
    /// <param name="isNext">会自动处理，0-上一个候选值，1-下一个候选值</param>
    public void SetWaitCoinTime(bool isNext)
    {
        waitCoinTime = isNext ? waitCoinTime + 1 : waitCoinTime - 1;
        if (waitCoinTime >= waitCoinTimeChoice.Length) waitCoinTime = 0;
        if (waitCoinTime < 0) waitCoinTime = waitCoinTimeChoice.Length - 1;
    }
    public int GetWaitCoinTime()
    {
        Debug.Log("waitCoinTime=" + waitCoinTimeChoice[waitCoinTime]);
        return waitCoinTimeChoice[waitCoinTime];
    }

    /// <summary>
    /// 设置是否播放演示视频
    /// </summary>
    public void SetDemoVideo()
    {
        demoVideo = !demoVideo;
    }
    public String GetDemoVideo()
    {
        if (language == 0)
        {
            return boolValueChoice[demoVideo ? 0 : 1];
        }
        else
        {
            return boolValueChoiceEn[demoVideo ? 0 : 1];
        }
    }


    public void SetAutoFire()
    {
        autoFire = !autoFire;
    }
    public String GetAutoFire()
    {
        if (language == 0)
        {
            return boolValueChoice[autoFire ? 0 : 1];
        }
        else
        {
            return boolValueChoiceEn[autoFire ? 0 : 1];
        }
    }

}


