using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public bool tutorialIsGoing = false;
    public static TutorialController Instance { get; private set; }

    public void StartTutorial(TutorialInfo tutorial)
    {
        StartCoroutine(TutorialPlay(tutorial));
    }

    public void StopTutorial()
    {
        StopAllCoroutines();
        ArrowManager.Instance.HideAllArrows();
        G.textPanel.ShowText("Теперь попробуй сам", 2f);
    }

    public IEnumerator TutorialPlay(TutorialInfo tutorial)
    {
        if (tutorial == null) yield break;

        Debug.Log("Start tut");

        for (int i = 0; i < tutorial.tutorialSteps.Count; i++)
        {
            var step = tutorial.tutorialSteps[i];
            if (step == null) continue;

            if (step.arrow != Arrows.None) ArrowManager.Instance.ShowArrow(step.arrow);

            if (step.message != "")
            {
                G.textPanel.ShowText(step.message, step.duration - 0.5f);
            }

            yield return new WaitForSeconds(step.duration);

            if (step.arrow != Arrows.None) ArrowManager.Instance.HideArrow(step.arrow);

        }

        StopTutorial();


    }

    private void Awake()
    {

        Instance = this;
    }
}
