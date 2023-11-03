using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Scripts.GamePlay;
// using Microsoft.SqlServer.Server;
using UnityEngine;

public class GameConfig
{

    public int initHealth = 99;

    public int coinsStartGame = 1;

    public bool playVideoSound = true;

    public GameLevelConfig[] data;


    public GameConfig()
    {

    }
    public GameConfig(int _health, bool audio, GameLevelConfig[] tmp)
    {
        initHealth = _health;
        playVideoSound = audio;
        data = tmp;
    }

    public void AddCoinStartGame()
    {
        coinsStartGame += 1;
        if (coinsStartGame > 6) coinsStartGame = 6;
    }
    public void ReduceCoinStartGame()
    {
        coinsStartGame -= 1;
        if (coinsStartGame < 1) coinsStartGame = 1;
    }

    public void Addhealthy()
    {
        initHealth += 1;
        if (initHealth > 10) initHealth = 10;
    }
    public void Reducehealthy()
    {
        initHealth -= 1;
        if (initHealth < 1) initHealth = 1;
    }

    public void UpdateDemoSound()
    {
        playVideoSound = !playVideoSound;
    }
}



public class GameLevelConfig
{

    public int EnemyCount = 1;
    public int difficultyIndex = 0;

    public DifficultyLevel difficultyLevel;


    public GameLevelConfig(int enemyCount, DifficultyLevel diffculty)
    {
        EnemyCount = enemyCount;
        difficultyLevel = diffculty;
        switch (diffculty)
        {
            case DifficultyLevel.Easy:
                difficultyIndex = 0;
                break;
            case DifficultyLevel.Normal:
                difficultyIndex = 1;
                break;
            case DifficultyLevel.Hard:
                difficultyIndex = 2;
                break;
        }
    }

    public void AddEnemyCount()
    {
        EnemyCount += 1;
        if (EnemyCount > 20) { EnemyCount = 20; }
    }

    public void ReduceEnemyCount()
    {
        EnemyCount -= 1;
        if (EnemyCount < 1) { EnemyCount = 1; }
    }

    public void AddDifficulty()
    {
        difficultyIndex += 1;
        if (difficultyIndex > 2) { difficultyIndex = 2; }
        UpdateDifficulty();
    }

    public void ReduceDifficulty()
    {
        difficultyIndex -= 1;
        if (difficultyIndex < 0) { difficultyIndex = 0; }
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        switch (difficultyIndex)
        {
            case 0:
                difficultyLevel = DifficultyLevel.Easy;
                break;
            case 1:
                difficultyLevel = DifficultyLevel.Normal;
                break;
            case 2:
                difficultyLevel = DifficultyLevel.Hard;
                break;
        }
    }

}

