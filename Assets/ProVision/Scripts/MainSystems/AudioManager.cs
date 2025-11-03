using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private float fadeDuration = 1f;

    [Header("UI Sounds")]
    public AudioClip paperShowSound;
    public AudioClip buttonClickSound;
    public AudioClip doorSound;
    public AudioClip timerSound;

    private AudioClip currentMusic;
    private bool isMusicPlaying = false;
    private Coroutine fadeCoroutine;

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

    private void Start()
    {
        // Автоматически начинаем музыку при старте
        if (musicTracks.Length > 0)
        {
            PlayRandomMusic();
        }
    }

    #region Music System

    public void PlayRandomMusic()
    {
        if (musicTracks.Length == 0) return;

        AudioClip randomTrack;
        do
        {
            randomTrack = musicTracks[Random.Range(0, musicTracks.Length)];
        }
        while (randomTrack == currentMusic && musicTracks.Length > 1);

        PlayMusicWithFade(randomTrack, true);
    }

    public void PlayMusicWithFade(AudioClip music, bool loop = true)
    {
        if (music == null || music == currentMusic) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewMusic(music, loop));
    }

    public void StopMusicWithFade()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeToNewMusic(AudioClip newMusic, bool loop)
    {
        // Если музыка уже играет - плавно убавляем
        if (isMusicPlaying && musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
        }

        // Переключаем на новый трек
        currentMusic = newMusic;
        musicSource.clip = newMusic;
        musicSource.loop = loop;
        musicSource.volume = 0f; // Начинаем с нуля
        musicSource.Play();
        isMusicPlaying = true;

        // Плавно увеличиваем громкость
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
    }

    private IEnumerator FadeOutMusic()
    {
        if (!isMusicPlaying) yield break;

        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        isMusicPlaying = false;
        currentMusic = null;
    }

    // Твой оригинальный метод для прямого воспроизведения
    public void PlayMusic(AudioClip music, bool loop = true)
    {
        musicSource.clip = music;
        musicSource.loop = loop;
        musicSource.Play();
        currentMusic = music;
        isMusicPlaying = true;
    }

    #endregion

    #region SFX & UI Sounds

    // Для UI звуков
    public void PlayUIShow() => PlaySFX(paperShowSound);
    public void PlayDoorSound() => PlaySFX(doorSound);
    public void PlayTimerSound() => PlaySFX(timerSound);
    public void PlayButtonClick() => PlaySFX(buttonClickSound);


    // Для общих SFX
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    #endregion

    #region Volume Control

    public void SetMusicVolume(float volume) => musicSource.volume = volume;
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }

    #endregion

    private void Update()
    {
        // Автопереход к следующему случайному треку, когда музыка заканчивается
        if (isMusicPlaying && !musicSource.isPlaying && !musicSource.loop && musicTracks.Length > 0)
        {
            PlayRandomMusic();
        }
    }
}