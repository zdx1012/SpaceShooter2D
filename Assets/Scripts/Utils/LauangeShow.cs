using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class LauangeShow : MonoBehaviour
{
    private Text text;

    public string[] strs;

    private int cnSize;

    private int enSize;

    private bool isInit = true;

    private int Language = 0;

    private void Awake()
    {

        if (isInit)
        {
            isInit = false;
            text = GetComponent<Text>();
            cnSize = text.fontSize;
            enSize = cnSize - 12;
            if (enSize < 20) enSize = 20;
        }
    }

    private void OnEnable()
    {
        Language = LocalConfig.instance.gameConfig.language;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (isInit)
        {
            Awake();
        }
        if (Language == 0)
        {
            text.text = strs[0];
            text.fontSize = cnSize;
        }
        else if (Language == 1)
        {
            text.text = strs[1];
            text.fontSize = enSize;
            return;
        }
    }
}
