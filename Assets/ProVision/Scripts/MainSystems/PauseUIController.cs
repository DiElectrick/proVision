using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    [SerializeField] Slider musicScrollbar;
    [SerializeField] Slider sfxScrollbar;
    [SerializeField] Toggle tutorialToggle;

    private void Start()
    {
        pausePanel.SetActive(!G.isRunning);

        musicScrollbar.value = AudioManager.Instance.GetMusicVolume();
        sfxScrollbar.value = AudioManager.Instance.GetSFXVolume();

        musicScrollbar.onValueChanged.AddListener(MusicSetValue);
        sfxScrollbar.onValueChanged.AddListener(SFXSetValue);


        tutorialToggle.isOn = G.tutorialIsActive;
        tutorialToggle.onValueChanged.AddListener(TutorialChanger);

    }

    void TutorialChanger(bool value)
    {
        G.tutorialIsActive = value;
        if (value == true)
        {
            G.tutorialProgress = 0;

        }
    }
    void MusicSetValue(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    void SFXSetValue(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !G.verdictController._visible)
        {
            G.isRunning = !G.isRunning;
            pausePanel.SetActive(!G.isRunning);
        }
    }
}
