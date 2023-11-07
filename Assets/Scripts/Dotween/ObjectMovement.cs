using UnityEngine;
using DG.Tweening;

public class ObjectMovement : MonoBehaviour
{
    public Vector3 targetPositionOffset;  // 目标位置
    public float movementDuration = 1f;  // 移动持续时间
    public Ease movementEase = Ease.Linear;  // 移动缓动类型
    private Vector3 targetPosition;  // 目标位置

    private void Start()
    {
        targetPosition = base.transform.position + targetPositionOffset;
        StartMovement();
    }

    private void StartMovement()
    {
        // 将游戏对象移动到目标位置
        transform.DOMove(targetPosition, movementDuration).SetEase(movementEase).SetLoops(-1).Play();
    }

    private void ReverseMovement()
    {
        // 将游戏对象移动回起始位置
        transform.DOMove(transform.position, movementDuration).SetEase(movementEase).OnComplete(StartMovement);
    }
}