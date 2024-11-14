using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerAccountParent : SettingLayers
{

    public SettingLayers PwdLayer;
    public SettingLayers AccountLayer;

    private SettingLayers curActivate;

    public override void Init()
    {
        curActivate = PwdLayer;
        if (curActivate == AccountLayer)
        {
            AccountLayer.gameObject.SetActive(true);
            PwdLayer.gameObject.SetActive(false);
        }
        else
        {
            AccountLayer.gameObject.SetActive(false);
            PwdLayer.gameObject.SetActive(true);
        }
        curActivate.Init();
    }

    public override void Run()
    {
        if (curActivate == PwdLayer && !PwdLayer.gameObject.activeSelf)
        {
            GotoInfo();
        }
        curActivate.Run();
    }

    public override void SelectItem()
    {
    }

    public void GotoInfo()
    {
        curActivate = AccountLayer;
        AccountLayer.gameObject.SetActive(true);
        PwdLayer.gameObject.SetActive(false);
        curActivate.Init();
    }

}
