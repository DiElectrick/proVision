using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "BalanceData", menuName = "Scriptable Objects/BalanceData")]
public class BalanceData : ScriptableObject
{
   public List<DayBalanceData> daysData;
}
