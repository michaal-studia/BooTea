using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private static AudioSource audioSource;
    private static AudioSource randomPitchAudioSource;
    private static AudioSource voiceAudioSource;
    private static AudioLibrary audioLibrary;
    private static AudioSource musicSource;

    private Slider MusicSlider;
    private Slider SFXSlider;
    private Slider VoiceSlider;

    private void Awake()
    {
        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);

            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSource = audioSources[0];
            randomPitchAudioSource = audioSources[1];
            voiceAudioSource = audioSources[2];
            musicSource = audioSources[3];
            audioLibrary = GetComponent<AudioLibrary>();
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines(); // zatrzymaj poprzednią muzykę

        if (scene.name == "StartScene")
        {
            StartCoroutine(LoopMusic("MainMenuBackgroundMusic"));
        }
        else if (scene.name == "SampleScene")
        {
            StartCoroutine(LoopMusic("BackgroundMusic"));
        }

        // Spróbuj przypisać suwaki, jeśli są obecne w scenie
        TryAssignSlidersFromScene();
    }

    private void TryAssignSlidersFromScene()
    {
        Slider music = GameObject.Find("MusicSlider")?.GetComponent<Slider>();
        Slider sfx = GameObject.Find("SFXSlider")?.GetComponent<Slider>();
        Slider voice = GameObject.Find("VoiceSlider")?.GetComponent<Slider>();

        if (music != null && sfx != null && voice != null)
        {
            AssignSliders(music, sfx, voice);
        }
    }

    public void AssignSliders(Slider music, Slider sfx, Slider voice)
    {
        MusicSlider = music;
        SFXSlider = sfx;
        VoiceSlider = voice;

        LoadVolumeSettings(); // Od razu ustawia suwaki na właściwe wartości

        if (MusicSlider != null)
            MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(MusicSlider.value); });

        if (SFXSlider != null)
            SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(SFXSlider.value); });

        if (VoiceSlider != null)
            VoiceSlider.onValueChanged.AddListener(delegate { SetVoiceVolume(VoiceSlider.value); });
    }

    public static void Play(string soundName, bool randomPitch = false)
    {
        AudioClip audioClip = audioLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            if (randomPitch)
            {
                randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                randomPitchAudioSource.PlayOneShot(audioClip);
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

    }
    private void Start()
    {
        LoadVolumeSettings();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "StartScene") // Main Menu
        {
            StartCoroutine(LoopMusic("MainMenuBackgroundMusic"));
        }
        else if (currentScene == "SampleScene") // Game
        {
            StopMusic(); // zatrzymaj poprzednią muzykę
            StartCoroutine(LoopMusic("BackgroundMusic"));
        }


        if (MusicSlider != null)
            MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(MusicSlider.value); });

        if (SFXSlider != null)
            SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(SFXSlider.value); });

        if (VoiceSlider != null)
            VoiceSlider.onValueChanged.AddListener(delegate { SetVoiceVolume(VoiceSlider.value); });
    }

    private IEnumerator LoopMusic(string music)
    {
        AudioClip[] backgroundTracks = audioLibrary.GetClips(music);

        Debug.Log($"Ładowanie muzyki: {music}, ilość utworów: {backgroundTracks.Length}");

        if (backgroundTracks.Length == 0)
        {
            Debug.LogError("Brak przypisanych utworów! Ustaw backgroundMusic1 - 4 w Inspectorze.");
            yield break;
        }

        while (true)
        {
            musicSource.clip = backgroundTracks[Random.Range(0, backgroundTracks.Length)];
            musicSource.Play();
            yield return new WaitForSeconds(musicSource.clip.length);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public static void PlayBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public static void PlayVoice(AudioClip clip, float pitch)
    {
        voiceAudioSource.pitch = pitch * Random.Range(0.95f, 1.15f);
        voiceAudioSource.PlayOneShot(clip);
    }

    public static void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioSource.volume = volume;
        randomPitchAudioSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetVoiceVolume(float volume)
    {
        voiceAudioSource.volume = volume;
        PlayerPrefs.SetFloat("VoiceVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        float savedVoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.5f);

        musicSource.volume = savedMusicVolume;
        audioSource.volume = savedSFXVolume;
        randomPitchAudioSource.volume = savedSFXVolume;
        voiceAudioSource.volume = savedVoiceVolume;

        if (MusicSlider != null)
            MusicSlider.value = savedMusicVolume;

        if (SFXSlider != null)
            SFXSlider.value = savedSFXVolume;

        if (VoiceSlider != null)
            VoiceSlider.value = savedVoiceVolume;
    }

    public static AudioClip[] GetAudioClipsFromLibrary(string groupName) // dla PianoNPC
    {
        return audioLibrary.GetClips(groupName);
    }
}
