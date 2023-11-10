using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
// using System.ComponentModel;
using System;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GamePlay
{
    public static class TransformExtensions
    {
        public static Vector3? MoveFromAxis(this Transform transform, float speed)
        {
            return null;
        }

        public static Vector2 EnsurePositionInScreenBoundaries(this Transform transform, Vector2 pos)
        {
            var cameraBoundaryX = ScreenHelper.GetOrthographicXRate() - transform.localScale.x / 2;
            var cameraBoundaryY = Camera.main.orthographicSize - transform.localScale.y / 2;

            pos.x = Mathf.Clamp(pos.x, cameraBoundaryX * -1, cameraBoundaryX);
            pos.y = Mathf.Clamp(pos.y, cameraBoundaryY * -1, cameraBoundaryY);
            return pos;
        }
    }

    public class ScreenHelper
    {
        public static float GetOrthographicXRate()
        {
            return Mathf.Abs(Screen.width / (float)Screen.height) * Camera.main.orthographicSize;
        }

        public static float GetRandomBoundaryX()
        {
            return Random.Range(GetOrthographicXRate() * -1, GetOrthographicXRate());
        }

        public static Vector3 GetRandomScreenPoint(float? x = null, float? y = null)
        {
            x = x ?? GetRandomBoundaryX();
            y = y ?? Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize);

            return new Vector3((float)x, (float)y, 0);
        }
    }

    public static class GameObjectExtensions
    {
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : UnityEngine.Component
        {
            var t = parent.transform;
            foreach (Transform tr in t)
            {
                if (tr.tag == tag)
                    return tr.GetComponent<T>();
            }
            return null;
        }

        public static GameObject[] FindComponentsInChildWithTag(this GameObject parent, string tag)
        {
            return parent.GetComponentsInChildren<UnityEngine.Component>()
                         .Where(c => c.tag == ObjectTags.BulletPoints)
                         .Select(c => c.gameObject)
                         .ToArray();
        }
    }

    public class LocalConfig
    {

        private static LocalConfig _instance = null;
        public static LocalConfig instance => _instance ?? (_instance = new LocalConfig());

        public GameConfig gameConfig;
        private static string fileName = "GameConfig.json";
        string saveFilePath = Path.Combine(Application.persistentDataPath, fileName);


        public LocalConfig()
        {

            // 检查本地是否存在游戏配置文件
            if (File.Exists(saveFilePath))
            {
                // 如果存在，从文件中加载游戏配置
                LoadGameConfig();
            }
            else
            {
                // 如果不存在，使用默认的游戏配置并保存为 JSON 文件
                SaveGameConfig();
            }
        }
        public void ReLoadConfig()
        {
            LoadGameConfig();
        }


        public void DefaultGameConfig()
        {
            gameConfig = new GameConfig(0, 0, 0, 0, false, 0, 0, 0, false, false, 0, 0);
            SaveGameConfig();
        }

        public void SaveGameConfig()
        {
            if (gameConfig == null)
            {
                gameConfig = new GameConfig(0, 0, 0, 0, false, 0, 0, 0, false, false, 0, 0);
            }

            // 序列化游戏配置为 JSON 字符串
            string json = JsonConvert.SerializeObject(gameConfig);
            Debug.Log("json=" + json);

            // 写入 JSON 字符串到文件
            File.WriteAllText(saveFilePath, json);

            Debug.Log("Game config saved to file: " + saveFilePath);
            LoadGameConfig();
        }

        // 从 JSON 文件中加载游戏配置
        private void LoadGameConfig()
        {
            // 从文件中读取 JSON 字符串
            string json = File.ReadAllText(saveFilePath);

            // 反序列化 JSON 字符串为游戏配置对象
            gameConfig = JsonConvert.DeserializeObject<GameConfig>(json);

            Debug.Log("Game config loaded from file: " + saveFilePath);
            Debug.Log(" 兑换礼品所需分数： " + gameConfig.GetGiftScore());
            Debug.Log(" 一次出礼品个数： " + gameConfig.GetGiftCount());
            Debug.Log(" 礼品可否累加： " + gameConfig.GetBoolGiftAdd());
            Debug.Log(" 初始生命值： " + gameConfig.GetHealthy());
            Debug.Log(" 1币几分: " + gameConfig.GetCoinValue());
            Debug.Log(" 几分一玩： " + gameConfig.GetValueGame());
        }


    }

    public class PlayerInfo
    {

        private static PlayerInfo _instance = null;
        public static PlayerInfo instance => _instance ?? (_instance = new PlayerInfo());

        private int Health = 1;
        private int Score = 0;
        private int ShootingPower = 1;

        public bool hasUpdate = false;

        public void SaveData(int healthy, int score, int shootingPower)
        {
            Health = healthy;
            Score = score;
            ShootingPower = shootingPower;
            hasUpdate = true;
        }

        public int GetHealthy()
        {
            hasUpdate = false;
            return Health;
        }
        public int GetScore()
        {
            return Score;
        }
        public int GetShootingPower()
        {
            return ShootingPower;
        }

        public void Reset()
        {
            Health = 1;
            Score = 0;
            ShootingPower = 1;
            hasUpdate = false;
        }
    }


    public enum GameState
    {
        None,
        Start,
        Success,
        GameOver,
        Contiunue,
        Gift,
        NoGift,

    }

    public enum CurrentGameScene
    {
        [System.ComponentModel.Description("Init")]
        Init,
        [System.ComponentModel.Description("Part1")]
        Part1,
        [System.ComponentModel.Description("Part2")]
        Part2,
        [System.ComponentModel.Description("Part3")]
        Part3,
        [System.ComponentModel.Description("SettingMain")]
        SettingMain
    }

    static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}