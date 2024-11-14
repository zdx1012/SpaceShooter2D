using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class ImageLanguageAdaptation : MonoBehaviour
{
    public Sprite[] sprites;
    private Image image;

    private int Language = 0;

    void Awake()
    {
        Language = LocalConfig.instance.gameConfig.language;
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        Debug.Assert(Language <= sprites.Length);
        // 默认使用是中文
        if ((int)Language < sprites.Length && sprites[(int)Language] != null)
        {
            image.sprite = sprites[(int)Language];
            image.SetNativeSize();
        }
    }
}
