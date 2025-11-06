using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TextPanel : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;      // Ссылка на Panel (дочерний объект)
    public TextMeshProUGUI textDisplay; // Ссылка на Text (TMP)

    [Header("Settings")]
    public float lettersPerSecond = 30f;
    public const float baseHideDelay = 2f;

    private string currentText;
    private Coroutine showTextCoroutine;
    private Coroutine hideCoroutine;
    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        G.textPanel = this;
    }

    void Start()
    {
        // Находим или добавляем CanvasGroup на панель
        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        // Скрываем всю панель при старте
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.blocksRaycasts = false;
        panel.SetActive(false);
    }

    public void ShowText(string text, float hideDelay = baseHideDelay)
    {
        // Отменяем предыдущие корутины
        if (showTextCoroutine != null) StopCoroutine(showTextCoroutine);
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        // Активируем и показываем панель
        panel.SetActive(true);
        panelCanvasGroup.DOKill();
        panelCanvasGroup.DOFade(1f, 0.3f);
        panelCanvasGroup.blocksRaycasts = true;

        // Запускаем показ текста
        currentText = text;
        showTextCoroutine = StartCoroutine(ShowTextLetterByLetter(text, hideDelay));
    }

    private IEnumerator ShowTextLetterByLetter(string text, float hideDelay = baseHideDelay)
    {
        textDisplay.text = "";

        float delayBetweenLetters = 1f / lettersPerSecond;

        // Просто показываем текст посимвольно
        for (int i = 0; i < text.Length; i++)
        {
            textDisplay.text += text[i];
            yield return new WaitForSeconds(delayBetweenLetters);
        }

        showTextCoroutine = null;

        // Запускаем скрытие с задержкой
        hideCoroutine = StartCoroutine(HideAfterDelay(hideDelay));
    }

    private IEnumerator HideAfterDelay(float hideDelay)
    {
        yield return new WaitForSeconds(hideDelay);
        Hide();
    }

    public void Hide()
    {
        // Останавливаем все корутины
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
            showTextCoroutine = null;
        }
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Плавно скрываем панель
        panelCanvasGroup.DOKill();
        panelCanvasGroup.DOFade(0f, 0.3f).OnComplete(() => {
            panelCanvasGroup.blocksRaycasts = false;
            panel.SetActive(false);
            textDisplay.text = "";
        });
    }

    public void SkipAnimation()
    {
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
            showTextCoroutine = null;

            textDisplay.text = currentText;

            // Перезапускаем таймер скрытия
            if (hideCoroutine != null) StopCoroutine(hideCoroutine);
            hideCoroutine = StartCoroutine(HideAfterDelay(baseHideDelay));
        }
    }

    public bool IsShowingText => showTextCoroutine != null;
}