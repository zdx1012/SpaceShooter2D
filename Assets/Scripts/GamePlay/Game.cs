﻿using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public class Game
    {
        private PlayerController _player;
        private static Game _game = null;

        public int currentGameLevel = 0;
        public int totalGameLevel = 0;

        public int Score { get; private set; }
        public int EnemiesKilled { get; private set; }
        public PlayerController Player => _player ?? (_player = GameObject.FindGameObjectWithTag(ObjectTags.Player).GetComponent<PlayerController>());
        public static Game Current => _game ?? (_game = new Game());

        private Game()
        {
        }

        public void ReadPlayerInfoData(){
            Current.Score = PlayerInfo.instance.GetScore();
            Current.Player.Health = PlayerInfo.instance.GetHealthy();
            Current.Player.ShootingPower = PlayerInfo.instance.GetShootingPower();
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
