using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDur = 1f;
    [SerializeField] private Ease easeType = Ease.OutCubic;

    // Словарь для хранения исходной прозрачности объектов
    private Dictionary<Transform, Dictionary<SpriteRenderer, float>> originalAlphas = new Dictionary<Transform, Dictionary<SpriteRenderer, float>>();

    public void AnimateSprite(Transform targetTransform, bool animateForward)
    {
        if (targetTransform == null) return;

        // Получаем все спрайт рендереры у самого объекта и у его детей
        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        // Сохраняем исходную прозрачность при первой анимации
        if (!originalAlphas.ContainsKey(targetTransform))
        {
            SaveOriginalAlphas(targetTransform, spriteRenderers);
        }

        Sequence sequence = DOTween.Sequence();

        if (animateForward)
        {
            // Прямая анимация: вверх + уменьшение + растворение всех спрайтов
            sequence.Join(targetTransform.DOMoveY(targetTransform.position.y + moveDistance, duration));
            sequence.Join(targetTransform.DOScale(Vector3.zero, duration));

            // Анимируем альфа-канал всех спрайт рендереров до 0
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

            // Возвращаем исходную прозрачность всем спрайтам
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                if (originalAlphas.ContainsKey(targetTransform) &&
                    originalAlphas[targetTransform].ContainsKey(renderer))
                {
                    float originalAlpha = originalAlphas[targetTransform][renderer];
                    sequence.Join(renderer.DOFade(originalAlpha, duration));
                }
                else
                {
                    // Если по какой-то причине нет сохраненного значения, используем 1
                    sequence.Join(renderer.DOFade(1f, duration));
                }
            }
        }

        sequence.SetEase(easeType);
    }

    // Метод для сохранения исходной прозрачности
    private void SaveOriginalAlphas(Transform targetTransform, SpriteRenderer[] spriteRenderers)
    {
        var rendererAlphas = new Dictionary<SpriteRenderer, float>();

        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            rendererAlphas[renderer] = renderer.color.a;
        }

        originalAlphas[targetTransform] = rendererAlphas;
    }

    // Метод для принудительного обновления сохраненных прозрачностей (если нужно изменить извне)
    public void UpdateOriginalAlphas(Transform targetTransform)
    {
        if (targetTransform == null) return;

        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        SaveOriginalAlphas(targetTransform, spriteRenderers);
    }

    // Метод для очистки сохраненных данных (если объект уничтожается)
    public void ClearSavedAlphas(Transform targetTransform)
    {
        if (originalAlphas.ContainsKey(targetTransform))
        {
            originalAlphas.Remove(targetTransform);
        }
    }
}