using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.PackageManager;
using UnityEngine;
using System.Reflection;


public class Config
{
    public static bool isAndroid = Application.platform == RuntimePlatform.Android;
}

// 单片机的按键
public static class AllKey
{
    public static int KeyStart = 11; // 开始键
    public static int KeyUp = 12; // 上
    public static int KeyDown = 13; // 下
    public static int KeyLeft = 14; // 左
    public static int KeyRight = 22; // 右

    public static int keySettingCenter = 7; // 设置-中
    public static int keySettingUp = 29; // 设置-上
    public static int keySettingDown = 9; // 设置-下
    public static int keySettingLeft = 8; // 设置-左
    public static int keySettingRight = 38; // 设置-右
}


// 单片机的按键与键盘上的键的映射关系
public class KeyToKeyCode
{
    public Dictionary<int, UnityEngine.KeyCode> keyMap;

    public KeyToKeyCode()
    {
        keyMap = new Dictionary<int, KeyCode>();
        keyMap[AllKey.KeyStart] = KeyCode.P;
        keyMap[AllKey.KeyUp] = KeyCode.Y;
        keyMap[AllKey.KeyDown] = KeyCode.U;
        keyMap[AllKey.KeyLeft] = KeyCode.I;
        keyMap[AllKey.KeyRight] = KeyCode.O;
        keyMap[AllKey.keySettingCenter] = KeyCode.Q;
        keyMap[AllKey.keySettingUp] = KeyCode.W;
        keyMap[AllKey.keySettingDown] = KeyCode.S;
        keyMap[AllKey.keySettingLeft] = KeyCode.A;
        keyMap[AllKey.keySettingRight] = KeyCode.D;
    }

}

public class InputUtil
{

    private Dictionary<int, bool> keyStatus;// 当前帧的状态
    private Dictionary<int, bool> keyLastStatus; // 上一帧的状态

    private Dictionary<int, int> keyPressedFrame; // 按下的帧数

    private float stepAxis = 0.1f;
    private float xAxis = 0.0f;
    private float yAxis = 0.0f;


    /// <summary>
    /// 投入总币数
    /// </summary>
    private int CoinTotalNum;

    /// <summary>
    /// 当前可用币数
    /// </summary>
    private int CoinCurrentNum;

    UInt32 KEY_Old;
    UInt32 KEY_Down;
    private static InputUtil _instance = null;
    public static InputUtil instance => _instance ?? (_instance = new InputUtil());
    // Start is called before the first frame update

    private Type allKeyType;
    private FieldInfo[] fields;
    private KeyToKeyCode keyToKey;

    private InputUtil()
    {
        Debug.Log("excute new InputUtil");
        keyToKey = new KeyToKeyCode();
        keyStatus = new Dictionary<int, bool>();
        keyLastStatus = new Dictionary<int, bool>();
        keyPressedFrame = new Dictionary<int, int>();

        // 获取AllKey类的Type
        allKeyType = typeof(AllKey);
        // 获取AllKey类中的所有字段
        fields = allKeyType.GetFields(BindingFlags.Public | BindingFlags.Static);
        // 遍历字段并输出它们的名称和值
        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name;
            int keyName = (int)field.GetValue(404);
            keyStatus[keyName] = false;
            keyLastStatus[keyName] = false;
            keyPressedFrame[keyName] = 0;
        }

        ClearKey();
    }

    private void ClearKey()
    {
        KEY_Old = 0xffffffff;
        KEY_Down = 0;
    }


    public void SetKey(byte[] keyvalue)
    {
        UInt32 key;
        key = keyvalue[2];
        key <<= 7;
        key |= keyvalue[3];
        key <<= 7;
        key |= keyvalue[4];
        key <<= 7;
        key |= keyvalue[5];
        key <<= 7;
        key |= keyvalue[6];
        KEY_Down |= ((KEY_Old ^ key) & KEY_Old);
        KEY_Old = key;
        // Debug.Log("key = " + key + " KEY_Down = " + KEY_Down);
        // for (int i = 0; i < 39; i++)
        // {
        //     if (IOStatue(i) == 0)
        //     {
        //         Debug.Log(i + " pressed");
        //     }
        // }
    }

    public void Run()
    {
        // 遍历字段并输出它们的名称和值
        int keyName = 0;
        foreach (FieldInfo field in fields)
        {
            keyName = (int)field.GetValue(404);
            keyLastStatus[keyName] = keyStatus[keyName];
            keyStatus[keyName] = IsKeyPressed(keyName);
            if (keyStatus[keyName])
            {
                keyPressedFrame[keyName] += 1;
                if (keyPressedFrame[keyName] > 1000)
                {
                    keyPressedFrame[keyName] = 1000;
                }
            }
            else
            {
                keyPressedFrame[keyName] = 0;
            }
        }

        // 设置X轴的偏移
        if (keyStatus[AllKey.KeyLeft] || keyStatus[AllKey.KeyRight])
        {

            if (keyStatus[AllKey.KeyRight])
            {
                xAxis += stepAxis;
                if (xAxis >= 1) xAxis = 1;
            }
            else
            {
                xAxis -= stepAxis;
                if (xAxis <= -1) xAxis = -1;
            }
        }
        else
        {
            if (xAxis > 0) xAxis -= stepAxis;
            if (xAxis < 0) xAxis += stepAxis;
            if (Math.Abs(xAxis) < stepAxis * 2) xAxis = 0;
        }


        // 设置Y轴的偏移
        if (keyStatus[AllKey.KeyUp] || keyStatus[AllKey.KeyDown])
        {

            if (keyStatus[AllKey.KeyUp])
            {
                yAxis += stepAxis;
                if (yAxis >= 1) yAxis = 1;
            }
            else
            {
                yAxis -= stepAxis;
                if (yAxis <= -1) yAxis = -1;
            }
        }
        else
        {
            if (yAxis > 0) yAxis -= stepAxis;
            if (yAxis < 0) yAxis += stepAxis;
            if (Math.Abs(yAxis) < stepAxis * 2) yAxis = 0;
        }



        // PC上 按F2键，增加币数
        if (Input.GetKeyDown(KeyCode.F2) && !Config.isAndroid)
        {
            CoinTotalNum += 1;
            CoinCurrentNum += 1;
            GameData.Instance.AddGameCoin();
        }
    }

    private int IOStatue(int no)
    {
        // 按下0，， 起来1
        no = (1 << no);
        return (int)(KEY_Old & no);
    }


    private bool IsKeyPressed(int key)
    {
        if (Config.isAndroid)
            return 0 == IOStatue(key);
        else
            return Input.GetKey(keyToKey.keyMap[key]);
    }

    public bool SetCoinNum(int n)
    {
        if (CoinTotalNum != n)
        {
            CoinTotalNum = n;
            CoinCurrentNum += 1;
            GameData.Instance.AddGameCoin();
            return true;
        }
        return false;
    }

    // 供外部使用的函数

    // 长按视作点击一次
    public bool AnyAxisPressed()
    {
        return (keyStatus[AllKey.KeyUp] && !keyLastStatus[AllKey.KeyUp]) || (keyStatus[AllKey.KeyDown] && !keyLastStatus[AllKey.KeyDown]) || (keyStatus[AllKey.KeyLeft] && !keyLastStatus[AllKey.KeyLeft]) || (keyStatus[AllKey.KeyRight] && !keyLastStatus[AllKey.KeyRight]);
    }

    // // 长按 每次都有响应
    public bool IsStartPressed()
    {
        return keyStatus[AllKey.KeyStart];
    }

    // 长按视作点击一次
    public bool IsStartOnceClicked()
    {
        return keyStatus[AllKey.KeyStart] && !keyLastStatus[AllKey.KeyStart];
    }

    public bool IsSettingCenterOnceClicked()
    {
        return keyStatus[AllKey.keySettingCenter] && !keyLastStatus[AllKey.keySettingCenter];
    }

    public bool IsSettingUpOnceClicked()
    {
        return keyStatus[AllKey.keySettingUp] && !keyLastStatus[AllKey.keySettingUp];
    }
    public bool IsSettingDownOnceClicked()
    {
        return keyStatus[AllKey.keySettingDown] && !keyLastStatus[AllKey.keySettingDown];
    }
    public bool IsSettingLeftOnceClicked()
    {
        return keyStatus[AllKey.keySettingLeft] && !keyLastStatus[AllKey.keySettingLeft];
    }
    public bool IsSettingRightOnceClicked()
    {
        return keyStatus[AllKey.keySettingRight] && !keyLastStatus[AllKey.keySettingRight];
    }


    public float GetHorizontalAxis()
    {
        return xAxis;
    }

    public float GetVerticalAxis()
    {
        return yAxis;
    }

    public int GetCoinNum() { return CoinCurrentNum; }

    /// <summary>
    /// 扣除玩家币数
    /// </summary>
    /// <param name="n">扣除数量</param>
    public void CutCoin(int n)
    {
        Debug.Log("cut coin " + n);
        CoinCurrentNum -= n;

    }

    public void SetSound(bool isOpen)
    {
        Uart.GetInstance().SendSound(isOpen);
    }

    public void SendGift(bool isOpen)
    {
        Uart.GetInstance().SendSSR(isOpen);
    }

    public bool IsSettingKeyHold()
    {
        if (keyPressedFrame[AllKey.keySettingCenter] >= 5 * 60)
        {
            keyPressedFrame[AllKey.keySettingCenter] = 0;
            return true;
        }
        return false;
    }

    public bool IO_Pressed(int no)
    {
        no = 1 << no;
        if ((KEY_Down & no) > 0)
        {
            KEY_Down &= (uint)(~no);
            return true;
        }
        return false;
    }

    public bool IsGiftPressed()
    {
        if (Config.isAndroid)
        {
            return IO_Pressed(3);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.F3);
        }
    }
}
