/*
 


 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcess : MonoBehaviour
{

    GameSession session;

    [SerializeField] Timer timer;
    [SerializeField] FinalePanelUIController statsPanel;
    [SerializeField] DoorAnimator doorAnimator;


    private int dailyPrize = 0;
    private int dailyFine = 0;
    private int dailyPacients = 0;

    bool tutorialIsGoing = false;

    private BalanceManager balanceManager;
    private EyeGenerator eyeGenerator;
    private TutorialController tutorController;

    Transform curentEye;

    bool error = false;

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

        NextDay();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        
        StartCoroutine(costCor2());

    }

    public void NextDay()
    {
        StartCoroutine(NextDayCoroutine());
    }

    IEnumerator FourCoroutine()
    {

        G.textPanel.ShowText("Твой рабочий день увеличен");
        yield return new WaitForSeconds(4);
    }

    IEnumerator ErrorCoroutine()
    {

        G.textPanel.ShowText("Вы ошиблись с прошлым диагнозом. Вам выписан штраф");
        yield return new WaitForSeconds(4);
    }

    private IEnumerator NextDayCoroutine()
    {

        AudioManager.Instance.PlayRandomMusic();

        session.curentDay++;
        dailyPrize = 0;
        dailyFine = 0;
        dailyPacients = 0;

        doorAnimator.HideInstantly();

        G.verdictController.Hide();
        statsPanel.Hide();

        if (session.curentDay == 4 || session.curentDay == 2) yield return FourCoroutine();

        TutorialInfo tutorial = balanceManager.Tutorial(session.curentDay);



        if (tutorial != null && G.tutorialIsActive && G.tutorialProgress < session.curentDay)
        {
            G.tutorialProgress = session.curentDay;

            if (session.curentDay == 6) { 
                G.tutorialIsActive = false;
            }

            G.verdictController.SetVariants(tutorial.diagnosis.diseases);

            // Используйте переменную из TutorialController
            TutorialController.Instance.tutorialIsGoing = true;

            DoorAnimator.Instance.HideInstantly();

            if (tutorial.diagnosis != null)
            {

                EyeGenerator.Instance.GenerateEye(tutorial.diagnosis);
                doorAnimator.AnimateSprite(false);
                AudioManager.Instance.PlayDoorSound();
            }

            yield return new WaitForSeconds(0.5f);

            tutorController.StartTutorial(tutorial);

            while (TutorialController.Instance.tutorialIsGoing)
            {
                yield return new WaitForSeconds(0.1f);
            }

            //doorAnimator.AnimateSprite(true);
            yield return new WaitForSeconds(0.5f);
            TutorialController.Instance.tutorialIsGoing = false;

        }
        else
        {
            NewPacient();
        }


        Debug.Log("end of tutor");


        if (timer != null)
        {
            if (session.curentDay == 1) timer.ResetAndStartTimer(20);
            else if (session.curentDay < 4) timer.ResetAndStartTimer(30);
            else timer.ResetAndStartTimer();
        }

    }


    void NewPacient()
    {


        Debug.Log("New pacient");
        // Проверяем через TutorialController
        if (TutorialController.Instance.tutorialIsGoing)
        {
            TutorialController.Instance.StopTutorial();
            TutorialController.Instance.tutorialIsGoing = false;
        }

        curentDiagnosis = GenerateDiagnosis(balanceManager.AvailableDiseases(session.curentDay), balanceManager.DiseasesNum(session.curentDay));
        G.curentDiagnosis = curentDiagnosis;

        eyeGenerator.GenerateEye(curentDiagnosis);
        AudioManager.Instance.PlayDoorSound();

        // Проверка перед анимацией
        if (doorAnimator != null && doorAnimator.targetTransform != null)
        {
            doorAnimator.AnimateSprite(false);
        }
        else
        {
            Debug.Log("Animator null");
        }

    }


    Diagnosis GenerateDiagnosis(List<Diseases> availableDiseases1, int diseasesNum)
    {
        Diagnosis diagnosis = new Diagnosis(); // Always create a new instance

        // Add null checks for input parameters
        if (availableDiseases1 == null || availableDiseases1.Count == 0)
        {
            Debug.LogWarning("No available diseases provided, returning empty diagnosis");
            return diagnosis;
        }

        List<Diseases> availableDiseases = new List<Diseases>(availableDiseases1);
        G.verdictController.SetVariants(availableDiseases);

        int n = UnityEngine.Random.Range(0, diseasesNum + 1);
        n = Mathf.Min(n, availableDiseases.Count);

        List<Diseases> selectedDiseases = new List<Diseases>();

        // Your existing disease selection logic...
        for (int i = 0; i < n; i++)
        {
            if (availableDiseases.Count == 0)
                break;

            int index = UnityEngine.Random.Range(0, availableDiseases.Count);
            Diseases selectedDisease = availableDiseases[index];
            selectedDiseases.Add(selectedDisease);
            availableDiseases.RemoveAt(index);
        }

        // Your existing disease application logic...
        foreach (Diseases disease in selectedDiseases)
        {
            Debug.Log($"Applying disease: {disease}");
            diagnosis.diseases[(int)disease] = true;

            if (disease == Diseases.LongRange)
            {
                if (selectedDiseases.Contains(Diseases.ShortRange))
                {
                    diagnosis.diseases[(int)Diseases.ShortRange] = false;
                }
            }
            else if (disease == Diseases.ShortRange)
            {
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
        if (!TutorialController.Instance.tutorialIsGoing)
        {
            dailyPacients++;

            // Используем баланс-менеджер для расчета награды
            int reward = balanceManager.CalculateDiagnosisReward(diagnosis, curentDiagnosis, ref error);

            if (reward >= 0)
            {
                dailyPrize += reward;
            }
            else
            {
                dailyFine += reward; // reward уже отрицательный
            }
        }
        StartCoroutine(costCor());

    }

    IEnumerator costCor()
    {
        doorAnimator.AnimateSprite(true);
        yield return new WaitForSeconds(0.5f);

        if (error) StartCoroutine(ErrorCoroutine());

        NewPacient();

    }

    IEnumerator costCor2()
    {
        yield return new WaitForSeconds(0.5f);
        NewPacient();

    }
}

