using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerAccountInfo : SettingLayers
{
    private Image[] images;
    private Text[] texts;

    private int curSelectIndex = -1;

    public override void Init()
    {
        int textCount = base.transform.Find("TextObject").transform.childCount;
        texts = new Text[textCount];
        Debug.Log("textCount = " + textCount);
        for (int i = 0; i < textCount; i++)
        {
            texts[i] = base.transform.Find("TextObject").transform.GetChild(i).Find("Image").GetComponentInChildren<Text>();
        }

        int imageCount = base.transform.Find("Item").transform.childCount;
        SetText();

        images = new Image[imageCount];
        for (int i = 0; i < imageCount; i++)
        {
            images[i] = base.transform.Find("Item").transform.GetChild(i).GetComponent<Image>();
            images[i].sprite = GameSetting.Instance.UnCheckedSprite;

        }
    }

    public override void Run()
    {
        if (InputUtil.instance.IsSettingDownOnceClicked() || InputUtil.instance.IsSettingRightOnceClicked())
        {
            curSelectIndex += 1;
            if (curSelectIndex > images.Length - 1) curSelectIndex = 0;
        }
        if (InputUtil.instance.IsSettingUpOnceClicked() || InputUtil.instance.IsSettingLeftOnceClicked())
        {
            curSelectIndex -= 1;
            if (curSelectIndex < 0) curSelectIndex = images.Length - 1;
        }
        SelectItem();


        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            switch (curSelectIndex)
            {
                case 0:
                    ConfirmCallBack callBack = delegate
                    {
                        GamePlayerPrefs.Instance.ResetCurrentData();
                        SetText();
                    };
                    GameSetting.Instance.SetConfirmCallBack(callBack);
                    break;
                case 1:
                    ConfirmCallBack callBack1 = delegate
                        {
                            GamePlayerPrefs.Instance.ResetCurrentData();
                            GamePlayerPrefs.Instance.ResetHistoryData();
                            SetText();
                        };
                    GameSetting.Instance.SetConfirmCallBack(callBack1);
                    break;
                case 2:
                    ConfirmCallBack callBack2 = delegate
                        {
                            GameSetting.Instance.showLayer(GameSetting.Instance.List);
                        };
                    GameSetting.Instance.SetConfirmCallBack(callBack2);
                    break;
            }
        }

    }

    public override void SelectItem()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i == curSelectIndex)
            {
                images[i].sprite = GameSetting.Instance.CheckedSprite;
            }
            else
            {
                images[i].sprite = GameSetting.Instance.UnCheckedSprite;
            }
        }
    }


    private void SetText()
    {
        Debug.Assert(texts.Length == 4);
        texts[0].text = GamePlayerPrefs.Instance.GetCurrentInsertCoin().ToString();
        texts[1].text = GamePlayerPrefs.Instance.GetHistoryInsertCoin().ToString();
        texts[2].text = GamePlayerPrefs.Instance.GetCurrentReturnGift().ToString();
        texts[3].text = GamePlayerPrefs.Instance.GetHistoryReturnGift().ToString();
    }

}
