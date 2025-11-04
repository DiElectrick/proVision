using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour
{

    public static TutorialController Instance { get; private set; }

    public IEnumerator TutorialPlay(TutorialInfo tutorial)
    {
        if(tutorial == null) yield break;

        DoorAnimator.Instance.HideInstantly();

        if (tutorial.diagnosis != null) {
            EyeGenerator.Instance.GenerateEye(tutorial.diagnosis);
            DoorAnimator.Instance.AnimateSprite(false);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < tutorial.tutorialSteps.Length; i++) { 
            var step = tutorial.tutorialSteps[i];
            if (step == null) continue;

            step.arrow.SetActive(true);

            yield return new WaitForSeconds(step.duration);

            step.arrow.SetActive(false);

        }


    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
