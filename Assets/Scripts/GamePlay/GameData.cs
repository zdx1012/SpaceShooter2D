using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay;
using UnityEngine;

public class GameData
{
    private static GameData _instance = null;
    public static GameData Instance => _instance ?? (_instance = new GameData());


    public int currentGameCoin;
    public Action insertCoinAction;
    private int coinValue;
    private int valueGame;

    private string gameCoinString;

    private int currentPlayer;

    public GameData()
    {
        currentGameCoin = 0;
        coinValue = LocalConfig.instance.gameConfig.GetCoinValue();
        valueGame = LocalConfig.instance.gameConfig.GetValueGame();
    }

    /// <summary>
    ///  获取当前多少币（分数）可以开始游戏
    /// </summary>
    /// <returns></returns>
    public int GetValueGame()
    {
        return valueGame;
    }

    /// <summary>
    ///  获取当前已有币（分数）
    /// </summary>
    /// <returns></returns>
    public int GetCurrentGameCoin()
    {
        return currentGameCoin;
    }

    /// <summary>
    ///  投入一个币
    /// </summary>
    public void AddGameCoin()
    {
        currentGameCoin += coinValue;
        insertCoinAction?.Invoke();
        Debug.Log("++ 当前可用游戏币：" + currentGameCoin);
    }

    /// <summary>
    /// 扣除游戏币
    /// </summary>
    /// <returns></returns>
    public bool ReduceGameCoin()
    {
        if (currentGameCoin >= valueGame)
        {
            currentGameCoin -= valueGame;
            Debug.Log("-- 当前可用游戏币：" + currentGameCoin);
            Debug.Log("valueGame = " + valueGame);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 当前币数是否可以开始游戏
    /// </summary>
    /// <returns></returns>
    public bool CanPlayGame()
    {
        return currentGameCoin >= valueGame;
    }

    public String GetShowCoinString()
    {
        gameCoinString = currentGameCoin.ToString();
        if (valueGame > 0) gameCoinString = gameCoinString + "/" + valueGame.ToString();
        return gameCoinString;
    }

    /// <summary>
    /// 获取玩家初始生命值
    /// </summary>
    /// <returns></returns>
    public int GetPlayerInitHealthy()
    {
        return LocalConfig.instance.gameConfig.GetHealthy();
    }

    public bool GetPlayerAutoFire()
    {
        return LocalConfig.instance.gameConfig.autoFire;
    }

    public void SetCurrentPlane(int player)
    {
        if (player < 0 || player > 2) player = 0;
        currentPlayer = player;
    }
    public int GetCurrentPlane()
    {
        return currentPlayer;
    }
}
