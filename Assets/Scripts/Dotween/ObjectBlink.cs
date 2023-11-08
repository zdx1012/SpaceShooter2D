using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ObjectBlink : MonoBehaviour
{

    public bool isFade = true;
    public int Count = 3;
    public bool HideOnComplet = true;
    public bool OnlyShowOnce = true;
    private int showCount = 0;

    private GameObject targetObject;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        targetObject = transform.gameObject;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {

    }

    void OnEnable()
    {
        showCount += 1;
        if (OnlyShowOnce && showCount > 1) return;
        // 循环渐隐渐现和缩放
        Sequence sequence = DOTween.Sequence();
        sequence.Append(targetObject.transform.DOScale(0.8f, 0.5f).SetEase(Ease.OutCubic));
        if (isFade) sequence.Join(canvasGroup.DOFade(0.6f, 0.5f).SetEase(Ease.OutCubic));
        sequence.Append(targetObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutCubic));
        if (isFade) sequence.Join(canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic));
        sequence.SetLoops(Count); // 设置循环次数
        if (HideOnComplet) sequence.OnComplete(HideObject);
        sequence.Play();
    }

    void HideObject()
    {
        // 渐变透明度
        canvasGroup.DOFade(0f, 2f).SetEase(Ease.OutCubic).OnComplete(DeactivateObject);
    }

    void DeactivateObject()
    {
        // 隐藏对象
        targetObject.SetActive(false);
    }
}