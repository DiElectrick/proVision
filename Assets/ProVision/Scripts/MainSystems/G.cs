using UnityEngine;

public static class G
{
    public static GameSession curentSession;
    public static GameProcess process;
    public static Diagnosis curentDiagnosis;
    public static TextPanel textPanel;
    public static bool isRunning = true;

    public static bool tutorialIsActive = true;
    public static int tutorialProgress = 0;

    public static VerdictUIController verdictController;

}
