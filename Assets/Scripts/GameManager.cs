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

    [Header("提供生成每一波敌人的相关设置")]
    public EnemyWave[] EnemyWaves;
    [Header("UI提示文本")]
    public Text ScoreText;
    public Text HealthText;
    public Text CoinText;
    [Header("boss生命")]
    public GameObject BossHP;
    [Header("boss当前生命")]
    public GameObject BossCurrentHP;
    // 当前BOSSS对象，获取生命信息
    private SpaceShipEnemy BossObject;
    public GameObject BossCome;

    [Header("读取已存储的玩家生命值，子弹等级等信息")]

    private EnemyFactory _enemyFactory;
    private PowerUpFactory _powerUpFactory;
    private DifficultyManager _difficultyManager;
    private WaveManager _waveManager;

    private GameState gameState;
    private bool isDuplicates; // 当前状态是否是重复的状态

    [Header("游戏失败界面")]
    public GameObject _gameOverObject;

    [Header("游戏成功界面")]
    public GameObject _gameSuccessObject;

    [Header("游戏开始界面")]
    public GameObject _gameStartbject;


    [Header("继续界面")]
    public GameObject _gameContinueObject;

    [Header("退礼界面")]
    public GameObject _gameGiftbject;
    public Text _gameGiftReturnCountText;
    public Text _gameGiftReturnOkText;
    public GameObject _gameNoGiftbject;

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

        _waveManager = new WaveManager(EnemyWaves, _difficultyManager, EnemySpawnPoint);

        Effetcs.Load();
        Game.Current.StartNew();
        gameState = GameState.Start;


    }

    void Start()
    {
        // 读取上次通关保存的玩家数据
        if (PlayerTmpInfo.instance.hasUpdate) Game.Current.ReadPlayerTmpInfoData();

        AudioManage.Instance.PlayBgm(AudioManage.Instance.gameNormalClip);
        AudioManage.Instance.PlayClip(AudioManage.Instance.gameStartClip);
        GameData.Instance.insertCoinUpdateTextAction = delegate
        {
            CoinText.text = GameData.Instance.GetShowCoinString();
        };
    }


    void Update()
    {
        if (Time.timeScale == 0f)
        {
            ContinueGame();
            return;
        }
        else if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            SceneManager.LoadScene(CurrentGameScene.SettingMain.GetDescription());
        }
        // 更新U元素
        UpdateUI();


        // s生成boss后，检测是否还有BOSS，没有则提示游戏结束
        if (_waveManager.BossCreated && ((BossObject != null && BossObject.Health == 0) || BossObject == null) && !isDuplicates)
        {
            SetGameState(GameState.Success);
            isDuplicates = true; // 游戏成功后，已跳转场景，这个值可不用再设置为false
            return;
        }
        if (Game.Current.Player.Health <= 0 && !isDuplicates)
        {
            SetGameState(GameState.Contiunue);
            isDuplicates = true;
        }

        _waveManager.ExecuteCurrentWave();
        // 随机生成道具
        if (_waveManager.CurrentWave.IsHalfCreated && _waveManager.CurrentWave.Definition.HasPowerUp)
        {
            if (_waveManager.CurrentWave.HasCreatePowerUp < _waveManager.CurrentWave.Definition.PowupCount && Random.Range(-10000, 20) > 0)
            {
                var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
                var powerUpType = _waveManager.CurrentWave.Definition.PowerUp;
                _powerUpFactory.Create(powerUpType, pos);
                _waveManager.CurrentWave.HasCreatePowerUp += 1;
            }

        }

        // 当前一波敌人生成完成，自动生成下一波
        if (_waveManager.CurrentWave.Ended)
        {
            _waveManager.MoveNext();
        }
        // 生成小行星
        if (_difficultyManager.CanCreateAsteroid())
        {
            _difficultyManager.NotifyEnemyTypeSelected(EnemyType.Asteroid1);
            var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
            _enemyFactory.Create(EnemyType.Asteroid1, pos);
        }
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    private void ContinueGame()
    {
        if (InputUtil.instance.IsStartOnceClicked() && GameData.Instance.CanPlayGame())
        {
            GameData.Instance.ReduceGameCoin();
            Game.Current.Player.ResetHealth(LocalConfig.instance.gameConfig.GetHealthy());
            Debug.Log("重置生命值 " + Game.Current.Player.Health);
            _gameContinueObject.SetActive(false);
            SetGameState(GameState.None, true);
            Time.timeScale = 1f;
            isDuplicates = false;
        }
    }

    private void UpdateUI()
    {
        if (ScoreText)
        {
            ScoreText.text = Game.Current.Score.ToString();
        }
        if (HealthText)
        {
            HealthText.text = Game.Current.Player.Health.ToString();
        }
        if (CoinText)
        {
            CoinText.text = GameData.Instance.GetShowCoinString();
        }
        switch (gameState)
        {
            case GameState.Start:
                _gameStartbject.SetActive(true);
                gameState = GameState.None;
                break;
            case GameState.Success:
                _gameSuccessObject.SetActive(true);
                AudioManage.Instance.PlayClip(AudioManage.Instance.gameSuccessClip);
                // 非第三关则跳转下一关，否则弹出礼品
                if (SceneManager.GetActiveScene().name != CurrentGameScene.Part3.GetDescription())
                {
                    StartCoroutine(GotoNextGameLevel());
                }
                else
                {
                    StartCoroutine(ReadyChangeGameState(GameState.Gift, 3));
                }
                gameState = GameState.None;
                break;
            case GameState.GameOver:
                _gameOverObject.SetActive(true);
                AudioManage.Instance.PlayClip(AudioManage.Instance.gameOverClip);
                gameState = GameState.None;
                StartCoroutine(ReadyChangeGameState(GameState.Gift, 3));
                break;
            case GameState.Contiunue:
                _gameContinueObject.SetActive(true);
                Debug.Log("goto coninue");
                CountDownCallBack countDownCallBack = delegate ()
                {
                    _gameContinueObject.SetActive(false);
                    Game.Current.Player.RunHealthyCheck();
                    SetGameState(GameState.GameOver, true);
                    Time.timeScale = 1f;
                };
                _gameContinueObject.GetComponent<CountDown>().StartCountDown(GameData.Instance.GetContinueTime(), countDownCallBack);
                Time.timeScale = 0f;
                break;
            case GameState.Gift:
                StartCoroutine(ShowGiftObject());
                gameState = GameState.None;
                break;
            case GameState.NoGift:
                break;
            case GameState.None:
                break;
        }
        if (_waveManager.IsLastWave() && BossCome.activeSelf == false && !_waveManager.BossCreated)
        {
            BossCome.SetActive(true);
            AudioManage.Instance.PlayClip(AudioManage.Instance.bossAppearClip);
            AudioManage.Instance.PlayBgm(AudioManage.Instance.gameBossClip);
        }
        if (_waveManager.BossCreated)
        {

            if (BossObject is null)
            {
                GameObject[] tmp = GameObject.FindGameObjectsWithTag(ObjectTags.Enemy);
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].name.StartsWith("Boss"))
                    {
                        BossObject = tmp[i].GetComponent<SpaceShipEnemy>();
                        BossHP.SetActive(true);
                    }
                }
            }
            else
            {
                float width = BossHP.GetComponent<RectTransform>().rect.width * BossObject.Health / BossObject.MaxHealth;
                float Height = BossCurrentHP.GetComponent<RectTransform>().rect.height;
                BossCurrentHP.GetComponent<RectTransform>().sizeDelta = new Vector2(width, Height);
            }
        }
    }

    IEnumerator GotoGameInit()
    {
        yield return new WaitForSeconds(3f);
        // 清空通关数据
        PlayerTmpInfo.instance.Reset();
        SceneManager.LoadScene(CurrentGameScene.Init.GetDescription());
        yield break;
    }

    IEnumerator GotoNextGameLevel()
    {
        yield return new WaitForSeconds(3f);
        string targetSceneName = "";
        if (SceneManager.GetActiveScene().name == CurrentGameScene.Part1.GetDescription())
        {
            targetSceneName = CurrentGameScene.Part2.GetDescription();
        }
        else if (SceneManager.GetActiveScene().name == CurrentGameScene.Part2.GetDescription())
        {
            targetSceneName = CurrentGameScene.Part3.GetDescription();
        }
        else if (SceneManager.GetActiveScene().name == CurrentGameScene.Part3.GetDescription())
        {
            targetSceneName = CurrentGameScene.Init.GetDescription();
            yield break;
        }
        PlayerTmpInfo.instance.SaveData(Game.Current.Player.Health, Game.Current.Score, Game.Current.Player.ShootingPower);
        Debug.Log("goto scene " + targetSceneName);
        SceneManager.LoadScene(targetSceneName);
        yield break;
    }

    /// <summary>
    /// 即时设置游戏状态
    /// </summary>
    /// <param name="state">状态</param>
    /// <param name="isForce">强制更改(当前状态不为None,且没有设置强制更新，则不会更新)</param>
    void SetGameState(GameState state, bool isForce = false)
    {
        if (gameState == GameState.None || isForce)
        {
            Debug.Log("set to " + state);
            gameState = state;
        }
    }

    /// <summary>
    /// 延时更改状态
    /// </summary>
    /// <param name="state">状态</param>
    /// <param name="Seconds">时间</param>
    /// <returns></returns>
    IEnumerator ReadyChangeGameState(GameState state, int Seconds)
    {
        yield return new WaitForSeconds(Seconds);
        SetGameState(state);
    }


    /// <summary>
    ///  显示退礼物弹窗
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowGiftObject()
    {
        _gameGiftbject.SetActive(true);
        float sendGiftStartTime = Time.time;
        float sendGiftMaxTime = 10f;
        int currentReturnOK = 0;
        while (true)
        {
            _gameGiftReturnCountText.text = GiftManager.Instance.GetPlayerGetCount(
                Game.Current.Score,
                 LocalConfig.instance.gameConfig.GetGiftScore(),
                 LocalConfig.instance.gameConfig.GetGiftCount(),
                 LocalConfig.instance.gameConfig.GetBoolGiftAdd()
                 ).ToString();
            _gameGiftReturnOkText.text = GiftManager.Instance.GetPlayerOkCount().ToString();
            Debug.Log("return = " + _gameGiftReturnCountText.text + " ok = " + _gameGiftReturnOkText.text);
            GiftManager.Instance.StartReturnGift();
            if (_gameGiftReturnCountText.text == _gameGiftReturnOkText.text)
            {
                // _gameNoGiftbject.SetActive(false);
                yield return new WaitForSeconds(1);
                StartCoroutine(GotoGameInit());
                yield break;
            }
            else if (Time.time - sendGiftStartTime > sendGiftMaxTime)
            {
                Debug.Log("退礼失败");
                _gameNoGiftbject.SetActive(true);
                yield break;
            }
            // 退礼成功一个，刷新退礼起始时间
            if (currentReturnOK.ToString() != _gameGiftReturnOkText.text)
            {
                sendGiftStartTime = Time.time;
                currentReturnOK = int.Parse(_gameGiftReturnOkText.text);
            }

            yield return null; // 等待下一帧
        }
    }
}
