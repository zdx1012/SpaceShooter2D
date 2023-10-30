using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputPoint : MonoBehaviour
{


    private static bool isCreated = false;

    private void Awake()
    {
        if (!isCreated)
        {
            DontDestroyOnLoad(gameObject);
            isCreated = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 读取串口数据
        if (Config.isAndroid)
        {
            Uart.GetInstance().Run();
        }
        else
        {
            // 不调用，则获取不到键盘的方向键
            InputUtil.instance.Run();
        }

    }
}
