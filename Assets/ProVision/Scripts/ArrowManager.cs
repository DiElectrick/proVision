using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Arrows
{
    None,
    Loop,
    Deck,
    Lamp,
    Eye,
    Spravka,
    Dis1,
    Dis2,
    Dis3,
    Dis4,
    Dis5,
    Dis6
}

public class ArrowManager : MonoBehaviour
{
    [SerializeField] List<GameObject> arrows = new List<GameObject>();
    [SerializeField] float fadeDuration = 0.3f;
    [SerializeField] float bounceStrength = 0.2f;
    [SerializeField] float bounceDuration = 0.5f;

    private Dictionary<Arrows, CanvasGroup> arrowCanvasGroups = new Dictionary<Arrows, CanvasGroup>();
    private Dictionary<Arrows, Vector3> originalScales = new Dictionary<Arrows, Vector3>();
    private Dictionary<Arrows, Sequence> activeAnimations = new Dictionary<Arrows, Sequence>();

    public static ArrowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCanvasGroups();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCanvasGroups()
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i] != null)
            {
                // Сохраняем оригинальный scale
                originalScales[(Arrows)i] = arrows[i].transform.localScale;

                // Добавляем CanvasGroup если его нет
                var canvasGroup = arrows[i].GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = arrows[i].AddComponent<CanvasGroup>();
                }

                arrowCanvasGroups[(Arrows)i] = canvasGroup;
                canvasGroup.alpha = 0f;
                arrows[i].SetActive(false);
            }
        }
    }

    public void ShowArrow(Arrows arrow)
    {
        if (!arrowCanvasGroups.ContainsKey(arrow) || arrowCanvasGroups[arrow] == null)
            return;

        // Останавливаем предыдущую анимацию если есть
        if (activeAnimations.ContainsKey(arrow))
        {
            activeAnimations[arrow].Kill();
        }

        var targetObject = arrows[(int)arrow];
        var canvasGroup = arrowCanvasGroups[arrow];
        var originalScale = originalScales[arrow];

        targetObject.SetActive(true);
        targetObject.transform.localScale = originalScale; // Восстанавливаем оригинальный scale

        Sequence showSequence = DOTween.Sequence();

        // Анимация появления
        showSequence.Append(canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad));

        // Анимация "bounce" эффекта от уменьшенного размера к оригинальному
        targetObject.transform.localScale = originalScale * 0.8f;
        showSequence.Join(targetObject.transform.DOScale(originalScale, fadeDuration).SetEase(Ease.OutBack));

        // Постоянная пульсация относительно оригинального размера
        showSequence.Append(targetObject.transform.DOScale(originalScale * (1f + bounceStrength), bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine));

        activeAnimations[arrow] = showSequence;
    }


    public void HideArrow(Arrows arrow)
    {
        if (!arrowCanvasGroups.ContainsKey(arrow) || arrowCanvasGroups[arrow] == null)
            return;

        // Останавливаем текущую анимацию
        if (activeAnimations.ContainsKey(arrow))
        {
            activeAnimations[arrow].Kill();
            activeAnimations.Remove(arrow);
        }

        var targetObject = arrows[(int)arrow];
        var canvasGroup = arrowCanvasGroups[arrow];
        var originalScale = originalScales[arrow];

        Sequence hideSequence = DOTween.Sequence();

        // Анимация исчезновения
        hideSequence.Append(canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad));
        hideSequence.Join(targetObject.transform.DOScale(originalScale * 0.8f, fadeDuration).SetEase(Ease.InBack));
        hideSequence.OnComplete(() => {
            targetObject.SetActive(false);
            targetObject.transform.localScale = originalScale; // Восстанавливаем оригинальный scale
        });
    }

    public void HideAllArrows()
    {
        foreach (var arrow in arrowCanvasGroups.Keys)
        {
            if (arrow != Arrows.None)
            {
                HideArrow(arrow);
            }
        }
    }

    public void PulseArrow(Arrows arrow)
    {
        if (!arrowCanvasGroups.ContainsKey(arrow) || arrowCanvasGroups[arrow] == null)
            return;

        var targetObject = arrows[(int)arrow];
        var originalScale = originalScales[arrow];

        // Создаем эффект пульсации относительно оригинального размера
        Sequence pulseSequence = DOTween.Sequence();
        pulseSequence.Append(targetObject.transform.DOScale(originalScale * 1.3f, 0.2f).SetEase(Ease.OutQuad));
        pulseSequence.Append(targetObject.transform.DOScale(originalScale, 0.2f).SetEase(Ease.InQuad));
        pulseSequence.SetLoops(2);
    }

    // Метод для принудительного восстановления оригинального scale
    public void ResetArrowScale(Arrows arrow)
    {
        if (originalScales.ContainsKey(arrow))
        {
            var targetObject = arrows[(int)arrow];
            if (targetObject != null)
            {
                targetObject.transform.localScale = originalScales[arrow];
            }
        }
    }

    private void OnDestroy()
    {
        // Очищаем все твины при уничтожении объекта
        foreach (var sequence in activeAnimations.Values)
        {
            sequence?.Kill();
        }
    }
}