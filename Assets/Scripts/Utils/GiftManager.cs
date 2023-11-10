﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GiftManager
{

    public PlayerGiftInfo playerGiftInfo = null;

    private static GiftManager _instance = null;

    private float sendGiftTime = 0f;
    private float IntervalTime = 3f;
    public static GiftManager Instance => _instance ?? (_instance = new GiftManager());

    public GiftManager()
    {
        playerGiftInfo = new PlayerGiftInfo();
    }

    /// <summary>
    /// 获取可退礼数量
    /// </summary>
    /// <param name="currentScore">当前分数</param>
    /// <param name="giftScore">退礼所需分数</param>
    /// <param name="giftCount">分数达标退礼个数</param>
    /// <param name="giftAdd">奖励是否累加<100分达标，达到500分，则可以退5次></param>
    /// <returns></returns>
    public int GetPlayerGetCount(int currentScore, int giftScore, int giftCount, bool giftAdd)
    {
        int count = 0;
        if (giftAdd)
        {
            count = (int)Math.Floor((float)currentScore / giftScore) * giftCount;
        }
        else
        {
            if (currentScore > giftScore) count = giftCount;
        }
        playerGiftInfo.GetCount = count;
        return count;
    }

    // 退礼成功数量
    public int GetPlayerOkCount()
    {
        return playerGiftInfo.OkCount;
    }

    /// <summary>
    /// 是否可以退礼
    /// </summary>
    /// <returns></returns>
    public bool CanReturnGift()
    {
        return playerGiftInfo.GetCount > playerGiftInfo.OkCount;
    }

    private bool SendGift()
    {
        if (CanReturnGift())
        {
            Debug.Log("发送礼物 do something ");
            if (UnityEngine.Random.Range(1, 10) > 5)
            {
                Debug.Log("发送礼物成功");
                return true;
            }
            else
            {
                Debug.Log("发送礼物失败");
            }
        }
        return false;
    }

    // 开始退礼
    public void StartReturnGift()
    {
        if (Time.time - sendGiftTime > IntervalTime && CanReturnGift())
        {
            if (SendGift())
            {
                playerGiftInfo.OkCount += 1;
            }
            sendGiftTime = Time.time;
        }
    }
}