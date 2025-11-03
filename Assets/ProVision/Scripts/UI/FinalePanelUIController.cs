using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class FinalePanelUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI pacientsText;
    [SerializeField] TextMeshProUGUI prizeText;
    [SerializeField] TextMeshProUGUI fineText;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI quotaDaysText;
    [SerializeField] TextMeshProUGUI quotaValueText;

    [SerializeField] GameObject[] stats;
    [SerializeField] private float delayBetween = 0.2f;
    [SerializeField] float popDuration = 0.1f;
    [SerializeField] private float animationDuration = 0.3f;

    private RectTransform _rectTransform;
    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;
    private bool _visible = false;

    private void Awake()
    {

    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _shownPosition = _rectTransform.anchoredPosition;

        // Получаем размер канваса через родительский Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        _hiddenPosition = new Vector2(_shownPosition.x, canvasRect.rect.height + _rectTransform.rect.height);
        _rectTransform.anchoredPosition = _hiddenPosition;

       // stats = GameObject.FindGameObjectsWithTag("stat");
    }


    public void ShowStats(int day, int pacients, int prize, int fine, int total, int dayUntillQuota, int nextQuota)
    {
        pacientsText.text = $"{pacients}";
        prizeText.text = $"{prize}";
        fineText.text = $"{fine}";
        totalText.text = $"Всего: {total}$";
        dayText.text = $"День {day}";
        quotaDaysText.text = $"Дней до квоты: {dayUntillQuota}";
        quotaValueText.text = $"Квота: {nextQuota}$";


        StartCoroutine(AnimateStats());
    }

    IEnumerator AnimateStats()
    {
        // Ждём, пока панель появится (до анимации UI самой панели)
        yield return new WaitForSeconds(animationDuration + 0.2f);

        for (int i = 0; i < stats.Length; i++)
        {

            if (stats[i] == null) continue;

            GameObject stat = stats[i];

            stat.SetActive(true);

            // pop анимация
            // yield return StartCoroutine(PopElement(stat.transform));

            // ждём перед следующим
            yield return new WaitForSeconds(delayBetween);
        }
    }


    public void Show()
    {
        foreach (var item in stats)
        {
            item.SetActive(false);
        }

        _visible = true;
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPos(_shownPosition, animationDuration)
            .SetEase(Ease.OutCubic);


    }

    public void Hide()
    {
        _visible = false;
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPos(_hiddenPosition, animationDuration)
            .SetEase(Ease.InCubic);

        StopAllCoroutines();   // simplest


    }

}
