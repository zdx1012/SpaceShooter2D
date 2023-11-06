using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class LauangeShow : MonoBehaviour
{
    private Text m_tv;

    public string[] strs;

    private int zh_size;

    private int en_size;

    private bool isInit = true;

    private int Language = 0;

    private void Awake()
    {
        
        if (isInit)
        {
            isInit = false;
            m_tv = GetComponent<Text>();
            zh_size = m_tv.fontSize;
            en_size = zh_size - 12;
            if (en_size < 20) en_size = 20;
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
            m_tv.text = strs[0];
            m_tv.fontSize = zh_size;
            return;
        }
        if (Language == 1 || strs.Length <= 2)
        {
            m_tv.text = strs[1];
            m_tv.fontSize = en_size;
            return;
        }
        if (Language == 2)
        {
            m_tv.text = strs[2];
            m_tv.fontSize = en_size;
            return;
        }
        if (strs.Length < 4)
        {
            m_tv.text = strs[1];
        }
        else
        {
            m_tv.text = strs[3];
        }
        m_tv.fontSize = en_size;
    }
}
