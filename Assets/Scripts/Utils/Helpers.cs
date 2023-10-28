using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
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
            return parent.GetComponentsInChildren<Component>()
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
        private string saveFilePath = "GameConfig.json";


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
        public void SaveGameConfig()
        {

            GameLevelConfig[] tmp = new GameLevelConfig[]
            {
                new GameLevelConfig(1, DifficultyLevel.Easy),
                new GameLevelConfig(2, DifficultyLevel.Normal)
            };
            gameConfig = new GameConfig(10, true, tmp);
            Debug.Log(tmp.Length);

            // 序列化游戏配置为 JSON 字符串
            string json = JsonConvert.SerializeObject(gameConfig);
            Debug.Log("json=" + json);

            // 写入 JSON 字符串到文件
            File.WriteAllText(saveFilePath, json);

            Debug.Log("Game config saved to file: " + saveFilePath);
        }

        // 从 JSON 文件中加载游戏配置
        private void LoadGameConfig()
        {
            // 从文件中读取 JSON 字符串
            string json = File.ReadAllText(saveFilePath);

            // 反序列化 JSON 字符串为游戏配置对象
            gameConfig = JsonConvert.DeserializeObject<GameConfig>(json);

            Debug.Log("Game config loaded from file: " + saveFilePath);
        }

        public void UpdateConfigHealthy(int healthy)
        {
            gameConfig.initHealth = healthy;
        }
        public void UpdateConfigGameLevel(int index, int n, DifficultyLevel difficulty)
        {
            gameConfig.data[index].EnemyCount = n;
            gameConfig.data[index].difficultyLevel = difficulty;
        }
    }
}