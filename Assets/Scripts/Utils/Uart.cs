using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uart
{
    private static volatile Uart Instance;
    private static readonly object lockObject = new object();
    public System.String msg1 = "", msg2 = "";
    private static AndroidJavaObject jo = null;

    public int[] adValue = new int[4];
    public int[] errorValue = new int[10];

    const int CMD2_Key = 0;                // 发送按键
    const int CMD2_Ad = 1;                 // Ad
    const int CMD2_Led = 2;                // led 输出，，[0-3],, 高有效，低无效    byte[0]: 序号，byte[1]:高低
    const int CMD2_SSR = 3;                // ssr 输出，，[0-1],, 高有效，低无效    byte[0]: 序号，byte[1]:高低
    const int CMD2_Dq = 4;                 // 打枪 输出，，[0-1],, 高有效，低无效    byte[0]: 序号，byte[1]:高低
    const int CMD2_Sound = 5;              // 声音开关 0:关   1：开
    const int CMD2_DqZD = 6;               // 打枪启动连续震动开关 0:停止 1:启动

    byte AccountAdd;

    const int CMD2_Credit = 7;             // 账目   byte[0],byte[1]:玩家1账目 ,byte[2],byte[3]:玩家2账目 ,
    const int CMD2_Credit_Sub = 8;         // 减账目   byte[0]: 序号(取值1 - 100)，byte[1]:玩家1账目要减值，byte[2]:玩家2账目要减值
    const int CMD2_Credit_Clear = 9;       // 账目清0


    const int CMD2_Out1 = 4;                 // 打枪 输出，，[0-1],, 高有效，低无效    byte[0]: 序号，byte[1]:高低
    const int CMD2_Out2 = 2;                // led 输出，，[0-3],, 高有效，低无效    byte[0]: 序号，byte[1]:高低

    const int CMD2_BUYUGAN = 12;           // byte[0]: 玩家， byte[1]:0:停止   1：启动 ，byte[2]: 脉冲有效时间(1 - 127)，byte[3]: 脉冲无效时间(1 - 127)


    public static Uart GetInstance()
    {
        if (Instance == null)
        {
            // 双重检查锁定
            lock (lockObject)
            {
                if (Instance == null)
                {
                    Instance = new Uart();
                }
            }
            Debug.Log("*--------------4545454");
            try
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
            catch (System.Exception)
            {
                Debug.LogError("没有找到安卓设备\nInit'd AndroidJavaClass with null ptr!");
            }
            //max = jo.Call<int>("Max", new object[] { 213, 666 });
            if (jo != null)
            {
                int portno = 0; //串口    (如果改串口0把这个值改为0即可)
                int baudrate = 115200;  //波特率
                Debug.Log("zzz n = " + jo.Call<int>("GetMinN", new object[] { 666, 888 }));

                jo.Call("OpenSerialPort", new object[] { portno, baudrate });
                // Debug.Log("CALL OpenSerialPort ok;");

                //水枪板
                jo.Call("SetKey", new object[] { 1152, 3648 });
                // Debug.Log("call SetKey ok;");
                jo.Call("setVERIFY_CODE", 0x9B);
                // Debug.Log("call setVERIFY_CODE ok;");

                Instance.SendSound(true);


                Config.isAndroid = true;
            }
            else
            {
                Config.isAndroid = false;
            }
        }
        return Instance;
    }


    /// <summary>
    /// 减账目
    /// </summary>
    /// <param name="player">要更改的玩家序号 0/1</param>
    /// <param name="coin">玩家账目要减值(币数)</param>
    public void SendAccount(int player, int coin)
    {
        //序号(取值1 - 100)
        if (AccountAdd < 1 ||
            AccountAdd > 100)
        {
            AccountAdd = 1;
        }
        byte[] buf = new byte[3];
        buf[0] = AccountAdd++;
        if (player == 0)
        {
            buf[1] = (byte)coin;
            buf[2] = (byte)0;
        }
        else
        {
            buf[1] = (byte)0;
            buf[2] = (byte)coin;
        }
        Uart.GetInstance().Send(CMD2_Credit_Sub, buf, buf.Length);
        Uart.GetInstance().Send(CMD2_Credit_Sub, buf, buf.Length);

        if (!Config.isAndroid)
        {
            //模拟扣币
            // FjData.GetInstance().HwCoin[player] -= coin;
        }
    }

    /// <summary>
    /// 清空账目(清除单片机上的保存币数)
    /// </summary>
    public void SendClearAccount()
    {
        for (int i = 0; i < 3; i++)
        {
            Uart.GetInstance().Send(CMD2_Credit_Clear, null, 0);
        }
    }

    /// <summary>
    /// 接收单片机数据
    /// </summary>
    /// <returns></returns>
    private byte[] Receive()
    {
        if (jo == null) return null;

        byte[] result = jo.Call<byte[]>("CheckCmd");
        return result;
    }

    public void Run()
    {
        Debug.Log("Run -----");
        if (jo != null)
        {
            byte[] data;
            do
            {
                data = Receive();
                if (1 == data[0])
                {
                    // Debug.Log("data = " + string.Join(", ", data));
                    switch (data[1])
                    {
                        case CMD2_Key: //按键
                            msg1 = "";
                            for (int i = 0; i < 5; i++)
                            {
                                msg1 += data[i + 2] + "\r ";
                            }
                            msg1 += "\n";
                            // Debug.Log("msg1 = " + msg1);
                            InputUtil.instance.SetKey(data);
                            break;
                        // case CMD2_Ad:  //4个定位器  校准
                        //     msg2 = "";
                        //     for (int i = 0; i < 8; i++)
                        //     {
                        //         //msg2 += data[i + 2] + "\r\n";
                        //     }

                        //     adValue[1] = (data[2] << 7) | (data[3] << 0);
                        //     adValue[0] = (data[4] << 7) | (data[5] << 0);
                        //     adValue[3] = (data[6] << 7) | (data[7] << 0);
                        //     adValue[2] = (data[8] << 7) | (data[9] << 0);

                        //     for (int i = 0; i < 4; i++)
                        //     {
                        //         msg2 += adValue[i] + "\r ";
                        //     }
                        //     // Debug.Log("msg2 = " + msg2);
                        //     break;
                        case CMD2_Credit:  //币数

                            int coin1 = 0;
                            coin1 = (int)(data[2] << 7);
                            coin1 |= (int)(data[3] << 0);

                            int coin2 = 0;
                            coin2 = (int)(data[4] << 7);
                            coin2 |= (int)(data[5] << 0);

                            Debug.Log("coin1 = " + coin1 + " coin2= " + coin2);

                            InputUtil.instance.SetCoinNum(coin1);
                            // FjData.GetInstance().HwCoin[0] = coin1;
                            // FjData.GetInstance().HwCoin[1] = coin2;
                            break;
                            // default:
                            //     break;
                    }
                }
            } while (1 == data[0]);


        }

        //更新按键数据
        InputUtil.instance.Run();

    }


    /// <summary>
    /// 发送数据给单片机
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="buf"></param>
    /// <param name="len"></param>
    public void Send(byte cmd, byte[] buf, int len)
    {
        if (jo == null) return;
        jo.Call("SendCmd", new object[] { cmd, buf, len });
    }

    public string GetStr()
    {
        string str = "";
        for (int i = 0; i < errorValue.Length; i++)
        {
            str += i + "- " + errorValue[i] + "\r\n";
        }
        return str;
    }

    /// <summary>
    /// OUT1   , 暂时用于摇摇车右转
    /// </summary>
    /// <param name="power"> 开,关</param>
    public void SendOut1(bool power)
    {
        byte[] buf = new byte[2];
        buf[0] = 0;
        buf[1] = (byte)(power ? 1 : 0);
        Send(CMD2_Out1, buf, buf.Length);
    }

    /// <summary>
    /// OUT2   , 暂时用于摇摇车左转
    /// </summary>
    /// <param name="power"> 开,关</param>
    public void SendOut2(bool power)
    {
        byte[] buf = new byte[2];
        buf[0] = 0;
        buf[1] = (byte)(power ? 1 : 0);
        Send(CMD2_Out2, buf, buf.Length);
    }

    /// <summary>
    /// 打枪震动
    /// </summary>
    /// <param name="player">玩家 0, 1 </param>
    /// <param name="power"> 开,关</param>
    public void SendDq(byte player, byte power)
    {
        byte[] buf = new byte[2];
        buf[0] = player;
        buf[1] = power;
        Uart.GetInstance().Send(CMD2_Dq, buf, buf.Length);
    }

    /// <summary>
    /// 灯光控制
    /// </summary>
    /// <param name="player">玩家编号 0 , 1 </param>
    /// <param name="isOpen"> 开,关 </param>
    public void SendLed(byte player, bool isOpen)
    {
        byte[] buf = new byte[2];
        buf[0] = player;
        if (isOpen)
        {
            buf[1] = 1;
        }
        else
        {
            buf[1] = 0;
        }
        Uart.GetInstance().Send(CMD2_Led, buf, buf.Length);
    }

    /// <summary>
    /// 声音控制
    /// </summary>
    /// <param name="isOpen"> 开关 </param>
    public void SendSound(bool isOpen)
    {
        byte[] buf = new byte[1];
        if (isOpen)
        {
            buf[0] = (byte)1;
        }
        else
        {
            buf[0] = (byte)0;
        }
        Uart.GetInstance().Send(CMD2_Sound, buf, buf.Length);
    }

    /// <summary>
    /// 退礼电机马达
    /// </summary>
    /// <param name="isOpen"> 开关</param>
    public void SendSSR(bool isOpen)
    {
        Debug.Log("SendSSR = " + isOpen);
        byte[] buf = new byte[2];
        buf[0] = 0;
        if (isOpen)
        {
            buf[1] = 1;
        }
        else
        {
            buf[1] = 0;
        }
        Send(CMD2_SSR, buf, buf.Length);
    }

    /// <summary>
    /// 鱼上钩
    /// </summary>
    /// <param name="player">玩家 0或者1</param>
    /// <param name="power">是否上钩 0否1是</param>
    public void SendHook(byte player, byte power)
    {
        SendFishGun(player, power, 7, 10);
    }

    /// <summary>
    /// 控制钓鱼摇杆力度   推荐值 小鱼On:7 Off:10  大鱼On:40 Off:50
    /// </summary>
    /// <param name="player">玩家编号      0    1</param>
    /// <param name="power">打开或关闭     0    1</param>
    /// <param name="delayOn">脉冲开间隔    0   127 </param>
    /// <param name="delayOff">脉冲关间隔   0   127 </param>
    public void SendFishGun(byte player, byte power, byte delayOn, byte delayOff)
    {
        byte[] buf = new byte[4];
        buf[0] = player;
        buf[1] = power;
        buf[2] = delayOn;
        buf[3] = delayOff;
        Send(CMD2_BUYUGAN, buf, buf.Length);
    }

    /// <summary>
    /// 电鱼震动
    /// </summary>
    /// <param name="player">玩家 0或者1</param>
    /// <param name="power">是否使用技能 0否1是</param>
    public void SendSkill(byte player, byte power)
    {
        //Debug.Log("震动：" + power.ToString());
        byte[] buf = new byte[2];
        buf[0] = player;
        buf[1] = power;
        Send(CMD2_Led, buf, buf.Length);
    }

}
