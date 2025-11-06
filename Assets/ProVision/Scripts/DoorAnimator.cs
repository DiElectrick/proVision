using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class DoorAnimator : MonoBehaviour
{
    public static DoorAnimator Instance { get; private set; }

    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease easeType = Ease.OutCubic;
    [SerializeField] public Transform targetTransform;

    [Header("Show State")]
    [SerializeField] private float showPositionY = 0f;
    [SerializeField] private Vector3 showScale = Vector3.one * 0.6f;

    [Header("Hide State")]
    [SerializeField] private float hidePositionY = 2f;
    [SerializeField] private Vector3 hideScale = Vector3.zero;

    private void Awake()
    {
            Instance = this;
    }

    // Словарь для хранения исходной прозрачности объектов
    private Dictionary<Transform, Dictionary<SpriteRenderer, float>> originalAlphas = new Dictionary<Transform, Dictionary<SpriteRenderer, float>>();

    public void AnimateSprite(bool animateForward)
    {
        Debug.Log("Animate");

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
            // Прямая анимация: скрытие
            sequence.Join(targetTransform.DOMoveY(hidePositionY, duration));
            sequence.Join(targetTransform.DOScale(hideScale, duration));

            // Анимируем альфа-канал всех спрайт рендереров до 0
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                sequence.Join(renderer.DOFade(0f, duration));
            }
        }
        else
        {
            // Обратная анимация: показ
            sequence.Join(targetTransform.DOMoveY(showPositionY, duration));
            sequence.Join(targetTransform.DOScale(showScale, duration));

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

    // Мгновенно скрывает объект
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

        // Мгновенно применяем настройки скрытия
        Vector3 newPosition = targetTransform.position;
        newPosition.y = hidePositionY;
        targetTransform.position = newPosition;

        targetTransform.localScale = hideScale;

        // Устанавливаем прозрачность в 0 для всех спрайтов
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = 0f;
            renderer.color = color;
        }
    }

    // Мгновенно показывает объект
    public void ShowInstantly()
    {
        if (targetTransform == null) return;

        // Получаем все спрайт рендереры у самого объекта и у его детей
        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        // Сохраняем исходную прозрачность при первом использовании
        if (!originalAlphas.ContainsKey(targetTransform))
        {
            SaveOriginalAlphas(targetTransform, spriteRenderers);
        }

        // Мгновенно применяем настройки показа
        Vector3 newPosition = targetTransform.position;
        newPosition.y = showPositionY;
        targetTransform.position = newPosition;

        targetTransform.localScale = showScale;

        // Восстанавливаем исходную прозрачность всем спрайтам
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (originalAlphas.ContainsKey(targetTransform) &&
                originalAlphas[targetTransform].ContainsKey(renderer))
            {
                float originalAlpha = originalAlphas[targetTransform][renderer];
                Color color = renderer.color;
                color.a = originalAlpha;
                renderer.color = color;
            }
            else
            {
                // Если по какой-то причине нет сохраненного значения, используем 1
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }
        }
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

    // Метод для принудительного обновления сохраненных прозрачностей
    public void UpdateOriginalAlphas(Transform targetTransform)
    {
        if (targetTransform == null) return;

        SpriteRenderer[] spriteRenderers = targetTransform.GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0) return;

        SaveOriginalAlphas(targetTransform, spriteRenderers);
    }

    // Метод для очистки сохраненных данных
    public void ClearSavedAlphas(Transform targetTransform)
    {
        if (originalAlphas.ContainsKey(targetTransform))
        {
            originalAlphas.Remove(targetTransform);
        }
    }
}