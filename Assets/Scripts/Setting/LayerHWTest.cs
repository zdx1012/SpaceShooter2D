using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerHWTest : SettingLayers
{
    private Image[] images;
    private int curSelectIndex;
    private Sprite checkSprite;
    private Sprite unCheckSprite;

    private int tmpCoinNum = 0;
    private int giftShowCount = 0;
    private int CoinShowCount = 0;
    public override void Init()
    {
        checkSprite = GameSetting.Instance.CheckedSprite;
        unCheckSprite = GameSetting.Instance.UnCheckedSprite;

        Transform transform = base.transform.Find("Item").transform;
        images = new Image[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            images[i] = transform.GetChild(i).GetComponent<Image>();
        }

        tmpCoinNum = GameData.Instance.GetCurrentGameCoin();
    }

    public override void Run()
    {
        for (int i = 0; i < images.Length - 1; i++)
        {
            if (i == 5 && images[i].sprite == checkSprite && CoinShowCount <= 60)
            {
                CoinShowCount += 1;
                if (CoinShowCount > 60) { CoinShowCount = 0; images[i].sprite = unCheckSprite; }
                continue;
            }
            else if (i == 6 && images[i].sprite == checkSprite && giftShowCount <= 60)
            {
                giftShowCount += 1;
                if (giftShowCount > 60) { giftShowCount = 0; images[i].sprite = unCheckSprite; }
                continue;
            }

            images[i].sprite = unCheckSprite;

        }
        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            GameSetting.Instance.showLayer(GameSetting.Instance.List);
        }
        Debug.Log(InputUtil.instance.GetVerticalAxis() + "---" + InputUtil.instance.GetHorizontalAxis());
        if (InputUtil.instance.GetVerticalAxis() > 0.1f)
        {
            images[0].sprite = checkSprite;
        }
        else if (InputUtil.instance.GetVerticalAxis() < -0.1f)
        {
            images[1].sprite = checkSprite;
        }
        if (InputUtil.instance.GetHorizontalAxis() > 0.1f)
        {
            images[3].sprite = checkSprite;
        }
        if (InputUtil.instance.GetHorizontalAxis() < -0.1f)
        {
            images[2].sprite = checkSprite;
        }
        if (InputUtil.instance.IsStartPressed())
        {
            images[4].sprite = checkSprite;
        }
        if (tmpCoinNum != GameData.Instance.GetCurrentGameCoin())
        {
            Debug.Log("tmpCoinNum=" + tmpCoinNum + "GetCurrentGameCoin = " + GameData.Instance.GetCurrentGameCoin());
            tmpCoinNum = GameData.Instance.GetCurrentGameCoin();
            images[5].sprite = checkSprite;
        }
        if (InputUtil.instance.IsGiftPressed())
        {
            images[6].sprite = checkSprite;
        }

    }

    public override void SelectItem()
    {

    }

}
