using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoSingleton<GameSetting>
{

    public Sprite CheckedSprite;
    public Sprite UnCheckedSprite;

    // 确认弹窗的是否对象
    public Image[] confirmImages;


    private int curSelectIndex;
    public readonly int List = 0;
    public readonly int GameSet = 1;
    public readonly int Account = 2;
    public readonly int HWTest = 3;
    public readonly int HWCheck = 4;
    public readonly int Factory = 5;
    public readonly int Exit = 6;


    private SettingLayers[] settingLayers;

    private GameObject confirmObject;
    private ConfirmCallBack callBack;
    private int confirmSelectedIndex = -1;





    protected override void Awake()
    {
        base.Awake();
    }


    // Start is called before the first frame update
    void Start()
    {
        curSelectIndex = 0;
        Transform transform = GameObject.Find("Canvas").transform.Find("Background").transform;
        settingLayers = new SettingLayers[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            settingLayers[i] = transform.GetChild(i).GetComponent<SettingLayers>();
            Debug.Assert(settingLayers[i] != null);
        }

        showLayer(0);

        // 确认弹窗
        confirmObject = GameObject.Find("Canvas").transform.Find("Confirm").gameObject;
    }

    public void showLayer(int index)
    {
        for (int i = 0; i < settingLayers.Length; i++)
        {
            settingLayers[i].gameObject.SetActive(i == index);
        }
        settingLayers[index].Init();

        curSelectIndex = index;
    }

    // Update is called once per frame
    void Update()
    {
        if (!confirmObject.activeInHierarchy)
        {
            settingLayers[curSelectIndex].Run();
        }
        else
        {
            UpdateConfirm();
        }
    }


    public void SetConfirmCallBack(ConfirmCallBack IcallBack)
    {
        callBack = IcallBack;
        confirmSelectedIndex = 0;
        confirmObject.SetActive(true);
    }
    void UpdateConfirm()
    {
        if (InputUtil.instance.IsSettingDownOnceClicked())
        {
            confirmSelectedIndex += 1;
        }
        else if (InputUtil.instance.IsSettingUpOnceClicked())
        {
            confirmSelectedIndex -= 1;
        }
        if (confirmSelectedIndex < 0 || confirmSelectedIndex > 1) confirmSelectedIndex = 0;
        for (int i = 0; i < confirmImages.Length; i++)
        {
            confirmImages[i].sprite = i == confirmSelectedIndex ? CheckedSprite : UnCheckedSprite;
        }
        if (InputUtil.instance.IsSettingCenterOnceClicked())
        {
            if (confirmSelectedIndex == 0)
            {
                callBack();
            }
            confirmObject.SetActive(false);
        }
    }

    public void Save()
    {
        Debug.Log("save data");
        LocalConfig.instance.SaveGameConfig();
    }
}
