using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.GamePlay;

public class LayerList : SettingLayers
{

    private Image[] imageItems;
    private int curIndex = 0;
    public override void Init()
    {
        Transform transform = base.transform.Find("Item").transform;
        imageItems = new Image[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            imageItems[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }
    public override void SelectItem()
    {
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

    public override void Run()
    {
        if (InputUtil.instance.IsSettingDownOnceClicked())
        {
            curIndex += 1;
            if (curIndex > 5) curIndex = 0;
        }
        else if (InputUtil.instance.IsSettingUpOnceClicked())
        {
            curIndex -= 1;
            if (curIndex < 0) curIndex = 5;
        }
        // 显示指定的layer
        else if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            if (curIndex == 0) GameSetting.Instance.showLayer(GameSetting.Instance.GameSet);
            if (curIndex == 1) GameSetting.Instance.showLayer(GameSetting.Instance.Account);
            if (curIndex == 2) GameSetting.Instance.showLayer(GameSetting.Instance.HWTest);
            if (curIndex == 4) GameSetting.Instance.showLayer(4);
            if (curIndex == 5) SceneManager.LoadScene(CurrentGameScene.Init.GetDescription());
        };
        SelectItem();
    }
}
