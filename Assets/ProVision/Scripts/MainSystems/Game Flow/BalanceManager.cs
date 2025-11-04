using System;
using System.Collections.Generic;
using UnityEngine;

//Diseases[] progression = new Diseases[] {
//    Diseases.Capillaries,
//    Diseases.Rotation,
//    Diseases.Focus,
//    Diseases.Double,
//    Diseases.LongRange,
//    Diseases.ShortRange
//};


[System.Serializable]
public class DayBalanceData {
    public List<Diseases> addedDiseases = new List<Diseases>();
    public TutorialInfo tutorialInfo;
    public int maxDiseasesNum;
}

public class BalanceManager : MonoBehaviour
{
    [Header("Balance Settings")]
    [SerializeField] int dayByQouta = 3;
    [SerializeField] int startQouta = 300;
    [SerializeField] int bonusQuota = 100;
    [SerializeField] int prize = 100;
    [SerializeField] int fine = -50;
    [SerializeField] int neutralPrize = 10;
    [SerializeField] int neutralFine = -10;

    [SerializeField] GameObject loop;
    [SerializeField] GameObject deck;
    [SerializeField] GameObject lamp;

    [SerializeField] BalanceData balanceData;

    List<Diseases> allDisasesList = new List<Diseases>();

    // Переменные для квоты
    private int currentQuota = 0;
    private int daysUntilNextQuota = 0;

    private GameProcess gameProcess;
    private GameSession session;

    private void Awake()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Diseases)).Length; i++)
        {
            allDisasesList.Add((Diseases)i);
        }
    }

    private void Start()
    {
        gameProcess = G.process;
        session = G.curentSession;

        InitializeQuotaSystem();
    }

    public List<Diseases> AvailableDiseases(int dayN) { 
        List<Diseases> list = new List<Diseases>(balanceData.daysData[dayN-1 < balanceData.daysData.Count ? dayN-1 : balanceData.daysData.Count - 1].addedDiseases);
        return list;
    }

    public int MaxDiseasesNum(int dayN) {
        if (dayN < balanceData.daysData.Count) return balanceData.daysData[dayN - 1].maxDiseasesNum;
        else {
            return balanceData.daysData[balanceData.daysData.Count - 1].maxDiseasesNum;
        }
    }

    public TutorialInfo Tutorial(int dayN) { 
        return balanceData.daysData[dayN-1].tutorialInfo;
    }

    public int DiseasesNum(int dayN)
    {
        if (dayN <= 6) return 1;
        else return 2;
    }

    private void InitializeQuotaSystem()
    {
        currentQuota = startQouta;
        daysUntilNextQuota = dayByQouta;
    }

    public void ProcessDayResults(int patientsCount, int totalPrize, int totalFine)
    {
        // Обновляем общие деньги
        session.curentMoney += totalPrize + totalFine; // fine уже отрицательный

        // Проверяем и списываем квоту
        daysUntilNextQuota--;
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

    public int CalculateDiagnosisReward(Diagnosis playerDiagnosis, Diagnosis correctDiagnosis)
    {
        int patientPrize = 0;
        int patientFine = 0;

        for (int i = 0; i < Enum.GetNames(typeof(Diseases)).Length; i++)
        {
            if (correctDiagnosis.diseases[i] == true)
            {
                // У пациента есть болезнь
                if (correctDiagnosis.diseases[i] == playerDiagnosis.diseases[i])
                {
                    // Правильно диагностировали
                    patientPrize += prize;
                }
                else
                {
                    // Неправильно диагностировали
                    patientFine += fine;
                }
            }
            else
            {
                // У пациента нет болезни
                if (correctDiagnosis.diseases[i] == playerDiagnosis.diseases[i])
                {
                    // Правильно определили отсутствие болезни
                    patientPrize += neutralPrize;
                }
                else
                {
                    // Неправильно определили болезнь
                    patientFine += neutralFine;
                }
            }
        }

        return patientPrize + patientFine; // возвращаем общий результат
    }

    public (int currentQuota, int daysUntilNextQuota) GetQuotaInfo()
    {
        return (currentQuota, daysUntilNextQuota);
    }

}