using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // 1. 필드 수정
        private FullScreenMode[] _fullScreenModes = new[]
        {
            FullScreenMode.Windowed,
            FullScreenMode.ExclusiveFullScreen
        };
        private int _fullScreenModeIndex = 0;

        [Binding]
        public int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                if (_resolutionIndex != value)
                {
                    _resolutionIndex = value;
                    OnPropertyChanged(nameof(ResolutionIndex));
                    ApplyResolution();
                }
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
                SaveSettings(); // 변경 즉시 저장
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
            InitializeResolutions(); // 해상도 목록만 초기화 (인덱스 세팅 X)
            LoadSettings();          // 저장된 값만 적용
        }

        private void InitializeResolutions()
        {
            // 모든 해상도 가져오기
            Resolution[] allResolutions = Screen.resolutions;
            
            if (allResolutions == null || allResolutions.Length == 0)
            {
                // 원하는 해상도 프리셋 직접 추가
                allResolutions = new Resolution[]
                {
                    new Resolution { width = 640, height = 480 },    // SD
                    new Resolution { width = 800, height = 600 },    // SD
                    new Resolution { width = 1024, height = 768 },   // XGA
                    new Resolution { width = 1280, height = 720 },   // HD
                    new Resolution { width = 1280, height = 800 },
                    new Resolution { width = 1366, height = 768 },
                    new Resolution { width = 1440, height = 900 },
                    new Resolution { width = 1600, height = 900 },
                    new Resolution { width = 1680, height = 1050 },
                    new Resolution { width = 1920, height = 1080 },  // FHD
                    new Resolution { width = 1920, height = 1200 },
                    new Resolution { width = 2048, height = 1280 },
                    new Resolution { width = 2560, height = 1440 },  // QHD
                    new Resolution { width = 2560, height = 1600 },
                    new Resolution { width = 2880, height = 1800 },
                    new Resolution { width = 3840, height = 2160 },  // UHD(4K)
                };
                Debug.LogWarning("Screen.resolutions가 비어있습니다. 프리셋 해상도를 사용합니다.");
            }
            
            // 중복 제거 및 필터링 (너무 낮은 해상도 제외)
            var filteredResolutions = allResolutions
                .GroupBy(r => $"{r.width}x{r.height}") // 중복 제거
                .Select(g => g.First()) // 첫 번째 항목 선택
                .OrderByDescending(r => r.width * r.height) // 해상도 높은 순으로 정렬
                .ToArray();
            
            _resolutions = filteredResolutions;
            _resolutionOptions = new string[_resolutions.Length];

            for (int i = 0; i < _resolutions.Length; i++)
            {
                _resolutionOptions[i] = $"{_resolutions[i].width}x{_resolutions[i].height}";
            }

            OnPropertyChanged(nameof(ResolutionOptions));
            
            Debug.Log($"사용 가능한 해상도: {string.Join(", ", _resolutionOptions)}");
            Debug.Log($"현재 해상도 인덱스: {_resolutionIndex}, 해상도: {_resolutionOptions[_resolutionIndex]}");
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
            if (_resolutions == null || _resolutions.Length == 0) return;
            if (_resolutionIndex >= 0 && _resolutionIndex < _resolutions.Length)
            {
                Resolution resolution = _resolutions[_resolutionIndex];
                // 반드시 ViewModel의 _fullScreen 사용!
                Screen.SetResolution(resolution.width, resolution.height, _fullScreen);
                UpdateCurrentResolutionText();
                SaveSettings(); // 변경 즉시 저장
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
            SaveSettings(); // 변경 즉시 저장
            ApplyBrightness();
        }

        private void ApplyFullScreen()
        {
            Screen.fullScreen = _fullScreen;
        }

        // 2. OnClickFullScreen 그대로 사용
        [Binding]
        public void OnClickFullScreen()
        {
            // 다음 모드로 순환
            _fullScreenModeIndex = (_fullScreenModeIndex + 1) % _fullScreenModes.Length;

            if (_fullScreenModes[_fullScreenModeIndex] == FullScreenMode.Windowed)
            {
                Screen.fullScreen = false; // 반드시 먼저 false!
                Screen.fullScreenMode = FullScreenMode.Windowed;

                // 창 크기 재설정 (현재 선택된 해상도 사용)
                if (_resolutions != null && _resolutions.Length > 0 && _resolutionIndex >= 0)
                {
                    var res = _resolutions[_resolutionIndex];
                    Screen.SetResolution(res.width, res.height, false);
                }
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.fullScreen = true;
            }

            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            SaveSettings(); // 변경 즉시 저장
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
            // 다음 해상도로 순환
            _resolutionIndex = (_resolutionIndex + 1) % _resolutions.Length;
            OnPropertyChanged(nameof(ResolutionIndex));
            ApplyResolution();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        // 새로운 메서드: 특정 해상도 인덱스로 직접 설정 (드롭다운용)
        [Binding]
        public void SetResolutionByIndex(int index)
        {
            if (index >= 0 && index < _resolutions.Length)
            {
                _resolutionIndex = index;
                OnPropertyChanged(nameof(ResolutionIndex));
                ApplyResolution();
                Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            }
        }

        // 새로운 메서드: 해상도 증가 (화살표 버튼용)
        [Binding]
        public void IncreaseResolution()
        {
            if (_resolutions.Length > 1)
            {
                _resolutionIndex = (_resolutionIndex + 1) % _resolutions.Length;
                OnPropertyChanged(nameof(ResolutionIndex));
                ApplyResolution();
                Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            }
        }

        // 새로운 메서드: 해상도 감소 (화살표 버튼용)
        [Binding]
        public void DecreaseResolution()
        {
            if (_resolutions.Length > 1)
            {
                _resolutionIndex = (_resolutionIndex - 1 + _resolutions.Length) % _resolutions.Length;
                OnPropertyChanged(nameof(ResolutionIndex));
                ApplyResolution();
                Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            }
        }

        // 드롭다운에서 선택된 값이 변경될 때 호출되는 메서드
        [Binding]
        public void OnResolutionDropdownChanged(int selectedIndex)
        {
            SetResolutionByIndex(selectedIndex);
        }

        [Binding]
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
            PlayerPrefs.SetFloat("Brightness", _brightness > 0 ? _brightness : 50);
            PlayerPrefs.SetInt("ShowDamage", _showDamage ? 1 : 0);
            PlayerPrefs.SetInt("FullScreen", _fullScreen ? 1 : 0);
            PlayerPrefs.Save();
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        private void LoadSettings()
        {
            // 이미 세팅된 값이 있으면 덮어쓰지 않음
            if (PlayerPrefs.HasKey("ResolutionIndex"))
                ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            // 없으면 현재 값 유지 (기본값)
            if (PlayerPrefs.HasKey("Brightness"))
                Brightness = PlayerPrefs.GetFloat("Brightness");
            if (PlayerPrefs.HasKey("ShowDamage"))
                ShowDamage = PlayerPrefs.GetInt("ShowDamage") == 1;
            else
                ShowDamage = true;
            if (PlayerPrefs.HasKey("FullScreen"))
                FullScreen = PlayerPrefs.GetInt("FullScreen") == 1;

            ApplyFullScreen(); // <<<<< 먼저 호출

            if (_resolutions != null && _resolutions.Length > 0)
                ApplyResolution();
            ApplyBrightness();

            if (GameManager.DamageTextPoolManager != null)
                GameManager.DamageTextPoolManager.showDamageText = _showDamage;
        }
    }
}
