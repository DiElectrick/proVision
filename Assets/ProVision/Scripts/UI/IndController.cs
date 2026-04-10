using UnityEngine;
using DG.Tweening;

public class IndController : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private void Start()
    {
        // Запоминаем начальную позицию
        Vector3 startPos = transform.position;

        // Конечная позиция (вверх)
        Vector3 endPos = startPos + Vector3.up * moveDistance;

        // Анимируем и удаляем
        transform.DOMove(endPos, duration)
            .SetEase(easeType)
            .OnComplete(() => Destroy(gameObject));
    }
}