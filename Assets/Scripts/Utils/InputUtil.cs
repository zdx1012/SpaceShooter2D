using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Config
{
    public static bool isAndroid = true;
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



    private bool StartKeyPressed;
    private bool UpKeyPressed;
    private bool DownKeyPressed;
    private bool LeftKeyPressed;
    private bool RightKeyPressed;
    private bool SettingCenterKeyPessed;


    private int coinNum;

    UInt32 KEY_Old;
    UInt32 KEY_Down;
    public static InputUtil instance = null;
    // Start is called before the first frame update

    private InputUtil()
    {
        Debug.Log("excute new InputUtil");
        ClearKey();
    }

    private void ClearKey()
    {
        KEY_Old = 0xffffffff;
        KEY_Down = 0;
        Debug.Log("run ClearKey");
    }
    public static InputUtil GetInstance()
    {
        Debug.Log($"instance is null ? {null == instance}");
        if (null == instance)
        {
            Debug.Log("goto new InputUtil");
            instance = new InputUtil();
        }
        return instance;
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
        StartKeyPressed = KEY_Start_Pressed();
        UpKeyPressed = KEY_Up_Pressed();
        DownKeyPressed = KEY_Down_Pressed();
        LeftKeyPressed = KEY_Left_Pressed();
        RightKeyPressed = KEY_Right_Pressed();
        SettingCenterKeyPessed = KEY_SettingCenter_pressed();
        if (SettingCenterKeyPessed)
        {
            Uart.GetInstance().SendClearAccount();
        }
        Debug.Log($"111 {Config.isAndroid} StartKeyPressed = {StartKeyPressed}, UpKeyPressed = {UpKeyPressed}, DownKeyPressed = {DownKeyPressed}, LeftKeyPressed = {LeftKeyPressed}, RightKeyPressed = {RightKeyPressed}");
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
            return Input.GetKeyDown(KeyCode.KeypadEnter);
    }


    private bool KEY_Up_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyUp);
        else
            return Input.GetKeyDown(KeyCode.UpArrow);
    }

    private bool KEY_Down_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyDown);
        else
            return Input.GetKeyDown(KeyCode.DownArrow);
    }

    private bool KEY_Left_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyLeft);
        else
            return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    private bool KEY_Right_Pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(KeyRight);
        else
            return Input.GetKeyDown(KeyCode.RightArrow);
    }

    private bool KEY_SettingCenter_pressed()
    {
        if (Config.isAndroid)
            return 0 == IO_Statue(keySettingCenter);
        else
            return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public void SetCoinNum(int n)
    {
        coinNum = n;
    }

    // 给外部使用的函数

    public bool AnyKeyPressed()
    {
        return StartKeyPressed || UpKeyPressed || DownKeyPressed || LeftKeyPressed || RightKeyPressed || SettingCenterKeyPessed;
    }
    public bool isStartPressed() { return StartKeyPressed; }
    public bool isUpPressed() { return UpKeyPressed; }
    public bool isDownPressed() { return DownKeyPressed; }
    public bool isLeftPressed() { return LeftKeyPressed; }
    public bool isRightPressed() { return RightKeyPressed; }

    public int GetCoinNum() { return coinNum; }
}
