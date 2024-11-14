using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LayerFactory : SettingLayers
{
    public GameObject arraw;

    private Image[] imageItems;
    private Text[] textItems;
    private int curIndex;
    private bool changeValue;
    public override void Init()
    {
        Transform transform = base.transform.Find("Item").transform;
        imageItems = new Image[transform.childCount];
        textItems = new Text[transform.childCount - 1];
        for (int i = 0; i < transform.childCount; i++)
        {
            imageItems[i] = transform.GetChild(i).GetComponent<Image>();
            if (i < textItems.Length)
            {
                textItems[i] = imageItems[i].transform.Find("Image").transform.GetComponentInChildren<Text>();
            }
        }
        changeValue = false;
    }

    public override void Run()
    {
        // 上下切换选中
        if (!changeValue)
        {
            if (InputUtil.instance.IsSettingDownOnceClicked())
            {
                curIndex += 1;
                if (curIndex > imageItems.Length - 1) curIndex = 0;
            }
            else if (InputUtil.instance.IsSettingUpOnceClicked())
            {
                curIndex -= 1;
                if (curIndex < 0) curIndex = imageItems.Length - 1;
            }
        }
        // 改值
        else
        {
            if (InputUtil.instance.IsSettingDownOnceClicked())
            {
                LocalConfig.instance.gameConfig.SetLanguage(true);
            }
            else if (InputUtil.instance.IsSettingUpOnceClicked())
            {
                LocalConfig.instance.gameConfig.SetLanguage(false);
            }
        }


        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            if (curIndex == 0)
            {
                changeValue = !changeValue;
                arraw.SetActive(changeValue);
            }
            else
            {
                ConfirmCallBack callback = delegate
                {
                    LocalConfig.instance.SaveGameConfig();
                    GameSetting.Instance.showLayer(GameSetting.Instance.List);
                };
                GameSetting.Instance.SetConfirmCallBack(callback);
            }

        }
        SelectItem();
    }

    public override void SelectItem()
    {
        if (curIndex < textItems.Length) textItems[curIndex].text = LocalConfig.instance.gameConfig.GetLanguage();


        for (int i = 0; i < imageItems.Length; i++)
        {
            if (i == curIndex)
            {
                imageItems[i].sprite = GameSetting.Instance.CheckedSprite;
            }
            else
            {
                imageItems[i].sprite = GameSetting.Instance.UnCheckedSprite;
            }
        }
    }
}
