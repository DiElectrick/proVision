using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerdictUIController : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;

    [SerializeField] List<Toggle> toggles;

    private RectTransform _rectTransform;
    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;
    private bool _visible = false;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Запоминаем текущую позицию как показанную
        _shownPosition = _rectTransform.anchoredPosition;

        // Вычисляем скрытую позицию за правым краем
        _hiddenPosition = new Vector2(Screen.width + _rectTransform.rect.width, _shownPosition.y);

        // Устанавливаем начальную позицию - скрытую
        _rectTransform.anchoredPosition = _hiddenPosition;
    }


    public void Verdict()
    {

        List<bool> list = new List<bool>();
        for (int i = 0; i < toggles.Count; i++)
        {
            list.Add(toggles[i].isOn);
        }

        Diagnosis diagnosis = new Diagnosis(list);
        Hide();

        G.process.SendDiagnosis(diagnosis);

    }

    public void Change()
    {
        if (_visible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    void Show()
    {
        _visible = true;
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPos(_shownPosition, animationDuration)
            .SetEase(Ease.OutCubic);
    }

    void Hide()
    {
        _visible = false;
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPos(_hiddenPosition, animationDuration)
            .SetEase(Ease.InCubic);
    }
}