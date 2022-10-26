using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

    [Header("Brightness Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;

    [Header("Quality Level Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Sensitivity Setting")]
    [SerializeField] private TMP_Text mouseSensitivityText = null;
    [SerializeField] private Slider mouseSensitivitySlider = null;

    [Header("Invert Y Setting")]
    [SerializeField] private Toggle invertYToggle = null;

    void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                AudioListener.volume = localVolume;
                volumeSlider.value = localVolume;
                volumeTextValue.text = localVolume.ToString("0.0");
            }
            else
            {
                menuController.ResetButton("Audio");
            }

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");

                QualitySettings.SetQualityLevel(localQuality);
                qualityDropdown.value = localQuality;
            }

            if (PlayerPrefs.HasKey("masterFullScreen"))
            {
                int localFullScreen = PlayerPrefs.GetInt("masterFullScreen");

                if (localFullScreen == 1)
                {
                    Screen.fullScreen = true;
                    fullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    fullScreenToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                brightnessSlider.value = localBrightness;
                brightnessTextValue.text = localBrightness.ToString("0.0");
            }

            if (PlayerPrefs.HasKey("masterSensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

                mouseSensitivitySlider.value = localSensitivity;
                mouseSensitivityText.text = localSensitivity.ToString("0.0");
            }

            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if (PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    invertYToggle.isOn = true;
                }
                else
                {
                    invertYToggle.isOn = false;
                }
            }
        }
    }
}