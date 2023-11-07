using System;
using System.Linq;
using Assets.Scripts.Factories;
using Assets.Scripts.GamePlay;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyWave
{
    public EnemyWaveSet[] Sets;
    public float Delay = 2;
    public bool HasPowerUp;
    public int PowupCount = 1;
    public PowerUpType PowerUp = PowerUpType.Shooting;
}

[Serializable]
public class EnemyWaveSet
{
    public EnemyMode Mode = EnemyMode.Default;
    public EnemyType EnemyType = EnemyType.SpaceShip1;
    public int EnemyCount = 3;
    public GameObject Path;
    public bool IsInverted;
    public EndOfPathInstruction EndOfPathMode = EndOfPathInstruction.Reverse;

    public EnemyModeOptions ToEnemyMode() => new EnemyModeOptions(Mode, Path, EndOfPathMode);

}

public class GameManager : MonoBehaviour
{
    [Header("飞机预制体")]
    public GameObject[] AllPlanes;

    [Header("难度（读取设置生成）")]
    public DifficultyLevel Difficulty = DifficultyLevel.Normal;
    //public bool IncreaseDifficultyOverTime = true;
    [Header("提供生成敌人的Y坐标")]
    public GameObject EnemySpawnPoint;

    [Header("提供生成敌人类型《给Factory使用》")]
    public GameObject[] EnemiesTemplates;

    [Header("提供生成 升级技能 的类型 《给Factory使用》")]
    public GameObject[] PowerUpsTemplates;

    [Header("提供生成每一波敌人的相关设置<具体数量，以设置中配置的数量为准，从这里随机抽取进行组成>")]
    public EnemyWave[] EnemyWaves;
    [Header("UI提示文本")]
    public Text ScoreText;
    public Text ShieldText;
    public Text HealthText;
    public Text LevelText;

    [Header("读取已存储的玩家生命值，子弹等级等信息")]

    private EnemyFactory _enemyFactory;
    private PowerUpFactory _powerUpFactory;
    private DifficultyManager _difficultyManager;
    private WaveManager _waveManager;

    [Header("游戏失败界面")]
    public GameObject _gameOverObject;
    [Header("游戏成功界面")]
    public GameObject _gameSucessObject;
    private bool _waveIsOver = false;
    // 是否暂停，0-暂停，1-正常
    // private int _timeScale = 1;

    void Awake()
    {
        switch (LocalConfig.instance.gameConfig.GetDifficulty())
        {
            case 0:
                Difficulty = DifficultyLevel.Easy;
                break;
            case 1:
                Difficulty = DifficultyLevel.Normal;
                break;
            case 2:
                Difficulty = DifficultyLevel.Hard;
                break;
        }
        _enemyFactory = EnemyFactory.Instance;
        _enemyFactory.LoadTemplates(EnemiesTemplates);

        _powerUpFactory = PowerUpFactory.Instance;
        _powerUpFactory.LoadTemplates(PowerUpsTemplates);
        _difficultyManager = new DifficultyManager(Difficulty, _enemyFactory.AvailableTypes().ToList());


        // 读取配置，选择飞机模型
        GameObject prefabInstance = Instantiate(AllPlanes[GameData.Instance.GetCurrentPlane()]);
        prefabInstance.transform.position = new Vector3(0f, -3f, 0f);

        // 设置生命值
        Game.Current.Player.Health = LocalConfig.instance.gameConfig.initHealth;

        _waveManager = new WaveManager(EnemyWaves, _difficultyManager, EnemySpawnPoint);

        Effetcs.Load();
        Game.Current.StartNew();

        // 读取上次通关保存的玩家数据
        if (PlayerInfo.instance.hasUpdate) Game.Current.ReadPlayerInfoData();
    }

    void Start()
    {
    }


    void Update()
    {


        // Select 键 暂停
        // if (Input.GetKeyDown(KeyCode.Pause))
        // {
        //     _timeScale = _timeScale == 1 ? 0 : 1;
        //     Time.timeScale = _timeScale;
        // }
        // // Select + Start 键 关卡选择界面
        // if (Input.GetKey(KeyCode.Pause) && Input.GetKey(KeyCode.Return)){
        //     SceneManager.LoadScene(0);
        // }

        UpdateUI();

        // if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !_waveIsOver)
        // {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // }

        // 所有敌人生成完毕后,检测是否还有敌人，没有则提示游戏结束
        if (_waveIsOver)
        {
            if (GameObject.FindGameObjectsWithTag(ObjectTags.Enemy).ToArray().Length == 0)
            {
                _gameSucessObject.SetActive(true);
                if (InputUtil.instance.IsStartOnceClicked() || InputUtil.instance.AnyAxisPressed())
                {
                    StartCoroutine(GotoNextGameLevel());
                }
                return;
            }
        }

        if (Game.Current.Player.Health <= 0)
        {
            _gameOverObject.SetActive(true);
            if (InputUtil.instance.IsStartOnceClicked() || InputUtil.instance.AnyAxisPressed())
            {
                StartCoroutine(GotoGameLevel());
            }
        }

        _waveManager.ExecuteCurrentWave();
        // 
        if (_waveManager.CurrentWave.IsHalfCreated && _waveManager.CurrentWave.Definition.HasPowerUp && !_waveManager.CurrentWave.HasCreatePowerUp && Random.Range(-10000, 10) > 7)
        {
            for (int i = 0; i < _waveManager.CurrentWave.Definition.PowupCount; i++)
            {
                var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
                var powerUpType = _waveManager.CurrentWave.Definition.PowerUp;
                _powerUpFactory.Create(powerUpType, pos);
            }
            _waveManager.CurrentWave.HasCreatePowerUp = true;
        }

        // 当前一波敌人生成完成，自动生成下一波
        if (_waveManager.CurrentWave.Ended)
        {
            _waveManager.MoveNext();
            // 判断敌人是否全部生成完成
            if (_waveManager.CurrentWave.Index == _waveManager.WaveLength - 1)
            {
                _waveIsOver = true;
            }
        }
        // 生成小行星
        if (_difficultyManager.CanCreateAsteroid())
        {
            _difficultyManager.NotifyEnemyTypeSelected(EnemyType.Asteroid1);
            var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
            _enemyFactory.Create(EnemyType.Asteroid1, pos);
        }
    }


    private void UpdateUI()
    {
        if (ScoreText)
        {
            ScoreText.text = Game.Current.Score.ToString();
        }
        if (ShieldText)
        {
            ShieldText.text = Game.Current.Player.ShieldPower.ToString();
            // ShieldText.text = $"{_waveManager.CurrentWave.Index} / {_waveManager.WaveLength}";
        }
        if (HealthText)
        {
            HealthText.text = Game.Current.Player.Health.ToString();
        }
        if (LevelText)
        {
            LevelText.text = Game.Current.Player.ShootingPower.ToString();
        }

    }

    IEnumerator GotoGameLevel()
    {
        yield return new WaitForSeconds(3f);
        // 清空通关数据
        PlayerInfo.instance.Reset();
        SceneManager.LoadScene(0);
        yield break;
    }

    IEnumerator GotoNextGameLevel()
    {
        Debug.Log("currentGameLevel : " + Game.Current.currentGameLevel + "/" + Game.Current.totalGameLevel);
        yield return new WaitForSeconds(3f);
        if (Game.Current.currentGameLevel + 1 < Game.Current.totalGameLevel)
        {
            Game.Current.currentGameLevel += 1;
            // 保存通关数据
            PlayerInfo.instance.SaveData(Game.Current.Player.Health, Game.Current.Score, Game.Current.Player.ShootingPower);
            Debug.Log("load scene " + (Game.Current.currentGameLevel + 1));
            SceneManager.LoadScene(Game.Current.currentGameLevel + 1);
        }
        else if (Game.Current.currentGameLevel + 1 == Game.Current.totalGameLevel)
        {
            Debug.Log("load scene 0");
            SceneManager.LoadScene(0);
        }
        yield break;
    }
}
