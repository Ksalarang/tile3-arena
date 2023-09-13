using System;
using Services.Sounds;
using Services.Vibrations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Utils.Windows {
public class SettingsWindow : Window {
    [SerializeField] Button closeButton;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider vibrationSlider;

    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;

    void Awake() {
        closeButton.onClick.AddListener(() => animateHide());
        soundSlider.onValueChanged.AddListener(onSoundVolumeChanged);
        musicSlider.onValueChanged.AddListener(onMusicVolumeChanged);
        soundSlider.value = soundService.getSoundVolume();
        musicSlider.value = soundService.getMusicVolume();
        if (vibrationService.supportsVibration()) {
            vibrationSlider.onValueChanged.AddListener(onVibrationValueChanged);
            vibrationSlider.value = vibrationService.isVibrationEnabled() ? 1 : 0;
        } else {
            vibrationSlider.transform.parent.gameObject.SetActive(false);
        }
    }
    
    void onSoundVolumeChanged(float value) {
        soundService.setSoundVolume(value);
    }

    void onMusicVolumeChanged(float value) {
        soundService.setMusicVolume(value);
    }

    void onVibrationValueChanged(float value) {
        if (value == 1) {
            vibrationService.setVibrationEnabled(true);
            vibrationService.vibrate(VibrationType.Light);
        } else {
            vibrationService.setVibrationEnabled(false);
        }
    }

    public void animateShow(Action action = null) {
        show(FadeDuration, action);
    }
    
    public void animateHide(Action action = null) {
        hide(FadeDuration, action);
    }
}
}