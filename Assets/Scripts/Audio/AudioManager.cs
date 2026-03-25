using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Источники звука")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Музыка")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Звуки")]
    public AudioClip plantSound;
    public AudioClip harvestSound;
    public AudioClip upgradeSound;
    public AudioClip buttonSound;
    public AudioClip watermelonHarvestSound;

    protected override void Awake()
    {
        base.Awake();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        LoadVolume();
    }

    public static void EnsureExists()
    {
        if (Instance != null) return;

        // Проверяем, нет ли уже AudioManager в сцене
        AudioManager existing = Object.FindFirstObjectByType<AudioManager>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        GameObject obj = new GameObject("AudioManager");
        AudioManager am = obj.AddComponent<AudioManager>();
        
        // Создаем AudioSource если их нет
        if (am.musicSource == null)
        {
            am.musicSource = obj.AddComponent<AudioSource>();
            am.musicSource.loop = true;
            am.musicSource.playOnAwake = false;
        }
        if (am.sfxSource == null)
        {
            am.sfxSource = obj.AddComponent<AudioSource>();
            am.sfxSource.loop = false;
            am.sfxSource.playOnAwake = false;
            // Для WebGL важно: отключаем spatial blend для 2D звуков
            am.sfxSource.spatialBlend = 0f;
        }
    }

    void LoadVolume()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        if (sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public float GetMusicVolume() => musicSource.volume;
    public float GetSFXVolume() => sfxSource.volume;

    public void PlayButtonSound()
    {
        PlaySFX(buttonSound);
    }

    public void PlayWatermelonHarvestSound()
    {
        if (watermelonHarvestSound == null) return;
        PlaySFX(watermelonHarvestSound);
    }

    public void PlayUpgradeSound()
    {
        if (upgradeSound == null) return;
        PlaySFX(upgradeSound);
    }

    public void PlayPlantSound()
    {
        if (plantSound == null) return;
        PlaySFX(plantSound);
    }
}