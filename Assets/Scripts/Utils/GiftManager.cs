using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GiftManager
{

    public PlayerGiftInfo playerGiftInfo = null;

    private static GiftManager _instance = null;
    public static GiftManager Instance => _instance ?? (_instance = new GiftManager());

    public GiftManager()
    {
        playerGiftInfo = new PlayerGiftInfo();
    }

    /// <summary>
    /// 获取可退礼数量
    /// </summary>
    /// <returns></returns>
    public int GetPlayerGetCount(int currentScore, int giftScore)
    {
        int count = (int)Math.Floor((float)currentScore / giftScore);
        playerGiftInfo.GetCount = count;
        return count;
    }

    // 已申请退礼数量
    public int GetPlayerReturnCount()
    {
        return playerGiftInfo.ReturnCount;
    }


    // 退礼成功数量
    public int GetPlayerOkCount()
    {
        return playerGiftInfo.OkCount;
    }

    // 开始退礼
    void StartReturnGift()
    {
        
    }
}