using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Factories;
using PathCreation;
using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public class WaveManager
    {
        private EnemyFactory _enemyFactory;
        private readonly EnemyWave[] _waves;
        private readonly DifficultyManager _difficultyManager;
        private readonly GameObject _defaultSpawnPoint;

        private CurrentWave _currentWave;
        internal CurrentWave CurrentWave => _currentWave;
        public bool Ended { get; private set; } = false;

        public WaveManager(EnemyWave[] waves, DifficultyManager difficultyManager, GameObject defaultSpawnPoint)
        {
            _enemyFactory = EnemyFactory.Instance;

            _waves = waves;
            _difficultyManager = difficultyManager;
            _defaultSpawnPoint = defaultSpawnPoint;

            _currentWave = new CurrentWave(0, _waves.First());
        }

        // public
        public void ExecuteCurrentWave()
        {
            if (Ended) return;
            if (_currentWave.IsFullyCreated) return;
            if (_currentWave.Delaying) return;
            Debug.Log($"当前敌人进度： {_currentWave.Index} / {_waves.Length}  {_difficultyManager.Difficulty}");
            // 循环生成每一波的敌人
            foreach (var obj in _currentWave.Definition.Sets)
            {
                if (_currentWave.IsSetFullyCreated(obj))
                    continue;

                var enemyGO = CreateEnemy(obj);
                if (enemyGO != null)
                {
                    var enemy = enemyGO.GetComponent<Enemy>();

                    // 设置移动模式
                    SetMovementMode(enemy, obj);
                    // 生成成功后，记录已经成功的数量
                    _currentWave.AddEnemyCreate(enemy, obj);
                }
            }
        }

        public bool MoveNext()
        {
            var nextWaveIndex = _currentWave.Index + 1;
            Ended = nextWaveIndex >= _waves.Length;
            // 生成6波 升级技能后，将下标置0，才能重新生成 Enemy
            if (Ended){
                Ended = false;
                return false;
            }
                
            _currentWave = new CurrentWave(nextWaveIndex, _waves[nextWaveIndex]);
            return true;
        }

        // helper
        private void SetMovementMode(Enemy enemy, EnemyWaveSet set)
        {
            // 设置移动模式
            enemy.Spawn(_difficultyManager.Difficulty, set.ToEnemyMode());
        }

        private GameObject CreateEnemy(EnemyWaveSet set)
        {
            if (_difficultyManager.CanCreateEnemy(set.Mode))
            {
                _difficultyManager.NotifyEnemyTypeSelected(set.EnemyType, set.Mode);
                var position = ScreenHelper.GetRandomScreenPoint(y: _defaultSpawnPoint.transform.position.y);
                return _enemyFactory.Create(set.EnemyType, position);
            }
            return null;
        }
    }

    internal class CurrentWave
    {
        private readonly Dictionary<EnemyWaveSet, List<Enemy>> _enemies = new Dictionary<EnemyWaveSet, List<Enemy>>();
        private readonly float _creationTime = Time.time;
        private readonly int _totalEnemies;

        public int Index { get; }
        public int EnemiesCreated { get; private set; }
        public bool IsFullyCreated => EnemiesCreated >= _totalEnemies;
        public bool Ended => IsFullyCreated && _enemies.All(kv => kv.Value.All(e => e?.Destroyed ?? true));
        public bool Delaying => _creationTime + Definition.Delay > Time.time;
        public EnemyWave Definition { get; }

        public CurrentWave(int index, EnemyWave definition)
        {
            Index = index;
            Definition = definition;

            _totalEnemies = definition.Sets.Sum(s => s.EnemyCount);

            foreach (var set in Definition.Sets)
            {
                if (set.IsInverted)
                    InvertPath(set);
            }
        }

        public bool IsSetFullyCreated(EnemyWaveSet set) => _enemies.ContainsKey(set) && _enemies[set].Count >= set.EnemyCount;


        private void InvertPath(EnemyWaveSet set)
        {
            // TODO: Improve to dont create many multiple inverted paths unecessarily

            var invertedPath = GameObject.Instantiate(set.Path);
            var creator = invertedPath.GetComponent<PathCreator>();

            var totalPoints = creator.bezierPath.NumPoints;
            var points = totalPoints / 2;
            for (int i = 0; i < points; i++)
            {
                var i2 = totalPoints - i - 1;
                var pt1 = creator.bezierPath[i];
                var pt2 = creator.bezierPath[i2];

                creator.bezierPath.SetPoint(i, pt2, true);
                creator.bezierPath.SetPoint(i2, pt1, true);
            }
            creator.bezierPath.NotifyPathModified();

            set.Path = invertedPath;
        }

        public void AddEnemyCreate(Enemy enemy, EnemyWaveSet set)
        {
            EnemiesCreated++;

            if (!_enemies.ContainsKey(set))
                _enemies.Add(set, new List<Enemy>());

            _enemies[set].Add(enemy);
        }
    }
}