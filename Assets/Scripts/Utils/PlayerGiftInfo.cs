public class PlayerGiftInfo
{
    public int GetCount; // 获取可退礼数量
    public int ReturnCount; // 已申请退礼数量
    public int OkCount; // 已经退礼成功数量
    public int ErrorCount; // 退礼失败数量

    public PlayerGiftInfo()
    {
        GetCount = 0;
        ReturnCount = 0;
        OkCount = 0;
        ErrorCount = 0;
    }
}