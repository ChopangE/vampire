using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Manager
{
    [System.Serializable]
    public class BGMSound{
        public BGMEnum name;
        public AudioClip clip;
    }
    [System.Serializable]
    public class SFXSound{
        public SFXEnum name;
        public AudioClip clip;
    }
    public class SoundManager : MonoBehaviour
    {
        public BGMSound[] bgmArr;
        public SFXSound[] sfxArr;
        public AudioSource bgmSource, sfxSource, hitSource;

        private float _bgmVolume = 1f;
        private float _sfxVolume = 1f;
        private float _hitVolume = 1f;

        private void Awake()
        {
            // 전역 참조 설정
            Global.SoundManager = this;
            
            // 히트 사운드 소스가 없으면 생성
            if (hitSource == null)
            {
                hitSource = gameObject.AddComponent<AudioSource>();
                hitSource.playOnAwake = false;
            }
            
            // 저장된 볼륨 설정 불러오기
            LoadVolumeSettings();
        }

        private void LoadVolumeSettings()
        {
            _bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 100f) / 100f;
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100f) / 100f;
            _hitVolume = PlayerPrefs.GetFloat("HitVolume", 100f) / 100f;
            
            bool bgmMute = PlayerPrefs.GetInt("BGMMute", 0) == 1;
            bool sfxMute = PlayerPrefs.GetInt("SFXMute", 0) == 1;
            bool hitMute = PlayerPrefs.GetInt("HitMute", 0) == 1;
            
            // 볼륨 및 음소거 설정 적용
            SetBGMVolume(_bgmVolume, bgmMute);
            SetSFXVolume(_sfxVolume, sfxMute);
            SetHitVolume(_hitVolume, hitMute);
        }

        public void SetBGMVolume(float volume, bool mute = false)
        {
            _bgmVolume = volume;
            bgmSource.volume = volume;
            bgmSource.mute = mute;
        }

        public void SetSFXVolume(float volume, bool mute = false)
        {
            _sfxVolume = volume;
            sfxSource.volume = volume;
            sfxSource.mute = mute;
        }

        public void SetHitVolume(float volume, bool mute = false)
        {
            _hitVolume = volume;
            if (hitSource != null)
            {
                hitSource.volume = volume;
                hitSource.mute = mute;
            }
        }

        public void PlayMusic(BGMEnum name, float volume = -1, bool isLoop = true)
        {
            BGMSound s = FindBGM(name, bgmArr);
            if(s == null || s.name == BGMEnum.NONE) return;
            
            // volume이 -1이면 저장된 볼륨 사용, 아니면 지정된 볼륨 사용
            float playVolume = volume < 0 ? _bgmVolume : volume;
            
            bgmSource.loop = isLoop;
            bgmSource.clip = s.clip;
            bgmSource.volume = playVolume;
            bgmSource.Play();
        }

        public void PlaySFX(SFXEnum name, float volume = -1)
        {
            SFXSound s = FindSFX(name, sfxArr);
            if(s == null || s.name == SFXEnum.NONE) return;
            
            // volume이 -1이면 저장된 볼륨 사용, 아니면 지정된 볼륨 사용
            float playVolume = volume < 0 ? _sfxVolume : volume;
            
            sfxSource.PlayOneShot(s.clip, playVolume);
        }

        public void PlayHitSound(SFXEnum name, float volume = -1)
        {
            if (hitSource == null) return;
            if(name != SFXEnum.Hit) return;
            
            SFXSound s = FindSFX(name, sfxArr);
            if(s == null || s.name == SFXEnum.NONE) return;
            
            // volume이 -1이면 저장된 볼륨 사용, 아니면 지정된 볼륨 사용
            float playVolume = volume < 0 ? _hitVolume : volume;
            
            hitSource.PlayOneShot(s.clip, playVolume);
        }

        public BGMSound FindBGM(BGMEnum name, BGMSound[] soundsArr)
        {
            BGMSound s = Array.Find(bgmArr, x => x.name == name);
            if(s == null)
                Debug.Log("해당 사운드 찾기 실패");
            return s;
        }
        
        public SFXSound FindSFX(SFXEnum name, SFXSound[] soundsArr)
        {
            SFXSound s = Array.Find(sfxArr, x => x.name == name);

            if(s == null)
                Debug.Log("해당 사운드 찾기 실패");
            return s;
        }
        
        public void StopSFX(bool isStop = true)
        {
            if(isStop)
                sfxSource.Stop();
            else
                sfxSource.Play();
        }
        
        public void StopBGM(bool isStop = true)
        {
            if(isStop)
                bgmSource.Stop();
            else
                bgmSource.Play();
        }
        
        public void StopHit(bool isStop = true)
        {
            if (hitSource == null) return;
            
            if(isStop)
                hitSource.Stop();
            else
                hitSource.Play();
        }
        
        public bool MuteSFX(bool isMute) => sfxSource.mute = isMute; 
        public bool MuteBGM(bool isMute) => bgmSource.mute = isMute;
        public bool MuteHit(bool isMute) => hitSource != null ? hitSource.mute = isMute : false;
    }
}