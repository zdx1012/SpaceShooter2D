using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 存储游戏持久化数据
public class GamePlayerPrefs
{
    private static GamePlayerPrefs _instance = null;
    public static GamePlayerPrefs Instance => _instance ?? (_instance = new GamePlayerPrefs());

    private string KeyHistoryCoin = "HistoryCoin";
    private string KeyCurrentCoin = "CurrentCoin";
    private string KeyHistoryGift = "HistoryGift";
    private string KeyCurrentGift = "CurrentGift";

    public GamePlayerPrefs()
    {
        ResetCurrentData();
    }

    private void SetIntData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    private int GetIntData(string key)
    {
        int count = PlayerPrefs.GetInt(key);
        return count > 0 ? count : 0;
    }
    // 投币
    public void AddCoinRecord()
    {
        SetIntData(KeyHistoryCoin, GetIntData(KeyHistoryCoin) + 1);
        SetIntData(KeyCurrentCoin, GetIntData(KeyCurrentCoin) + 1);
    }

    // 退礼
    public void AddGiftRecord()
    {
        SetIntData(KeyHistoryGift, GetIntData(KeyHistoryGift) + 1);
        SetIntData(KeyCurrentGift, GetIntData(KeyCurrentGift) + 1);
    }

    // 重置当前数据
    public void ResetCurrentData()
    {
        SetIntData(KeyCurrentCoin, 0);
        SetIntData(KeyCurrentGift, 0);
    }

    // 重置历史数据
    public void ResetHistoryData()
    {
        SetIntData(KeyHistoryCoin, 0);
        SetIntData(KeyHistoryGift, 0);
    }

    // 获取投入币数信息
    public int GetCurrentInsertCoin()
    {
        return GetIntData(KeyCurrentCoin);
    }
    public int GetHistoryInsertCoin()
    {
        return GetIntData(KeyHistoryCoin);
    }


    // 获取退礼信息
    public int GetCurrentReturnGift()
    {
        return GetIntData(KeyCurrentGift);
    }
    public int GetHistoryReturnGift()
    {
        return GetIntData(KeyHistoryGift);
    }
}
