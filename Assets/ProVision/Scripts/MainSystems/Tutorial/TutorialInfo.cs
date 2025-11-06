using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialInfo", menuName = "Scriptable Objects/TutorialInfo")]
public class TutorialInfo : ScriptableObject
{
    [SerializeField] public Diagnosis diagnosis;
    [SerializeField] public List<TutorialStep> tutorialSteps;
}
