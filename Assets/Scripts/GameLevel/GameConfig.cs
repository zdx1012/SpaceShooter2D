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

    public bool playAudio = true;

    public GameLevelConfig[] data;


    public GameConfig()
    {

    }
    public GameConfig(int _health, bool audio, GameLevelConfig[] tmp)
    {
        initHealth = _health;
        playAudio = audio;
        data = tmp;
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

}



public class GameLevelConfig
{

    public int EnemyCount = 1;
    public int difficultyIndex = 0;

    public DifficultyLevel difficultyLevel;


    public GameLevelConfig(int n, DifficultyLevel diffculty)
    {
        EnemyCount = n;
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

