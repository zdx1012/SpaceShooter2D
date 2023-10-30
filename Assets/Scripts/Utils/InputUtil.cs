using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.PackageManager;
using UnityEngine;


public class Config
{
    public static bool isAndroid = Application.platform == RuntimePlatform.Android;
    public static bool ReadHistroyData = false;
}

public class InputUtil
{
    public int KeyStart = 11; // 开始键
    public int KeyUp = 12; // 上
    public int KeyDown = 13; // 下
    public int KeyLeft = 14; // 左
    public int KeyRight = 22; // 右

    public int keySettingCenter = 7; // 设置-中
    public int keySettingUp = 29; // 设置-中
    public int keySettingDown = 9; // 设置-中
    public int keySettingLeft = 8; // 设置-中
    public int keySettingRight = 38; // 设置-中


    private Dictionary<int, bool> keyStatus;// 当前帧的状态
    private Dictionary<int, bool> keyLastStatus; // 上一帧的状态

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

    private InputUtil()
    {
        Debug.Log("excute new InputUtil");
        keyStatus = new Dictionary<int, bool>();
        keyLastStatus = new Dictionary<int, bool>();
        keyStatus[KeyStart] = false;
        keyStatus[KeyUp] = false;
        keyStatus[KeyDown] = false;
        keyStatus[KeyLeft] = false;
        keyStatus[KeyRight] = false;
        keyStatus[keySettingCenter] = false;
        keyStatus[keySettingUp] = false;
        keyStatus[keySettingDown] = false;
        keyStatus[keySettingLeft] = false;
        keyStatus[keySettingRight] = false;

        keyLastStatus[KeyStart] = false;
        keyLastStatus[KeyUp] = false;
        keyLastStatus[KeyDown] = false;
        keyLastStatus[KeyLeft] = false;
        keyLastStatus[KeyRight] = false;
        keyLastStatus[keySettingCenter] = false;
        keyLastStatus[keySettingUp] = false;
        keyLastStatus[keySettingDown] = false;
        keyLastStatus[keySettingLeft] = false;
        keyLastStatus[keySettingRight] = false;

        ClearKey();
    }

    private void ClearKey()
    {
        KEY_Old = 0xffffffff;
        KEY_Down = 0;
        Debug.Log("run ClearKey");
    }
    // public static InputUtil GetInstance()
    // {
    //     Debug.Log($"instance is null ? {null == instance}");
    //     if (null == instance)
    //     {
    //         Debug.Log("goto new InputUtil");
    //         instance = new InputUtil();
    //     }
    //     return instance;
    // }

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
        for (int i = 0; i < 40; i++)
        {
            if (IO_Statue(i) == 0)
            {
                Debug.Log(i + " pressed");
            }
        }
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
        Debug.Log("run key update");
        StartKeyPressed = KEY_Start_Pressed();
        UpKeyPressed = KEY_Up_Pressed();
        DownKeyPressed = KEY_Down_Pressed();
        LeftKeyPressed = KEY_Left_Pressed();
        RightKeyPressed = KEY_Right_Pressed();
        SettingCenterKeyPessed = KEY_SettingCenter_pressed();
        SettingUpKeyPessed = KEY_SettingUp_pressed();
        SettingDownKeyPessed = KEY_SettingDown_pressed();
        SettingLeftKeyPessed = KEY_SettingLeft_pressed();
        SettingRightKeyPessed = KEY_SettingRight_pressed();


        // 设置X轴的偏移
        if (LeftKeyPressed || RightKeyPressed)
        {

            if (RightKeyPressed)
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
        if (UpKeyPressed || DownKeyPressed)
        {

            if (UpKeyPressed)
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



        // 记录上一帧的状态
        keyLastStatus[KeyStart] = keyStatus[KeyStart];
        keyLastStatus[KeyUp] = keyStatus[KeyUp];
        keyLastStatus[KeyDown] = keyStatus[KeyDown];
        keyLastStatus[KeyLeft] = keyStatus[KeyLeft];
        keyLastStatus[KeyRight] = keyStatus[KeyRight];
        keyLastStatus[keySettingCenter] = keyStatus[keySettingCenter];
        keyLastStatus[keySettingUp] = keyStatus[keySettingUp];
        keyLastStatus[keySettingDown] = keyStatus[keySettingDown];
        keyLastStatus[keySettingLeft] = keyStatus[keySettingLeft];
        keyLastStatus[keySettingRight] = keyStatus[keySettingRight];


        // 记录当前帧的状态
        keyStatus[KeyStart] = StartKeyPressed;
        keyStatus[KeyUp] = UpKeyPressed;
        keyStatus[KeyDown] = DownKeyPressed;
        keyStatus[KeyLeft] = LeftKeyPressed;
        keyStatus[KeyRight] = RightKeyPressed;
        keyStatus[keySettingCenter] = SettingCenterKeyPessed;
        keyStatus[keySettingUp] = SettingUpKeyPessed;
        keyStatus[keySettingDown] = SettingDownKeyPessed;
        keyStatus[keySettingLeft] = SettingLeftKeyPessed;
        keyStatus[keySettingRight] = SettingRightKeyPessed;



        // 按下 设置 中心键，清空币数
        // TODO: DELETE THIS CODE
        if (SettingCenterKeyPessed)
        {
            Uart.GetInstance().SendClearAccount();
        }
        // Debug.Log($"111 {Config.isAndroid} StartKeyPressed = {StartKeyPressed}, UpKeyPressed = {UpKeyPressed}, DownKeyPressed = {DownKeyPressed}, LeftKeyPressed = {LeftKeyPressed}, RightKeyPressed = {RightKeyPressed}");
        // Debug.Log($"222 {Config.isAndroid} SettingCenterKeyPessed = {SettingCenterKeyPessed} SettingUpKeyPessed = {SettingUpKeyPessed} SettingDownKeyPessed = {SettingDownKeyPessed} SettingLeftKeyPessed = {SettingLeftKeyPessed} SettingRightKeyPessed = {SettingRightKeyPessed}");
    }

    private int IO_Statue(int no)
    {
        // 按下0，， 起来1
        no = (1 << no);
        return (int)(KEY_Old & no);
    }


    private bool KEY_Start_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyStart);
        else
            return Input.GetKey(KeyCode.P);
    }


    private bool KEY_Up_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyUp);
        else
            return Input.GetKey(KeyCode.Y);
    }

    private bool KEY_Down_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyDown);
        else
            return Input.GetKey(KeyCode.U);
    }

    private bool KEY_Left_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyLeft);
        else
            return Input.GetKey(KeyCode.I);
    }

    private bool KEY_Right_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyRight);
        else
            return Input.GetKey(KeyCode.O);
    }

    private bool KEY_SettingCenter_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingCenter);
        else
            return Input.GetKey(KeyCode.Q);
    }
    private bool KEY_SettingUp_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingUp);
        else
            return Input.GetKey(KeyCode.W);
    }
    private bool KEY_SettingDown_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingDown);
        else
            return Input.GetKey(KeyCode.S);
    }
    private bool KEY_SettingLeft_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingLeft);
        else
            return Input.GetKey(KeyCode.A);
    }
    private bool KEY_SettingRight_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingRight);
        else
            return Input.GetKey(KeyCode.D);
    }


    public void SetCoinNum(int n)
    {
        CoinNum = n;
    }

    // 供外部使用的函数

    // 长按视作点击一次
    public bool AnyAxisPressed()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[KeyUp] && !keyLastStatus[KeyUp]) || (keyStatus[KeyDown] && !keyLastStatus[KeyDown]) || (keyStatus[KeyLeft] && !keyLastStatus[KeyLeft]) || (keyStatus[KeyRight] && !keyLastStatus[KeyRight]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.O);
        }
    }

    // // 长按 每次都有响应
    public bool isStartPressed()
    {
        return KEY_Start_Pressed();
    }

    // 长按视作点击一次
    public bool isStartOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[KeyStart] && !keyLastStatus[KeyStart]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.P);
        }

    }

    public bool isSettingCenterOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[keySettingCenter] && !keyLastStatus[keySettingCenter]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Q);
        }
    }

    public bool isSettingUpOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[keySettingUp] && !keyLastStatus[keySettingUp]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.W);
        }
    }
    public bool isSettingDownOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[keySettingDown] && !keyLastStatus[keySettingDown]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.S);
        }
    }
    public bool isSettingLeftOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[keySettingLeft] && !keyLastStatus[keySettingLeft]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.A);
        }
    }
    public bool isSettingRightOnceClicked()
    {
        if (Config.isAndroid)
        {
            return (keyStatus[keySettingRight] && !keyLastStatus[keySettingRight]);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.D);
        }
    }

    // public bool isUpPressed()
    // {
    //     if (Config.isAndroid)
    //     {
    //         return keyStatus[KeyUp];
    //     }
    //     else
    //     {
    //         return Input.GetKey(KeyCode.Y);
    //     }
    // }
    // public bool isDownPressed()
    // {
    //     if (Config.isAndroid)
    //     {
    //         return keyStatus[KeyDown];
    //     }
    //     else
    //     {
    //         return Input.GetKey(KeyCode.U);
    //     }
    // }
    // public bool isLeftPressed()
    // {
    //     if (Config.isAndroid)
    //     {
    //         return keyStatus[KeyLeft];
    //     }
    //     else
    //     {
    //         return Input.GetKey(KeyCode.I);
    //     }
    // }
    // public bool isRightPressed()
    // {
    //     if (Config.isAndroid)
    //     {
    //         return keyStatus[KeyRight];
    //     }
    //     else
    //     {
    //         return Input.GetKey(KeyCode.O);
    //     }
    // }

    // public  bool SettingCenter_pressed()
    // {
    //     if (Config.isAndroid)
    //         return 0 == IO_Statue(keySettingCenter);
    //     else
    //         return Input.GetKey(KeyCode.L);
    // }

    public float GetHorizontalAxis()
    {
        return xAxis;
    }

    public float GetVerticalAxis()
    {
        return yAxis;
    }

    public int GetCoinNum() { return CoinNum; }
}
