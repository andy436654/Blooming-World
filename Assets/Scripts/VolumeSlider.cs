using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }

    [SerializeField] private VolumeType volumeType = VolumeType.Master;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.minValue = 0;
        _slider.maxValue = 1;
    }

    private void Start()
    {
        // ”станавливаем начальное значение слайдера
        _slider.value = GetCurrentVolume();
        _slider.onValueChanged.AddListener(HandleVolumeChanged);
    }

    private float GetCurrentVolume()
    {
        return volumeType switch
        {
            VolumeType.Master => AudioManager.MasterVolume,
            VolumeType.Music => AudioManager.MusicVolume,
            VolumeType.SFX => AudioManager.SFXVolume,
            _ => 1f
        };
    }

    private void HandleVolumeChanged(float volume)
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                AudioManager.SetMasterVolume(volume);
                break;
            case VolumeType.Music:
                AudioManager.SetMusicVolume(volume);
                break;
            case VolumeType.SFX:
                AudioManager.SetSFXVolume(volume);
                break;
        }
    }

    private void OnDestroy()
    {
        if (_slider != null)
            _slider.onValueChanged.RemoveListener(HandleVolumeChanged);
    }
}