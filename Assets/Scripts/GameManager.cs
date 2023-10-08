using System;
using System.Linq;
using Assets.Scripts.Factories;
using Assets.Scripts.GamePlay;
using PathCreation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject EnemySpawnPoint;
    public GameObject[] EnemiesTemplates;
    public GameObject[] PowerUpsTemplates;
    public EnemyWave[] EnemyWaves;
    public Text ScoreText;
    public Text ShieldText;
    public Text HealthText;
    public Text LevelText;

    private EnemyFactory _enemyFactory;
    private PowerUpFactory _powerUpFactory;
    private DifficultyManager _difficultyManager;
    private WaveManager _waveManager;

    void Awake()
    {
        _enemyFactory = EnemyFactory.Instance;
        _enemyFactory.LoadTemplates(EnemiesTemplates);

        _powerUpFactory = PowerUpFactory.Instance;
        _powerUpFactory.LoadTemplates(PowerUpsTemplates);

        _difficultyManager = new DifficultyManager(Difficulty, _enemyFactory.AvailableTypes().ToList());
        Debug.Log($"zzz EnemyWaves = {EnemyWaves.Length}");
        _waveManager = new WaveManager(EnemyWaves, _difficultyManager, EnemySpawnPoint);

        Effetcs.Load();
        Game.Current.StartNew();
    }

    void Update()
    {
        UpdateUI();

        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        _waveManager.ExecuteCurrentWave();

        // 生成 升级技能点《可拾取》
        if (_waveManager.CurrentWave.Ended)
        {
            // handle wave powerUp if present
            // ???????????????
            if (_waveManager.CurrentWave.Definition.HasPowerUp)
            {
                for(int i =0;i<_waveManager.CurrentWave.Definition.PowupCount;i++){
                    var pos = ScreenHelper.GetRandomScreenPoint(y: EnemySpawnPoint.transform.position.y);
                    var powerUpType = _waveManager.CurrentWave.Definition.PowerUp;
                    _powerUpFactory.Create(powerUpType, pos);
                }

            }

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

    private void UpdateUI()
    {
        ScoreText.text = Game.Current.Score.ToString();
        if (ShieldText)
        {
            ShieldText.text = Game.Current.Player.ShieldPower.ToString();
        }
        if (HealthText)
        {
            HealthText.text = Game.Current.Player.Health.ToString();
        }
        if (LevelText){
            LevelText.text = Game.Current.Player.ShootingPower.ToString();
        }

    }
}
