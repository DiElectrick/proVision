using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour
{
    Diagnosis curentDiagnosis = new Diagnosis();
    [SerializeField] GameObject curentEye;
    EyeController controller;
    [SerializeField] EyeElementsLib lib;

    List<Diseases> allDisasesList = new List<Diseases>();

    GameSession session;

    [Header("Balance")]
    [SerializeField] int dayByQouta = 3;
    [SerializeField] int startQouta = 300;
    [SerializeField] int bonusQuota = 100;

    [SerializeField] int prize = 100;
    [SerializeField] int fine = -50;
    [SerializeField] int neutralPrize = 10;
    [SerializeField] int neutralFine = -10;

    [SerializeField] Timer timer;
    [SerializeField] FinalePanelUIController statsPanel;

    // Переменные для хранения статистики за день
    private int dailyPrize = 0;
    private int dailyFine = 0;
    private int dailyPacients = 0;

    // Переменные для квоты
    private int currentQuota = 0;
    private int daysUntilNextQuota = 0;

    private void Awake()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Diseases)).Length; i++)
        {
            allDisasesList.Add((Diseases)i);
        }
    }

    private void Start()
    {
        G.process = this;

        controller = curentEye.GetComponent<EyeController>();
        session = G.curentSession;
        timer = GetComponent<Timer>();

        if (session == null)
        {
            session = new GameSession();
            G.curentSession = session;
        }

        // Инициализация системы квоты
        InitializeQuotaSystem();

        // Подписываемся на событие таймера
        if (timer != null)
        {
            timer.OnTimerExpired += OnTimerExpired;
        }

        NextDay();
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (timer != null)
        {
            timer.OnTimerExpired -= OnTimerExpired;
        }
    }

    private void InitializeQuotaSystem()
    {
        currentQuota = startQouta;
        daysUntilNextQuota = dayByQouta;
    }

    private void OnTimerExpired()
    {
        // Проверяем и списываем квоту при необходимости
        CheckAndPayQuota();

        statsPanel.Show();
        statsPanel.ShowStats(session.curentDay, dailyPacients, dailyPrize, dailyFine, session.curentMoney, daysUntilNextQuota, currentQuota);
    }

    public void NextDay()
    {
        session.curentDay++;
        // Сбрасываем статистику за день
        dailyPrize = 0;
        dailyFine = 0;
        dailyPacients = 0;
        statsPanel.Hide();
        NewPacient();

        if (timer != null)
        {
            timer.ResetAndStartTimer();
        }
    }

    void NewPacient()
    {
        curentDiagnosis = GenerateDiagnosis(allDisasesList, 2);
        GenerateEye();
    }

    Diagnosis GenerateDiagnosis(List<Diseases> availableDiseases1, int diseasesNum)
    {
        Diagnosis diagnosis = new Diagnosis();

        // Создаем копию списка
        List<Diseases> availableDiseases = new List<Diseases>(availableDiseases1);

        int n = UnityEngine.Random.Range(1, diseasesNum + 1);
        Debug.Log($"Trying to generate {n} diseases from {availableDiseases.Count} available");

        // Ограничиваем n размером доступного списка
        n = Mathf.Min(n, availableDiseases.Count);

        List<Diseases> selectedDiseases = new List<Diseases>();

        // Сначала выбираем все болезни
        for (int i = 0; i < n; i++)
        {
            if (availableDiseases.Count == 0)
                break;

            int index = UnityEngine.Random.Range(0, availableDiseases.Count);
            Diseases selectedDisease = availableDiseases[index];
            selectedDiseases.Add(selectedDisease);
            availableDiseases.RemoveAt(index);
        }

        // Затем применяем их к диагнозу и обрабатываем конфликты
        foreach (Diseases disease in selectedDiseases)
        {
            Debug.Log($"Applying disease: {disease}");
            diagnosis.diseases[(int)disease] = true;

            // Обрабатываем конфликтующие болезни
            if (disease == Diseases.Stigmatizm_plus)
            {
                // Убираем противоположную болезнь если она была выбрана
                if (selectedDiseases.Contains(Diseases.Stigmatizm_minus))
                {
                    diagnosis.diseases[(int)Diseases.Stigmatizm_minus] = false;
                }
            }
            else if (disease == Diseases.Stigmatizm_minus)
            {
                // Убираем противоположную болезнь если она была выбрана
                if (selectedDiseases.Contains(Diseases.Stigmatizm_plus))
                {
                    diagnosis.diseases[(int)Diseases.Stigmatizm_plus] = false;
                }
            }
        }

        return diagnosis;
    }

    void GenerateEye()
    {
        foreach (Transform child in curentEye.transform)
        {
            Destroy(child.gameObject);
        }

        if (lib.applePrefabs.Count > 0)
            Instantiate(lib.applePrefabs[UnityEngine.Random.Range(0, lib.applePrefabs.Count)],
            curentEye.transform);

        if (lib.rainbowPrefabs.Count > 0)
        {
            GameObject rainbow = Instantiate(lib.rainbowPrefabs[UnityEngine.Random.Range(0, lib.rainbowPrefabs.Count)],
            curentEye.transform);

            controller.rainbow = rainbow;
        }

        if (lib.pupilPrefabs.Count > 0)
        {
            GameObject pupil = Instantiate(lib.pupilPrefabs[UnityEngine.Random.Range(0, lib.pupilPrefabs.Count)],
            curentEye.transform);

            controller.pupil = pupil;
        }

        if (lib.headsPrefabs.Count > 0)
            Instantiate(lib.headsPrefabs[UnityEngine.Random.Range(0, lib.headsPrefabs.Count)],
                curentEye.transform);

        if (lib.footsPrefabs.Count > 0)
            Instantiate(lib.footsPrefabs[UnityEngine.Random.Range(0, lib.footsPrefabs.Count)],
            curentEye.transform);


            GameObject vens = Instantiate(curentDiagnosis.diseases[(int)Diseases.Vitamins]? lib.vensPrefabsDisease:lib.vensPrefabs,
            curentEye.transform);
            controller.capilares = vens;
        
        controller.diagnosis = curentDiagnosis;
    }

    public void SendDiagnosis(Diagnosis diagnosis)
    {
        int patientPrize = 0;
        int patientFine = 0;
        dailyPacients++;

        for (int i = 0; i < Enum.GetNames(typeof(Diseases)).Length; i++)
        {
            if (session == null) return;

            if (curentDiagnosis.diseases[i] == true)
            {
                // У пациента есть болезнь
                if (curentDiagnosis.diseases[i] == diagnosis.diseases[i])
                {
                    // Правильно диагностировали
                    patientPrize += prize;
                }
                else
                {
                    // Неправильно диагностировали
                    patientFine += Mathf.Abs(fine); // Берем абсолютное значение для отображения
                }
            }
            else
            {
                // У пациента нет болезни
                if (curentDiagnosis.diseases[i] == diagnosis.diseases[i])
                {
                    // Правильно определили отсутствие болезни
                    patientPrize += neutralPrize;
                }
                else
                {
                    // Неправильно определили болезнь (ложноположительный результат)
                    patientFine += Mathf.Abs(neutralFine); // Берем абсолютное значение для отображения
                }
            }
        }

        // Обновляем общую статистику за день
        dailyPrize += patientPrize;
        dailyFine += patientFine;

        // Обновляем общие деньги (учитываем, что fine и neutralFine отрицательные)
        session.curentMoney += patientPrize - patientFine;

        NewPacient();
    }

    private void CheckAndPayQuota()
    {
        // Уменьшаем счетчик дней до следующей квоты
        daysUntilNextQuota--;

        // Если пришло время платить квоту
        if (daysUntilNextQuota <= 0)
        {
            PayQuota();
        }
    }

    private void PayQuota()
    {
        session.curentMoney -= currentQuota;
        currentQuota += bonusQuota;
        daysUntilNextQuota = dayByQouta;
    }

    public (int currentQuota, int daysUntilNextQuota) GetQuotaInfo()
    {
        return (currentQuota, daysUntilNextQuota);
    }
}