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

    private GameObject _gameOverImage;
    private GameObject _gameSucessImage;
    private bool _waveIsOver = false;
    // 是否暂停，0-暂停，1-正常
    // private int _timeScale = 1;

    void Awake()
    {
        _enemyFactory = EnemyFactory.Instance;
        _enemyFactory.LoadTemplates(EnemiesTemplates);

        _powerUpFactory = PowerUpFactory.Instance;
        _powerUpFactory.LoadTemplates(PowerUpsTemplates);
        // 读取本地配置文件，设置游戏难度
        Debug.Assert(LocalConfig.instance.gameConfig.data.Length >= Game.Current.currentGameLevel, "游戏关卡配置有误(当前关卡未配置)");
        Difficulty = LocalConfig.instance.gameConfig.data[Game.Current.currentGameLevel].difficultyLevel;
        _difficultyManager = new DifficultyManager(Difficulty, _enemyFactory.AvailableTypes().ToList());
        // 设置生命值
        Game.Current.Player.Health = LocalConfig.instance.gameConfig.initHealth;

        // 设置敌人波数<读取配置文件敌人数据>
        List<EnemyWave> newEnemyWaves = new List<EnemyWave>();
        if (EnemyWaves.Length > 0)
        {
            int maxEnemyCount = LocalConfig.instance.gameConfig.data[Game.Current.currentGameLevel].EnemyCount;
            newEnemyWaves.Clear(); // 清空列表

            for (int i = 0; i < maxEnemyCount; i++)
            {
                int randomIndex = Random.Range(0, EnemyWaves.Length);
                newEnemyWaves.Add(EnemyWaves[randomIndex]);
            }
        }
        EnemyWaves = newEnemyWaves.ToArray();
        _waveManager = new WaveManager(EnemyWaves, _difficultyManager, EnemySpawnPoint);

        Effetcs.Load();
        Game.Current.StartNew();

        // 读取上次通关保存的玩家数据
        if (PlayerInfo.instance.hasUpdate) Game.Current.ReadPlayerInfoData();
    }

    void Start()
    {
        _gameSucessImage = GameObject.Find("GameSuccessImage").gameObject;
        _gameSucessImage.SetActive(false);

        _gameOverImage = GameObject.Find("GameOverImage").gameObject;
        _gameOverImage.SetActive(false);
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
                _gameSucessImage.SetActive(true);
                if (InputUtil.instance.isStartOnceClicked())
                {
                    StartCoroutine(GotoNextGameLevel());
                }
                return;
            }
        }

        if (Game.Current.Player.Health <= 0)
        {
            _gameOverImage.SetActive(true);
            if (InputUtil.instance.isStartOnceClicked() || InputUtil.instance.AnyAxisPressed())
            {
                StartCoroutine(GotoGameLevel());
            }
        }

        _waveManager.ExecuteCurrentWave();

        // 每一波结束并且非最后一波敌人生成完成后， 随机升级技能点《可拾取》
        if (_waveManager.CurrentWave.Ended && !_waveIsOver)
        {
            // handle wave powerUp if present
            if (_waveManager.CurrentWave.Definition.HasPowerUp)
            {
                for (int i = 0; i < _waveManager.CurrentWave.Definition.PowupCount; i++)
                {
                    var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
                    var powerUpType = _waveManager.CurrentWave.Definition.PowerUp;
                    _powerUpFactory.Create(powerUpType, pos);
                }

            }

            _waveIsOver = !_waveManager.MoveNext();
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
        yield return new WaitForSeconds(3f);
        if (Game.Current.currentGameLevel + 1 < Game.Current.totalGameLevel)
        {
            Game.Current.currentGameLevel += 1;
            // 保存通关数据
            PlayerInfo.instance.SaveData(Game.Current.Player.Health, Game.Current.Score, Game.Current.Player.ShootingPower);
            SceneManager.LoadScene(Game.Current.currentGameLevel);
        }
        else if (Game.Current.currentGameLevel + 1 == Game.Current.totalGameLevel)
        {
            SceneManager.LoadScene("Start");
        }
        yield break;
    }
}
