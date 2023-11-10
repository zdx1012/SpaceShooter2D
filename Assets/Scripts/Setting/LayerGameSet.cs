using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class LayerGameSet : SettingLayers
{

    public GameObject arraw;
    private Image[] imageItems;
    private Text[] textItems;

    private int curIndex;
    private bool isChangeValue;

    // Start is called before the first frame update
    public override void Init()
    {
        curIndex = 0;
        Transform transform = base.transform.Find("Item").transform;
        imageItems = new Image[transform.childCount];
        textItems = new Text[transform.childCount - 3];
        for (int i = 0; i < transform.childCount; i++)
        {
            imageItems[i] = transform.GetChild(i).GetComponent<Image>();
            if (i < textItems.Length)
            {
                textItems[i] = imageItems[i].transform.Find("Image").transform.GetComponentInChildren<Text>();
            }
        }
    }

    // Update is called once per frame
    public override void Run()
    {
        // 更改选中条目
        if (!isChangeValue)
        {
            if (InputUtil.instance.IsSettingDownOnceClicked())
            {
                curIndex += 1;
                if (curIndex > imageItems.Length - 1) curIndex = 0;
            }
            else if (InputUtil.instance.IsSettingUpOnceClicked() && !isChangeValue)
            {
                curIndex -= 1;
                if (curIndex < 0) curIndex = imageItems.Length - 1;
            }
        }
        // 改变值
        else if (isChangeValue)
        {
            int status = 0;

            if (InputUtil.instance.IsSettingDownOnceClicked()) status = 1;
            if (InputUtil.instance.IsSettingUpOnceClicked()) status = -1;
            if (status != 0)
            {
                bool isNext = status == 1 ? true : false;
                switch (curIndex)
                {
                    case 0:
                        LocalConfig.instance.gameConfig.SetCoinValue(isNext);
                        break;
                    case 1:
                        LocalConfig.instance.gameConfig.SetValueGame(isNext);
                        break;
                    case 2:
                        LocalConfig.instance.gameConfig.SetGiftAdd();
                        break;
                    case 3:
                        LocalConfig.instance.gameConfig.SetVolumeGame(isNext);
                        break;
                    case 4:
                        LocalConfig.instance.gameConfig.SetDifficulty(isNext);
                        break;
                    case 5:
                        LocalConfig.instance.gameConfig.SetWaitCoinTime(isNext);
                        break;
                    case 6:
                        LocalConfig.instance.gameConfig.SetDemoVideo();
                        break;
                    case 7:
                        LocalConfig.instance.gameConfig.SetAutoFire();
                        break;
                    case 8:
                        LocalConfig.instance.gameConfig.SetHealthy(isNext);
                        break;
                    case 9:
                        LocalConfig.instance.gameConfig.SetGiftScore(isNext);
                        break;
                    case 10:
                        LocalConfig.instance.gameConfig.SetGiftCount(isNext);
                        break;
                }
            }
        }

        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {

            if (curIndex < textItems.Length)
            {
                // 如果当前不是选中改值状态，则显示图标
                if (!isChangeValue)
                {
                    Debug.Log(imageItems[curIndex].transform.position);
                    arraw.transform.position = imageItems[curIndex].transform.position;
                }
                isChangeValue = !isChangeValue;
                arraw.transform.gameObject.SetActive(isChangeValue);
            }
            else if (curIndex == textItems.Length)
            {
                // default
                ConfirmCallBack callBack = delegate
                {
                    Debug.Log("default value");
                    LocalConfig.instance.DefaultGameConfig();
                    GameSetting.Instance.showLayer(GameSetting.Instance.List);
                };
                GameSetting.Instance.SetConfirmCallBack(callBack);
            }
            else if (curIndex == textItems.Length + 1)
            {
                // give up setting
                ConfirmCallBack callBack = delegate
                {
                    Debug.Log(" give up setting");
                    LocalConfig.instance.ReLoadConfig();
                    GameSetting.Instance.showLayer(GameSetting.Instance.List);
                };
                GameSetting.Instance.SetConfirmCallBack(callBack);
            }
            else if (curIndex == textItems.Length + 2)
            {
                // save and exit
                ConfirmCallBack callBack = delegate
                {
                    Debug.Log(" save and exit");
                    LocalConfig.instance.SaveGameConfig();
                    GameSetting.Instance.showLayer(GameSetting.Instance.List);
                };
                GameSetting.Instance.SetConfirmCallBack(callBack);
            }

        }


        SelectItem();
        UpdateUI();
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

    void UpdateUI()
    {
        textItems[0].text = LocalConfig.instance.gameConfig.GetCoinValue().ToString();
        textItems[1].text = LocalConfig.instance.gameConfig.GetValueGame().ToString();
        textItems[2].text = LocalConfig.instance.gameConfig.GetGiftAdd();
        textItems[3].text = LocalConfig.instance.gameConfig.GetVolume().ToString();
        textItems[4].text = LocalConfig.instance.gameConfig.GetDifficulty().ToString();
        textItems[5].text = LocalConfig.instance.gameConfig.GetWaitCoinTime().ToString();
        textItems[6].text = LocalConfig.instance.gameConfig.GetDemoVideo();
        textItems[7].text = LocalConfig.instance.gameConfig.GetAutoFire();
        textItems[8].text = LocalConfig.instance.gameConfig.GetHealthy().ToString();
        textItems[9].text = LocalConfig.instance.gameConfig.GetGiftScore().ToString();
        textItems[10].text = LocalConfig.instance.gameConfig.GetGiftCount().ToString();
    }

}
