using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;


namespace UI
{
    [Binding]
    public class DisplayOptionViewModel : ViewModel
    {
        private int _resolutionIndex;
        private float _brightness;
        private bool _showDamage;
        private bool _fullScreen;
        private Resolution[] _resolutions;
        private string[] _resolutionOptions;
        private string _currentResolutionText;
        private string _brightnessText;

        [Binding]
        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                _resolutionIndex = value;
                OnPropertyChanged(nameof(ResolutionIndex));
                ApplyResolution();
            }
        }

        [Binding]
        public string[] ResolutionOptions
        {
            get => _resolutionOptions;
            set
            {
                _resolutionOptions = value;
                OnPropertyChanged(nameof(ResolutionOptions));
            }
        }

        [Binding]
        public float Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged(nameof(Brightness));
                ApplyBrightness();
            }
        }

        [Binding]
        public bool ShowDamage
        {
            get => _showDamage;
            set
            {
                _showDamage = value;
                ShowDamageText = _showDamage ? "데미지 표시 : ON" : "데미지 표시 : OFF";
                OnPropertyChanged(nameof(ShowDamage));
            }
        }
        private string _showDamageText;
        [Binding]
        public string ShowDamageText
        {
            get => _showDamageText;
            set
            {
                _showDamageText = value;
                OnPropertyChanged(nameof(ShowDamageText));
            }
        }

        [Binding]
        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                _fullScreen = value;
                OnPropertyChanged(nameof(FullScreen));
                ApplyFullScreen();
            }
        }

        [Binding]
        public string CurrentResolutionText
        {
            get => _currentResolutionText;
            set
            {
                _currentResolutionText = value;
                OnPropertyChanged(nameof(CurrentResolutionText));
            }
        }

        [Binding]
        public string BrightnessText
        {
            get => _brightnessText;
            set
            {
                _brightnessText = value;
                OnPropertyChanged(nameof(BrightnessText));
            }
        }

        private void OnEnable()
        {
            InitializeResolutions();
            LoadSettings();
        }

        private void InitializeResolutions()
        {
            _resolutions = Screen.resolutions;
            
            if (_resolutions == null || _resolutions.Length == 0)
            {
                _resolutions = new Resolution[] 
                {
                    new Resolution { width = 1280, height = 720 },
                    new Resolution { width = 1920, height = 1080 }
                };
                Debug.LogWarning("Screen.resolutions가 비어있습니다. 기본 해상도를 사용합니다.");
            }
            
            _resolutionOptions = new string[_resolutions.Length];

            for (int i = 0; i < _resolutions.Length; i++)
            {
                _resolutionOptions[i] = $"{_resolutions[i].width}x{_resolutions[i].height}";
            }

            OnPropertyChanged(nameof(ResolutionOptions));

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }

            _resolutionIndex = currentResolutionIndex;
            UpdateCurrentResolutionText();
        }

        private void UpdateCurrentResolutionText()
        {
            if (_resolutions == null || _resolutions.Length == 0)
            {
                CurrentResolutionText = "해상도 없음";
                return;
            }
            
            if (_resolutionIndex >= 0 && _resolutionIndex < _resolutions.Length)
            {
                CurrentResolutionText = _resolutionOptions[_resolutionIndex];
            }
            else
            {
                CurrentResolutionText = "잘못된 해상도";
            }
        }

        private void ApplyResolution()
        {
            if (_resolutions == null || _resolutions.Length == 0)
            {
                Debug.LogError("해상도 배열이 초기화되지 않았습니다.");
                return;
            }
            
            if (_resolutionIndex >= 0 && _resolutionIndex < _resolutions.Length)
            {
                Resolution resolution = _resolutions[_resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                UpdateCurrentResolutionText();
            }
            else
            {
                Debug.LogError($"해상도 인덱스가 범위를 벗어났습니다: {_resolutionIndex}, 배열 크기: {_resolutions.Length}");
            }
        }

        private void ApplyBrightness()
        {
            if (Global.UIManager != null)
            {
                Global.UIManager.SetBrightness(_brightness);
                UpdateBrightnessText();
            }
        }

        private void UpdateBrightnessText()
        {
            BrightnessText = $"밝기 : {_brightness}";
        }

        [Binding]
        public void OnClickBrightness()
        {
            // 밝기 값을 단계별로 변경 (0, 25, 50, 75, 100)
            if (_brightness < 25)
                _brightness = 25;
            else if (_brightness < 50)
                _brightness = 50;
            else if (_brightness < 75)
                _brightness = 75;
            else if (_brightness < 100)
                _brightness = 100;
            else
                _brightness = 0;
            
            OnPropertyChanged(nameof(Brightness));
            ApplyBrightness();
        }

        private void ApplyFullScreen()
        {
            Screen.fullScreen = _fullScreen;
        }

        [Binding]
        public void OnClickFullScreen()
        {
            FullScreen = !FullScreen;
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        [Binding]
        public void OnClickShowDamage()
        {
            ShowDamage = !ShowDamage;
            GameManager.DamageTextPoolManager.showDamageText = ShowDamage;
            PlayerPrefs.SetInt("ShowDamage", ShowDamage ? 1 : 0);
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        [Binding]
        public void OnClickResolution()
        {
            _resolutionIndex = (_resolutionIndex + 1) % _resolutions.Length;
            OnPropertyChanged(nameof(ResolutionIndex));
            ApplyResolution();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        [Binding]
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
            PlayerPrefs.SetFloat("Brightness", _brightness);
            PlayerPrefs.SetInt("ShowDamage", _showDamage ? 1 : 0);
            PlayerPrefs.SetInt("FullScreen", _fullScreen ? 1 : 0);
            PlayerPrefs.Save();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        private void LoadSettings()
        {
            ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            Brightness = PlayerPrefs.GetFloat("Brightness", 50f);
            ShowDamage = PlayerPrefs.GetInt("ShowDamage", 1) == 1;
            FullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;

            if (_resolutions != null && _resolutions.Length > 0)
            {
                ApplyResolution();
            }
            
            ApplyBrightness();
            ApplyFullScreen();
            
            if (GameManager.DamageTextPoolManager != null)
            {
                GameManager.DamageTextPoolManager.showDamageText = _showDamage;
            }
        }
    }
}
