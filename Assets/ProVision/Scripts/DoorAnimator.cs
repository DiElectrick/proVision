using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class DoorAnimator : MonoBehaviour
{
    public static DoorAnimator Instance { get; private set; }

    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDur = 1f;
    [SerializeField] private Ease easeType = Ease.OutCubic;
    [SerializeField] public Transform targetTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Словарь для хранения исходной прозрачности объектов
    private Dictionary<Transform, Dictionary<SpriteRenderer, float>> originalAlphas = new Dictionary<Transform, Dictionary<SpriteRenderer, float>>();

    public void AnimateSprite(bool animateForward)
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

    // Новый метод: мгновенно скрывает объект без анимации
    public void HideInstantly()
    {
        if (targetTransform == null) return;

        // Получаем все спрайт рендереры у самого объекта и у его детей
        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        // Сохраняем исходную прозрачность при первом скрытии
        if (!originalAlphas.ContainsKey(targetTransform))
        {
            SaveOriginalAlphas(targetTransform, spriteRenderers);
        }

        // Мгновенно устанавливаем прозрачность в 0 для всех спрайтов
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = 0f;
            renderer.color = color;
        }

        // Мгновенно скрываем объект (опционально - можно также изменить scale или position)
        targetTransform.localScale = Vector3.zero;
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