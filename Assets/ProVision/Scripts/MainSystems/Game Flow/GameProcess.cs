/*
 


 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour
{

    GameSession session;

    [SerializeField] Timer timer;
    [SerializeField] FinalePanelUIController statsPanel;
    [SerializeField] DoorAnimator doorAnimator;


    private int dailyPrize = 0;
    private int dailyFine = 0;
    private int dailyPacients = 0;


    private BalanceManager balanceManager;
    private EyeGenerator eyeGenerator;
    private TutorialController tutorController;

    Transform curentEye;


    Diagnosis curentDiagnosis = new Diagnosis();

    private void Start()
    {
        G.process = this;
        session = G.curentSession;
        timer = GetComponent<Timer>();
        balanceManager = GetComponent<BalanceManager>();
        eyeGenerator = GetComponent<EyeGenerator>();
        tutorController = GetComponent<TutorialController>();

        curentEye = eyeGenerator.curentEye.transform;


        if (session == null)
        {
            session = new GameSession();
            G.curentSession = session;
        }

        if (timer != null)
        {
            timer.OnTimerExpired += OnTimerExpired;
        }

        doorAnimator.HideInstantly();

        //NextDay();
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

    public void NextDay() {
        StartCoroutine(NextDayCoroutine());
    }

    public IEnumerator NextDayCoroutine()
    {
        AudioManager.Instance.PlayRandomMusic();

        session.curentDay++;

        TutorialInfo tutorial = balanceManager.Tutorial(session.curentDay);

        if (tutorial != null && G.tutorialIsActive && G.tutorialProgress < session.curentDay) {
            yield return TutorialController.Instance.TutorialPlay(tutorial);
        }

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
        curentDiagnosis = GenerateDiagnosis(balanceManager.AvailableDiseases(session.curentDay), balanceManager.DiseasesNum(session.curentDay));

        G.curentDiagnosis = curentDiagnosis;

        eyeGenerator.GenerateEye(curentDiagnosis);
        AudioManager.Instance.PlayDoorSound();
        doorAnimator.AnimateSprite(false);
    }

    Diagnosis GenerateDiagnosis(List<Diseases> availableDiseases1, int diseasesNum)
    {
        Diagnosis diagnosis = new Diagnosis();
        List<Diseases> availableDiseases = new List<Diseases>(availableDiseases1);

        G.verdictController.SetVariants(availableDiseases);

        int n = UnityEngine.Random.Range(0, diseasesNum + 1);
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


}

