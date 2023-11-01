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

    private bool StartKeyPressed;
    private bool UpKeyPressed;
    private bool DownKeyPressed;
    private bool LeftKeyPressed;
    private bool RightKeyPressed;
    private bool SettingCenterKeyPessed;
    private bool SettingUpKeyPessed;
    private bool SettingDownKeyPessed;
    private bool SettingLeftKeyPessed;
    private bool SettingRightKeyPessed;

    private float stepAxis = 0.1f;
    private float xAxis = 0.0f;
    private float yAxis = 0.0f;


    private int CoinNum;

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

        // keyStatus[KeyStart] = false;
        // keyStatus[KeyUp] = false;
        // keyStatus[KeyDown] = false;
        // keyStatus[KeyLeft] = false;
        // keyStatus[KeyRight] = false;
        // keyStatus[keySettingCenter] = false;
        // keyStatus[keySettingUp] = false;
        // keyStatus[keySettingDown] = false;
        // keyStatus[keySettingLeft] = false;
        // keyStatus[keySettingRight] = false;

        // keyLastStatus[KeyStart] = false;
        // keyLastStatus[KeyUp] = false;
        // keyLastStatus[KeyDown] = false;
        // keyLastStatus[KeyLeft] = false;
        // keyLastStatus[KeyRight] = false;
        // keyLastStatus[keySettingCenter] = false;
        // keyLastStatus[keySettingUp] = false;
        // keyLastStatus[keySettingDown] = false;
        // keyLastStatus[keySettingLeft] = false;
        // keyLastStatus[keySettingRight] = false;

        ClearKey();
    }

    private void ClearKey()
    {
        KEY_Old = 0xffffffff;
        KEY_Down = 0;
        Debug.Log("run ClearKey");
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

    // public string GetStr()
    // {
    //     string str = "";
    //     for (int i1 = 0; i1 < 31; i1++)
    //     {
    //         if (IO_Pressed(i1))
    //         {
    //             Uart.GetInstance().errorValue[0] = i1;
    //             break;
    //         }
    //     }
    //     return str;
    // }

    // public bool IO_Pressed(int no)
    // {
    //     no = (1 << no);
    //     if ((KEY_Down & no) > 0)
    //     {
    //         KEY_Down &= ~(UInt32)no;
    //         return true;
    //     }
    //     return false;
    // }

    public void Run()
    {
        // StartKeyPressed = KEY_Start_Pressed();
        // UpKeyPressed = KEY_Up_Pressed();
        // DownKeyPressed = KEY_Down_Pressed();
        // LeftKeyPressed = KEY_Left_Pressed();
        // RightKeyPressed = KEY_Right_Pressed();
        // SettingCenterKeyPessed = KEY_SettingCenter_pressed();
        // SettingUpKeyPessed = KEY_SettingUp_pressed();
        // SettingDownKeyPessed = KEY_SettingDown_pressed();
        // SettingLeftKeyPessed = KEY_SettingLeft_pressed();
        // SettingRightKeyPessed = KEY_SettingRight_pressed();


        // 设置X轴的偏移
        // if (LeftKeyPressed || RightKeyPressed)
        // {

        //     if (RightKeyPressed)
        //     {
        //         xAxis += stepAxis;
        //         if (xAxis >= 1) xAxis = 1;
        //     }
        //     else
        //     {
        //         xAxis -= stepAxis;
        //         if (xAxis <= -1) xAxis = -1;
        //     }
        // }
        // else
        // {
        //     if (xAxis > 0) xAxis -= stepAxis;
        //     if (xAxis < 0) xAxis += stepAxis;
        //     if (Math.Abs(xAxis) < stepAxis * 2) xAxis = 0;
        // }


        // 设置Y轴的偏移
        // if (UpKeyPressed || DownKeyPressed)
        // {

        //     if (UpKeyPressed)
        //     {
        //         yAxis += stepAxis;
        //         if (yAxis >= 1) yAxis = 1;
        //     }
        //     else
        //     {
        //         yAxis -= stepAxis;
        //         if (yAxis <= -1) yAxis = -1;
        //     }
        // }
        // else
        // {
        //     if (yAxis > 0) yAxis -= stepAxis;
        //     if (yAxis < 0) yAxis += stepAxis;
        //     if (Math.Abs(yAxis) < stepAxis * 2) yAxis = 0;
        // }



        // 记录上一帧的状态
        // keyLastStatus[AllKey.KeyStart] = keyStatus[AllKey.KeyStart];
        // keyLastStatus[AllKey.KeyUp] = keyStatus[AllKey.KeyUp];
        // keyLastStatus[AllKey.KeyDown] = keyStatus[AllKey.KeyDown];
        // keyLastStatus[AllKey.KeyLeft] = keyStatus[AllKey.KeyLeft];
        // keyLastStatus[AllKey.KeyRight] = keyStatus[AllKey.KeyRight];
        // keyLastStatus[AllKey.keySettingCenter] = keyStatus[AllKey.keySettingCenter];
        // keyLastStatus[AllKey.keySettingUp] = keyStatus[AllKey.keySettingUp];
        // keyLastStatus[AllKey.keySettingDown] = keyStatus[AllKey.keySettingDown];
        // keyLastStatus[AllKey.keySettingLeft] = keyStatus[AllKey.keySettingLeft];
        // keyLastStatus[AllKey.keySettingRight] = keyStatus[AllKey.keySettingRight];


        // // 记录当前帧的状态
        // keyStatus[AllKey.KeyStart] = StartKeyPressed;
        // keyStatus[AllKey.KeyUp] = UpKeyPressed;
        // keyStatus[AllKey.KeyDown] = DownKeyPressed;
        // keyStatus[AllKey.KeyLeft] = LeftKeyPressed;
        // keyStatus[AllKey.KeyRight] = RightKeyPressed;
        // keyStatus[AllKey.keySettingCenter] = SettingCenterKeyPessed;
        // keyStatus[AllKey.keySettingUp] = SettingUpKeyPessed;
        // keyStatus[AllKey.keySettingDown] = SettingDownKeyPessed;
        // keyStatus[AllKey.keySettingLeft] = SettingLeftKeyPessed;
        // keyStatus[AllKey.keySettingRight] = SettingRightKeyPessed;



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






        // 按下 设置 中心键，清空币数
        // TODO: DELETE THIS CODE
        if (SettingCenterKeyPessed && Config.isAndroid)
        {
            Uart.GetInstance().SendClearAccount();
        }
        // Debug.Log($"111 {Config.isAndroid} StartKeyPressed = {keyStatus[AllKey.KeyStart]}, UpKeyPressed = {keyStatus[AllKey.KeyUp]}, DownKeyPressed = {keyStatus[AllKey.KeyDown]}, LeftKeyPressed = {keyStatus[AllKey.KeyLeft]}, RightKeyPressed = {keyStatus[AllKey.KeyRight]}");
        // Debug.Log($"222 {Config.isAndroid} SettingCenterKeyPessed = {SettingCenterKeyPessed} SettingUpKeyPessed = {SettingUpKeyPessed} SettingDownKeyPessed = {SettingDownKeyPessed} SettingLeftKeyPessed = {SettingLeftKeyPessed} SettingRightKeyPessed = {SettingRightKeyPessed}");
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

    // private bool KEY_Start_Pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.KeyStart);
    //     else
    //         return Input.GetKey(KeyCode.P);
    // }


    // private bool KEY_Up_Pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.KeyUp);
    //     else
    //         return Input.GetKey(KeyCode.Y);
    // }

    // private bool KEY_Down_Pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.KeyDown);
    //     else
    //         return Input.GetKey(KeyCode.U);
    // }

    // private bool KEY_Left_Pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.KeyLeft);
    //     else
    //         return Input.GetKey(KeyCode.I);
    // }

    // private bool KEY_Right_Pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.KeyRight);
    //     else
    //         return Input.GetKey(KeyCode.O);
    // }

    // private bool KEY_SettingCenter_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.keySettingCenter);
    //     else
    //         return Input.GetKey(KeyCode.Q);
    // }
    // private bool KEY_SettingUp_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.keySettingUp);
    //     else
    //         return Input.GetKey(KeyCode.W);
    // }
    // private bool KEY_SettingDown_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.keySettingDown);
    //     else
    //         return Input.GetKey(KeyCode.S);
    // }
    // private bool KEY_SettingLeft_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.keySettingLeft);
    //     else
    //         return Input.GetKey(KeyCode.A);
    // }
    // private bool KEY_SettingRight_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IOStatue(AllKey.keySettingRight);
    //     else
    //         return Input.GetKey(KeyCode.D);
    // }


    public void SetCoinNum(int n)
    {
        CoinNum = n;
    }

    // 供外部使用的函数

    // 长按视作点击一次
    public bool AnyAxisPressed()
    {
        // if (Config.isAndroid)
        // {
            return (keyStatus[AllKey.KeyUp] && !keyLastStatus[AllKey.KeyUp]) || (keyStatus[AllKey.KeyDown] && !keyLastStatus[AllKey.KeyDown]) || (keyStatus[AllKey.KeyLeft] && !keyLastStatus[AllKey.KeyLeft]) || (keyStatus[AllKey.KeyRight] && !keyLastStatus[AllKey.KeyRight]);
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.O);
        // }
    }

    // // 长按 每次都有响应
    public bool IsStartPressed()
    {
        return keyStatus[AllKey.KeyStart];
    }

    // 长按视作点击一次
    public bool IsStartOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.KeyStart] && !keyLastStatus[AllKey.KeyStart];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.P);
        // }

    }

    public bool IsSettingCenterOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.keySettingCenter] && !keyLastStatus[AllKey.keySettingCenter];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.Q);
        // }
    }

    public bool IsSettingUpOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.keySettingUp] && !keyLastStatus[AllKey.keySettingUp];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.W);
        // }
    }
    public bool IsSettingDownOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.keySettingDown] && !keyLastStatus[AllKey.keySettingDown];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.S);
        // }
    }
    public bool IsSettingLeftOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.keySettingLeft] && !keyLastStatus[AllKey.keySettingLeft];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.A);
        // }
    }
    public bool IsSettingRightOnceClicked()
    {
        // if (Config.isAndroid)
        // {
            return keyStatus[AllKey.keySettingRight] && !keyLastStatus[AllKey.keySettingRight];
        // }
        // else
        // {
        //     return Input.GetKeyDown(KeyCode.D);
        // }
    }


    public float GetHorizontalAxis()
    {
        return xAxis;
    }

    public float GetVerticalAxis()
    {
        return yAxis;
    }

    public int GetCoinNum() { return CoinNum; }

    public bool IsSettingKeyHold(int second)
    {
        if (keyPressedFrame[AllKey.keySettingCenter] >= second * 60)
        {
            keyPressedFrame[AllKey.keySettingCenter] = 0;
            return true;
        }
        return false;
    }
}
