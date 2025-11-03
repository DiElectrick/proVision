using DG.Tweening;
using UnityEngine;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDur = 1f;
    [SerializeField] private Ease easeType = Ease.OutCubic;

    public void AnimateSprite(Transform targetTransform, bool animateForward)
    {
        if (targetTransform == null) return;

        // Получаем все спрайт рендереры у самого объекта и у его детей
        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        Sequence sequence = DOTween.Sequence();

        if (animateForward)
        {
            // Прямая анимация: вверх + уменьшение + растворение всех спрайтов
            sequence.Join(targetTransform.DOMoveY(targetTransform.position.y + moveDistance, duration));
            sequence.Join(targetTransform.DOScale(Vector3.zero, duration));

            // Анимируем альфа-канал всех спрайт рендереров
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                sequence.Join(renderer.DOFade(0f, duration));
            }
        }
        else
        {
            // Обратная анимация: возврат к исходному состоянию
            sequence.Join(targetTransform.DOMoveY(targetTransform.position.y - moveDistance, duration));
            sequence.Join(targetTransform.DOScale(Vector3.one * 0.6f, duration));

            // Возвращаем прозрачность всем спрайтам
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                sequence.Join(renderer.DOFade(1f, duration));
            }
        }

        sequence.SetEase(easeType);
    }
}