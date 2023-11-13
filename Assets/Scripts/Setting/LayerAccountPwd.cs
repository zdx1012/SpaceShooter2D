using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;

public class LayerAccountPwd : SettingLayers
{
    public Text[] pwdText;
    public Image okImage;
    public Image exitImage;
    public Text LogText;
    public GameObject arrowImage;
    public Image maskImage;

    private bool changeNum = false;
    private bool isUpPressed = false;
    private bool isDownPressed = false;
    private bool isSenterOkPressed = false;
    private int currentIndex = 0;
    private int maxIndex = 7;
    public override void Init()
    {
        pwdText = new Text[6];
        for (int i = 0; i < pwdText.Length; i++)
        {
            pwdText[i] = transform.Find("PwdNum").GetChild(i).GetComponent<Text>();
            pwdText[i].text = "0";
        }

    }

    public override void Run()
    {
        isUpPressed = InputUtil.instance.IsSettingUpOnceClicked() || InputUtil.instance.IsSettingLeftOnceClicked();
        isDownPressed = InputUtil.instance.IsSettingDownOnceClicked() || InputUtil.instance.IsSettingRightOnceClicked();
        isSenterOkPressed = InputUtil.instance.IsSettingCenterOnceClicked();


        if (isSenterOkPressed && currentIndex <= 5) changeNum = !changeNum;
        if (currentIndex <= 5) { arrowImage.SetActive(changeNum); } else { arrowImage.SetActive(false); };

        if (!changeNum)
        {
            if (isDownPressed)
            {
                currentIndex += 1;
                if (currentIndex > maxIndex) currentIndex = 0;
            }
            else if (isUpPressed)
            {
                currentIndex -= 1;
                if (currentIndex < 0) currentIndex = maxIndex;
            }
        }
        else
        {
            UpdateNum();
        }




        CheckPwdOrExit();
        SelectItem();
    }

    public override void SelectItem()
    {
        for (int i = 0; i < maxIndex; i++)
        {
            // 数字
            if (currentIndex <= 5 && currentIndex == i)
            {
                maskImage.gameObject.SetActive(true);
                maskImage.transform.position = pwdText[i].transform.position;
                arrowImage.transform.position = pwdText[i].transform.position;
            }
            else if (currentIndex > 5) { maskImage.gameObject.SetActive(false); }

            // 选中确定
            if (currentIndex == 6)
            {
                okImage.sprite = GameSetting.Instance.CheckedSprite;
            }
            else
            {
                okImage.sprite = GameSetting.Instance.UnCheckedSprite;
            }

            // 选中退出
            if (currentIndex == 7)
            {
                exitImage.sprite = GameSetting.Instance.CheckedSprite;
            }
            else
            {
                exitImage.sprite = GameSetting.Instance.UnCheckedSprite;
            }

        }
    }


    void UpdateNum()
    {
        if (currentIndex <= 5 && changeNum)
        {
            // string to int
            int num = int.Parse(pwdText[currentIndex].text);
            if (isUpPressed) { num -= 1; if (num < 0) num = 9; }
            if (isDownPressed) { num += 1; if (num > 9) num = 0; }
            pwdText[currentIndex].text = num.ToString();
        }
    }
    void CheckPwdOrExit()
    {
        if (currentIndex == 6 && isSenterOkPressed)
        {
            if (pwdText[0].text == "4" && pwdText[1].text == "5" && pwdText[2].text == "5" && pwdText[3].text == "5" && pwdText[4].text == "5" && pwdText[5].text == "5")
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                LogText.gameObject.SetActive(true);
            }
        }
        else if (currentIndex != 6)
        {
            LogText.gameObject.SetActive(false);
        }
        if (currentIndex == 7 && isSenterOkPressed)
        {
            GameSetting.Instance.showLayer(GameSetting.Instance.List);
        }
    }
}
