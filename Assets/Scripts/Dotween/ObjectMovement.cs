using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ObjectMovement : MonoBehaviour
{
    public Vector3 targetPositionOffset;  // 目标位置
    public float movementDuration = 1f;  // 移动持续时间
    public Ease movementEase = Ease.Linear;  // 移动缓动类型

    public int Loop = -1;
    public bool IsRevert = false;
    [Header("每次激活是否都显示动画")]
    public bool ShowOnEveryActivate = true;

    [Header("平移动画完成后是否隐藏")]
    public bool HideAfterShow = false;

    private Vector3 targetPosition;  // 目标位置
    private Vector3 startPosition;  // 起始位置

    private void Start()
    {

        StartMovement();
    }

    void OnEnable()
    {
        if (targetPosition != Vector3.zero && ShowOnEveryActivate)
        {
            StartMovement();
        }
    }

    public void StartMovement()
    {
        SetPosition();
        // 将游戏对象移动到目标位置
        transform.DOMove(targetPosition, movementDuration)
                .SetEase(movementEase)
                .SetLoops(Loop)
                .OnComplete(OnMovementComplete)
                .Play();
    }

    void SetPosition()
    {
        startPosition = base.transform.position;
        targetPosition = startPosition + targetPositionOffset;
        if (IsRevert) { transform.position = targetPosition; targetPosition = startPosition; }
    }


    void OnMovementComplete()
    {
        if (HideAfterShow) StartCoroutine(Hidden());


    }

    private IEnumerator Hidden()
    {
        yield return new WaitForSeconds(2);
        transform.gameObject.SetActive(false);
        yield return null;
    }
}