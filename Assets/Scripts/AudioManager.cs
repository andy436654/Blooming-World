using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Ключи сохранения
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private static AudioManager _instance;

    [SerializeField] private AudioMixer _audioMixer;

    // Параметры микшера (должны совпадать с названиями в AudioMixer)
    [SerializeField] private string _masterVolumeParam = "MasterVolume";
    [SerializeField] private string _musicVolumeParam = "MusicVolume";
    [SerializeField] private string _sfxVolumeParam = "SFXVolume";

    // Текущие значения громкости
    [Range(0, 1), SerializeField] private float _masterVolume = 0.7f;
    [Range(0, 1), SerializeField] private float _musicVolume = 0.7f;
    [Range(0, 1), SerializeField] private float _sfxVolume = 0.7f;

    public static float MasterVolume => _instance ? _instance._masterVolume : 0.7f;
    public static float MusicVolume => _instance ? _instance._musicVolume : 0.7f;
    public static float SFXVolume => _instance ? _instance._sfxVolume : 0.7f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllVolumes();
    }

    // === Методы для управления громкостью ===
    public static void SetMasterVolume(float volume) => _instance?.SetVolume(volume, MASTER_VOLUME_KEY, ref _instance._masterVolume, _instance._masterVolumeParam);
    public static void SetMusicVolume(float volume) => _instance?.SetVolume(volume, MUSIC_VOLUME_KEY, ref _instance._musicVolume, _instance._musicVolumeParam);
    public static void SetSFXVolume(float volume) => _instance?.SetVolume(volume, SFX_VOLUME_KEY, ref _instance._sfxVolume, _instance._sfxVolumeParam);

    private void SetVolume(float volume, string prefsKey, ref float volumeField, string mixerParam)
    {
        volumeField = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(prefsKey, volumeField);
        ApplyVolume(volumeField, mixerParam);
    }

    private void LoadAllVolumes()
    {
        _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 0.7f);
        _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.7f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);

        ApplyVolume(_masterVolume, _masterVolumeParam);
        ApplyVolume(_musicVolume, _musicVolumeParam);
        ApplyVolume(_sfxVolume, _sfxVolumeParam);
    }

    private void ApplyVolume(float volume, string mixerParam)
    {
        if (!_audioMixer) return;
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        _audioMixer.SetFloat(mixerParam, dB);
    }

    // Для проигрывания звуков с учётом громкости SFX
    public static void PlaySFX(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null || !_instance) return;
        AudioSource.PlayClipAtPoint(clip, position, SFXVolume * volumeScale);
    }

    // В класс AudioManager добавьте:
    public static void PlaySound(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        PlaySFX(clip, position, volumeScale);
    }
}