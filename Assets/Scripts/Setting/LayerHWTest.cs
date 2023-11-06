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
    }

    public override void Run()
    {
        for (int i = 0; i < images.Length - 1; i++)
        {
            images[i].sprite = unCheckSprite;
        }
        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            GameSetting.Instance.showLayer(GameSetting.Instance.List);
        }
        Debug.Log(InputUtil.instance.GetVerticalAxis() +  "---" + InputUtil.instance.GetHorizontalAxis());
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


        
    }

    public override void SelectItem()
    {
        
    }

}
