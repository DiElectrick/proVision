
using System;
using System.Collections;
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

    [SerializeField] Timer timer;
    [SerializeField] FinalePanelUIController statsPanel;
    [SerializeField] DoorAnimator doorAnimator;

    // Переменные для хранения статистики за день
    private int dailyPrize = 0;
    private int dailyFine = 0;
    private int dailyPacients = 0;

    private BalanceManager balanceManager;

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
        balanceManager = GetComponent<BalanceManager>(); // или FindObjectOfType<BalanceManager>()

        if (session == null)
        {
            session = new GameSession();
            G.curentSession = session;
        }

        if (timer != null)
        {
            timer.OnTimerExpired += OnTimerExpired;
        }

        NextDay();
    }

    private void OnDestroy()
    {
        if (timer != null)
        {
            timer.OnTimerExpired -= OnTimerExpired;
        }
    }

    private void OnTimerExpired()
    {
        AudioManager.Instance.PlayTimerSound();
        AudioManager.Instance.StopMusicWithFade();
        // Передаем результаты дня в баланс-менеджер
        balanceManager.ProcessDayResults(dailyPacients, dailyPrize, dailyFine);

        var quotaInfo = balanceManager.GetQuotaInfo();
        statsPanel.Show();
        statsPanel.ShowStats(session.curentDay, dailyPacients, dailyPrize, dailyFine,
                           session.curentMoney, quotaInfo.daysUntilNextQuota, quotaInfo.currentQuota);
    }

    public void NextDay()
    {
        AudioManager.Instance.PlayRandomMusic();

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
        if (G.tutorialIsActive) {
            curentDiagnosis = GenerateDiagnosis(balanceManager.AvailableDiseases(session.curentDay), balanceManager.DiseasesNum(session.curentDay));
        }
        else {
            curentDiagnosis = GenerateDiagnosis(allDisasesList, balanceManager.DiseasesNum(7));
        }
            

        G.curentDiagnosis = curentDiagnosis;
        StartCoroutine(GeneratorCoroutine());
    }

    Diagnosis GenerateDiagnosis(List<Diseases> availableDiseases1, int diseasesNum)
    {
        Diagnosis diagnosis = new Diagnosis();

        // Создаем копию списка
        List<Diseases> availableDiseases = new List<Diseases>(availableDiseases1);

        G.verdictController.SetVariants(availableDiseases);

        int n = UnityEngine.Random.Range(0, diseasesNum + 1);
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
            if (disease == Diseases.LongRange)
            {
                // Убираем противоположную болезнь если она была выбрана
                if (selectedDiseases.Contains(Diseases.ShortRange))
                {
                    diagnosis.diseases[(int)Diseases.ShortRange] = false;
                }
            }
            else if (disease == Diseases.ShortRange)
            {
                // Убираем противоположную болезнь если она была выбрана
                if (selectedDiseases.Contains(Diseases.LongRange))
                {
                    diagnosis.diseases[(int)Diseases.LongRange] = false;
                }
            }
        }

        return diagnosis;
    }

    IEnumerator GeneratorCoroutine()
    {
        doorAnimator.AnimateSprite(curentEye.transform, true);

        yield return new WaitForSeconds(0.5f);

        GenerateEye();

        AudioManager.Instance.PlayDoorSound();

        doorAnimator.AnimateSprite(curentEye.transform, false);

        yield return new WaitForSeconds(0.5f);
    }

    void GenerateEye()
    {

        foreach (Transform child in curentEye.transform)
        {
            if (child.gameObject.tag != "notDelete") Destroy(child.gameObject);
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


        GameObject vens = Instantiate(curentDiagnosis.diseases[(int)Diseases.Capillaries] ? lib.vensPrefabsDisease : lib.vensPrefabs,
        curentEye.transform);
        controller.capilares = vens;

        controller.diagnosis = curentDiagnosis;
    }

    public void SendDiagnosis(Diagnosis diagnosis)
    {
        dailyPacients++;

        // Используем баланс-менеджер для расчета награды
        int reward = balanceManager.CalculateDiagnosisReward(diagnosis, curentDiagnosis);

        if (reward >= 0)
        {
            dailyPrize += reward;
        }
        else
        {
            dailyFine += reward; // reward уже отрицательный
        }

        NewPacient();
    }

    // GenerateDiagnosis, GeneratorCoroutine, GenerateEye методы остаются без изменений
}

