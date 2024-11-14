using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;


public abstract class SettingLayers : MonoBehaviour{
    public abstract void Init();
    public abstract void Run();
    public abstract void SelectItem();
}