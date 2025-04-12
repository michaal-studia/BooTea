using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("--------------- Audio Source ---------------")]
    [SerializeField]
    AudioSource musicSource;
    [SerializeField]
    AudioSource SFXSource;
    [SerializeField]
    AudioSource voiceSource;

    [Header("--------------- Audio Clip ---------------")]
    public AudioClip backgroundMusic1;
    public AudioClip backgroundMusic2;
    public AudioClip backgroundMusic3;
    public AudioClip backgroundMusic4;
    public AudioClip teacupFilling;
    public AudioClip teacupPuttingDown;
    public AudioClip teaLeafsRustle;
    public AudioClip iceCubesClatter;
    public AudioClip NPCInteraction;
    public AudioClip NPCSpeaking; // it will be as in undertale where one really short sound sample is made in order to then be denoised, trimmed and have its pitch changed and repeated for each text sign that the npc is going to say making words for sentences
    public AudioClip footsteps;
    public AudioClip clientsChatter;
    public AudioClip wallTouch;
    public AudioClip teaReady;
    public AudioClip ingredientAdding;
    public AudioClip staircaseWalking;
    public AudioClip teaSlurping;
    public AudioClip stirringTea;
    public AudioClip teaKettleWhistling;
    public AudioClip catMeow;
    public AudioClip catPurr;
    public AudioClip dishesClatter;
    public AudioClip ghostHowlMale;
    public AudioClip ghostHowlFemale;
    public AudioClip steamHiss;
    public AudioClip bellChime; // jeden z najważnieszych
    public AudioClip pageFlip; // Przewracanie stron starej księgi (może do przepisu?)
    public AudioClip woodenFloorCreak;  // Skrzypienie podłogi
    public AudioClip candleFlicker;  // Dźwięk trzaskającego płomienia świecy
    public AudioClip rainOnWindow;  // Deszcz uderzający o szybę
    public AudioClip chairMove; // Przesuwanie drewnianego krzesła
    public AudioClip bookOpen; // Otwieranie starej książki z przepisami
    public AudioClip broomSweep; // Zamiatanie podłogi
    public AudioClip cashRegister; // Dźwięk monet (jeśli w grze jest system płatności)

    [Header("--------------- UI Elements ---------------")]
    [SerializeField]
    private Slider MusicSlider;
    [SerializeField]
    private Slider SFXSlider;
    [SerializeField]
    private Slider VoiceSlider;

    public static AudioManager Instance { get; private set; } // Singleton

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadVolumeSettings();

        StartCoroutine(LoopMusic());

        if (MusicSlider != null)
            MusicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (SFXSlider != null)
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);

        if (VoiceSlider != null)
            VoiceSlider.onValueChanged.AddListener(SetVoiceVolume);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!SFXSource.isPlaying) // Zapobiega nakładaniu się dźwięków
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    private IEnumerator LoopMusic()
    {
        AudioClip[] backgroundTracks = { backgroundMusic1, backgroundMusic2, backgroundMusic3, backgroundMusic4 };

        if (backgroundTracks.Length == 0)
        {
            Debug.LogError("❌ Brak przypisanych utworów! Ustaw backgroundMusic1-4 w Inspectorze.");
            yield break; // Zatrzymanie coroutine
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

    public void PlayBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        //Debug.Log(clip);
        musicSource.Play();
    }

    public bool IsSFXPlaying()
    {
        return SFXSource.isPlaying;
    }

    public void PlayFootstepSFX(float pitch)
    {
        SFXSource.pitch = pitch; // Ustaw losowy pitch
        SFXSource.PlayOneShot(footsteps); // Odtwórz dźwięk
    }

    public void PlayVoice(AudioClip clip, float pitch)
    {
        voiceSource.pitch = pitch;
        voiceSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetVoiceVolume(float volume)
    {
        voiceSource.volume = volume;
        PlayerPrefs.SetFloat("VoiceVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        float savedVoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.5f);

        musicSource.volume = savedMusicVolume;
        SFXSource.volume = savedSFXVolume;
        voiceSource.volume = savedVoiceVolume;

        if (MusicSlider != null)
            MusicSlider.value = savedMusicVolume;

        if (SFXSlider != null)
            SFXSlider.value = savedSFXVolume;

        if (VoiceSlider != null)
            VoiceSlider.value = savedVoiceVolume;
    }
}
