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
    public class SoundOptionViewModel : ViewModel
    {
        private float _bgmVolume;
        private float _sfxVolume;
        private float _hitVolume;
        private bool _bgmMute;
        private bool _sfxMute;
        private bool _hitMute;
        
        // 볼륨 텍스트 표시를 위한 속성
        private string _bgmVolumeText;
        private string _sfxVolumeText;
        private string _hitVolumeText;

        [Binding]
        public float BgmVolume
        {
            get => _bgmVolume;
            set
            {
                float newValue = Mathf.Round(value);
                if (!Mathf.Approximately(_bgmVolume, newValue))
                {
                    _bgmVolume = newValue;
                    BgmVolumeText = $"BGM : {(int)_bgmVolume}";
                    OnPropertyChanged(nameof(BgmVolume));
                    ApplyBgmVolume();
                    SaveSettings();
                }
            }
        }

        [Binding]
        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                float newValue = Mathf.Round(value);
                if (!Mathf.Approximately(_sfxVolume, newValue))
                {
                    _sfxVolume = newValue;
                    SfxVolumeText = $"SFX : {(int)_sfxVolume}";
                    OnPropertyChanged(nameof(SfxVolume));
                    ApplySfxVolume();
                    SaveSettings();
                }
            }
        }

        [Binding]
        public float HitVolume
        {
            get => _hitVolume;
            set
            {
                float newValue = Mathf.Round(value);
                if (!Mathf.Approximately(_hitVolume, newValue))
                {
                    _hitVolume = newValue;
                    HitVolumeText = $"Hit : {(int)_hitVolume}";
                    OnPropertyChanged(nameof(HitVolume));
                    ApplyHitVolume();
                    SaveSettings();
                }
            }
        }

        [Binding]
        public string BgmVolumeText
        {
            get => _bgmVolumeText;
            set
            {
                _bgmVolumeText = value;
                OnPropertyChanged(nameof(BgmVolumeText));
            }
        }

        [Binding]
        public string SfxVolumeText
        {
            get => _sfxVolumeText;
            set
            {
                _sfxVolumeText = value;
                OnPropertyChanged(nameof(SfxVolumeText));
            }
        }

        [Binding]
        public string HitVolumeText
        {
            get => _hitVolumeText;
            set
            {
                _hitVolumeText = value;
                OnPropertyChanged(nameof(HitVolumeText));
            }
        }

        [Binding]
        public bool BgmMute
        {
            get => _bgmMute;
            set
            {
                _bgmMute = value;
                OnPropertyChanged(nameof(BgmMute));
                ApplyBgmVolume();
                SaveSettings();
            }
        }

        [Binding]
        public bool SfxMute
        {
            get => _sfxMute;
            set
            {
                _sfxMute = value;
                OnPropertyChanged(nameof(SfxMute));
                ApplySfxVolume();
                SaveSettings();
            }
        }

        [Binding]
        public bool HitMute
        {
            get => _hitMute;
            set
            {
                _hitMute = value;
                OnPropertyChanged(nameof(HitMute));
                ApplyHitVolume();
                SaveSettings();
            }
        }

        private void Start()
        {
            LoadSettings();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void ApplyBgmVolume()
        {
            if (Global.SoundManager != null)
            {
                Global.SoundManager.SetBGMVolume(_bgmVolume / 100f, _bgmMute);
            }
        }

        private void ApplySfxVolume()
        {
            if (Global.SoundManager != null)
            {
                Global.SoundManager.SetSFXVolume(_sfxVolume / 100f, _sfxMute);
            }
        }

        private void ApplyHitVolume()
        {
            if (Global.SoundManager != null)
            {
                Global.SoundManager.SetHitVolume(_hitVolume / 100f, _hitMute);
            }
        }

        [Binding]
        public void OnClickBgmMute()
        {
            BgmMute = !BgmMute;
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            // 테스트용 사운드 재생 (선택 사항)
            if (Global.SoundManager != null && !BgmMute)
            {
                // BGM 음소거 해제 시 현재 BGM 다시 재생 (필요한 경우)
                // Global.SoundManager.PlayMusic(BGMEnum.MENU);
            }
        }

        [Binding]
        public void OnClickSfxMute()
        {
            SfxMute = !SfxMute;
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            // 테스트용 사운드 재생 (선택 사항)
            if (Global.SoundManager != null && !SfxMute)
            {
                // SFX 음소거 해제 시 테스트 사운드 재생 (필요한 경우)
                // Global.SoundManager.PlaySFX(SFXEnum.BUTTON);
            }
        }

        [Binding]
        public void OnClickHitMute()
        {
            HitMute = !HitMute;
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            // 테스트용 사운드 재생 (선택 사항)
            if (Global.SoundManager != null && !HitMute)
            {
                // Hit 음소거 해제 시 테스트 사운드 재생 (필요한 경우)
                Global.SoundManager.PlayHitSFX(SFXEnum.HGD_Hit);
            }
        }

        // 테스트용 사운드 재생 버튼 (선택 사항)
        [Binding]
        public void PlayTestBgm()
        {
            if (Global.SoundManager != null)
            {
                // 테스트 BGM 재생
            }
        }


        [Binding]
        public void SaveSettings()
        {
            Debug.Log($"SaveSettings called - BGM: {_bgmVolume}, SFX: {_sfxVolume}, Hit: {_hitVolume}");
            PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
            PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            PlayerPrefs.SetFloat("HitVolume", _hitVolume);
            PlayerPrefs.SetInt("BGMMute", _bgmMute ? 1 : 0);
            PlayerPrefs.SetInt("SFXMute", _sfxMute ? 1 : 0);
            PlayerPrefs.SetInt("HitMute", _hitMute ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log("Settings saved to PlayerPrefs");
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        private void LoadSettings()
        {
            Debug.Log("LoadSettings called");
            // 볼륨 값 불러오기 (0-100 범위) - 반드시 필드에만 할당!
            _bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 100f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100f);
            _hitVolume = PlayerPrefs.GetFloat("HitVolume", 100f);

            Debug.Log($"Loaded values - BGM: {_bgmVolume}, SFX: {_sfxVolume}, Hit: {_hitVolume}");

            // 음소거 상태 불러오기 - 반드시 필드에만 할당!
            _bgmMute = PlayerPrefs.GetInt("BGMMute", 0) == 1;
            _sfxMute = PlayerPrefs.GetInt("SFXMute", 0) == 1;
            _hitMute = PlayerPrefs.GetInt("HitMute", 0) == 1;

            // 볼륨 텍스트 업데이트
            BgmVolumeText = $"BGM : {(int)_bgmVolume}";
            SfxVolumeText = $"SFX : {(int)_sfxVolume}";
            HitVolumeText = $"Hit : {(int)_hitVolume}";

            // 속성 변경 알림 (UI 갱신)
            OnPropertyChanged(nameof(BgmVolume));
            OnPropertyChanged(nameof(SfxVolume));
            OnPropertyChanged(nameof(HitVolume));
            OnPropertyChanged(nameof(BgmMute));
            OnPropertyChanged(nameof(SfxMute));
            OnPropertyChanged(nameof(HitMute));

            // 설정 적용
            ApplyBgmVolume();
            ApplySfxVolume();
            ApplyHitVolume();
        }

        // 수동으로 설정을 다시 로드하는 메서드 (UI에서 호출 가능)
        [Binding]
        public void RefreshSettings()
        {
            LoadSettings();
        }

        // 수동으로 설정을 저장하는 메서드 (UI에서 호출 가능)
        [Binding]
        public void ManualSaveSettings()
        {
            Debug.Log("ManualSaveSettings called");
            SaveSettings();
        }
    }
}
