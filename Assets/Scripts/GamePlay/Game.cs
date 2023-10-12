using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public class Game
    {
        private PlayerController _player;
        private static Game _game = null;

        public int Score { get; private set; }
        public int EnemiesKilled { get; private set; }
        public PlayerController Player => _player ?? (_player = GameObject.FindGameObjectWithTag(ObjectTags.Player).GetComponent<PlayerController>());
        public static Game Current => _game ?? (_game = new Game());

        private Game()
        {
        }

        public void ReadHistroyData(){
            Debug.Log("start get playerPrefs");
            int historyScore = PlayerPrefs.GetInt(SavePlayerDataType.Score.ToString(), 0);
            int historyHealthy = PlayerPrefs.GetInt(SavePlayerDataType.Healthy.ToString(), 0);
            int historyShootingPower = PlayerPrefs.GetInt(SavePlayerDataType.ShootingPower.ToString(), 0);
            float historyShieldPower = PlayerPrefs.GetFloat(SavePlayerDataType.ShieldPower.ToString(), 0);
            Debug.Log($"zzz Games Read historyScore = {historyScore}, historyHealthy = {historyHealthy}, historyShootingPower = {historyShootingPower}, historyShieldPower = {historyShieldPower}");
            if (historyScore > 0)  Current.Score = historyScore;
            Debug.Log("set historyScore success ");
            if (historyHealthy > 0) Current.Player.Health = historyHealthy;
            Debug.Log("set historyHealthy success ");
            if (historyShootingPower > 0) Current.Player.ShootingPower = historyShootingPower;
            Debug.Log("set historyShootingPower success ");
            if (historyShieldPower > 0) Current.Player.ShieldPower = historyShieldPower;
        }

        public void StartNew()
        {
            Score = 0;
            EnemiesKilled = 0;
            _player = null;
        }

        public void EnemyKilled(Enemy enemy)
        {
            Score += enemy.ScoreValue;
            EnemiesKilled++;
        }
    }
}
