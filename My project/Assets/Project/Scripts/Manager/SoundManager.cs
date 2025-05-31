using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks; // UniTask를 사용하기 위한 네임스페이스 추가

namespace Manager
{
    [System.Serializable]
    public class BGMSound
    {
        public BGMEnum name;
        public AudioClip clip;
    }
    [System.Serializable]
    public class SFXSound
    {
        public SFXEnum name;
        public AudioClip clip;
    }
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private string sfxPath = "Assets/Project/Audio/최종 효과음 모음집/HGD 스킬 효과음";

        public BGMSound[] bgmArr;
        public SFXSound[] sfxArr;
        public AudioSource bgmSource, sfxSource, hitSource;

        private float _bgmVolume = 1f;
        private float _sfxVolume = 1f;
        private float _hitVolume = 1f;

        private float hitSoundCooldown = 0.1f;
        private Dictionary<SFXEnum, float> lastHitSoundTimes = new Dictionary<SFXEnum, float>();

        private Dictionary<SFXEnum, AudioSource> activeSFXSources = new Dictionary<SFXEnum, AudioSource>();

        private Queue<AudioSource> sfxSourcePool = new Queue<AudioSource>();
        private int initialPoolSize = 10; // 초기 풀 크기

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
            InitializeSoundPool();
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
            if (s == null || s.name == BGMEnum.NONE) return;

            // volume이 -1이면 저장된 볼륨 사용, 아니면 지정된 볼륨 사용
            float playVolume = volume < 0 ? _bgmVolume : volume;

            bgmSource.loop = isLoop;
            bgmSource.clip = s.clip;
            bgmSource.volume = playVolume;
            bgmSource.Play();
        }

        public void PlaySFX(SFXEnum name, float volume = -1, float delay = 0)
        {
            SFXSound s = FindSFX(name, sfxArr);
            if (s == null || s.name == SFXEnum.NONE) return;

            float playVolume = volume < 0 ? _sfxVolume : volume;

            if (delay > 0)
            {
                PlaySFXWithDelay(s.clip, playVolume, delay, name).Forget();
            }
            else
            {
                // 이전 소스가 있다면 풀에 반환
                if (activeSFXSources.ContainsKey(name))
                {
                    ReturnToPool(activeSFXSources[name]);
                }

                // 풀에서 AudioSource 가져오기
                AudioSource newSource = GetAudioSourceFromPool();
                newSource.gameObject.name = $"SFX_{name}";
                newSource.clip = s.clip;
                newSource.volume = playVolume;
                newSource.Play();

                activeSFXSources[name] = newSource;
            }
        }

        private async UniTaskVoid PlaySFXWithDelay(AudioClip clip, float volume, float delay, SFXEnum name)
        {
            await UniTask.Delay((int)(delay * 1000));
            
            if (activeSFXSources.ContainsKey(name))
            {
                ReturnToPool(activeSFXSources[name]);
            }

            AudioSource newSource = GetAudioSourceFromPool();
            newSource.gameObject.name = $"SFX_{name}_Delayed";
            newSource.clip = clip;
            newSource.volume = volume;
            newSource.Play();

            activeSFXSources[name] = newSource;
        }

        public void PlayHitSFX(SFXEnum name, float volume = -1, bool isShootCooldown = true)
        {
            if (hitSource == null) return;

            // 해당 사운드의 마지막 재생 시간 확인
            if (!lastHitSoundTimes.ContainsKey(name))
            {
                lastHitSoundTimes[name] = 0f;
            }
            
            // 현재 시간이 마지막 재생 시간 + 쿨다운보다 작으면 재생하지 않음
            if (isShootCooldown && Time.time < lastHitSoundTimes[name] + hitSoundCooldown) return;

            SFXSound s = FindSFX(name, sfxArr);
            if (s == null || s.name == SFXEnum.NONE) return;

            float playVolume = volume < 0 ? _hitVolume : volume;
            hitSource.PlayOneShot(s.clip, playVolume);
            
            // 마지막 재생 시간 업데이트
            lastHitSoundTimes[name] = Time.time;
        }

        public BGMSound FindBGM(BGMEnum name, BGMSound[] soundsArr)
        {
            BGMSound s = Array.Find(bgmArr, x => x.name == name);
            if (s == null)
                Debug.Log("해당 사운드 찾기 실패");
            return s;
        }

        public SFXSound FindSFX(SFXEnum name, SFXSound[] soundsArr)
        {
            SFXSound s = Array.Find(sfxArr, x => x.name == name);

            if (s == null)
                Debug.Log("해당 사운드 찾기 실패");
            return s;
        }

        public float GetSFXClipLength(SFXEnum name)
        {
            SFXSound s = FindSFX(name, sfxArr);
            if (s != null && s.clip != null)
            {
                return s.clip.length;  // 클립 길이 반환
            }
            return 0f;  // 클립이 없으면 0 반환
        }
        public void StopSFX(SFXEnum name = SFXEnum.NONE, bool isStop = true)
        {
            if (name == SFXEnum.NONE)
            {
                // 모든 효과음 중지
                foreach (var source in activeSFXSources.Values)
                {
                    if (source != null)
                    {
                        ReturnToPool(source);
                    }
                }
                activeSFXSources.Clear();
            }
            else if (activeSFXSources.ContainsKey(name))
            {
                // 특정 효과음 중지
                if (activeSFXSources[name] != null)
                {
                    ReturnToPool(activeSFXSources[name]);
                }
                activeSFXSources.Remove(name);
            }
        }

        public void StopBGM(bool isStop = true)
        {
            if (bgmSource == null) return;
            if (isStop)
                bgmSource.Stop();
            else
                bgmSource.Play();
        }

        public void StopHit(bool isStop = true)
        {
            if (hitSource == null) return;
            if (isStop)
                hitSource.Stop();
            else
                hitSource.Play();
        }

        public bool MuteSFX(bool isMute) => sfxSource.mute = isMute;
        public bool MuteBGM(bool isMute) => bgmSource.mute = isMute;
        public bool MuteHit(bool isMute) => hitSource != null ? hitSource.mute = isMute : false;

        [Button("InitializeSFXArray")]
        public void InitializeSFXArray()
        {
            // SFXEnum의 크기만큼 배열 초기화
            sfxArr = new SFXSound[System.Enum.GetValues(typeof(SFXEnum)).Length];

            // NONE 초기화
            sfxArr[0] = new SFXSound { name = SFXEnum.NONE, clip = null };

            // 나머지 인덱스 초기화
            for (int i = 1; i < sfxArr.Length; i++)
            {
                sfxArr[i] = new SFXSound { name = (SFXEnum)i, clip = null };
            }

            Debug.Log($"SFX 배열 초기화 완료: {sfxArr.Length}개");
        }
#if UNITY_EDITOR
        [Button("LoadSFXFile")]
        public void LoadSFXFile()
        {
            if (string.IsNullOrEmpty(sfxPath))
            {
                Debug.LogError("SFX 경로가 설정되지 않았습니다.");
                return;
            }

            // Unity AssetDatabase를 사용하여 파일 찾기
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { sfxPath });

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

                if (clip != null)
                {
                    // 현재 비어있는 clip 자리를 찾아서 할당
                    for (int i = 1; i < sfxArr.Length; i++)
                    {
                        if (sfxArr[i].clip == null)
                        {
                            sfxArr[i].clip = clip;
                            Debug.Log($"Loaded SFX to index {i}: {clip.name}");
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < sfxArr.Length; i++)
            {
                if (sfxArr[i].name == SFXEnum.NONE && i != 0)
                {
                    sfxArr[i].name = (SFXEnum)i;
                }
            }

            Debug.Log($"로드된 오디오 클립 수: {guids.Length}개");
        }
#endif

        private void InitializeSoundPool()
        {
            // 초기 오브젝트 풀 생성
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject soundObj = new GameObject($"SFX_Pool_{i}");
                soundObj.transform.parent = sfxSource.gameObject.transform;
                AudioSource source = soundObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                soundObj.SetActive(false);
                sfxSourcePool.Enqueue(source);
            }
        }

        private AudioSource GetAudioSourceFromPool()
        {
            AudioSource source;
            
            // 풀에 있는 비활성화된 오브젝트 찾기
            while (sfxSourcePool.Count > 0)
            {
                source = sfxSourcePool.Dequeue();
                if (source != null && !source.isPlaying)
                {
                    source.gameObject.SetActive(true);
                    return source;
                }
            }

            // 풀이 비었으면 새로 생성
            GameObject newSoundObj = new GameObject($"SFX_Pool_{sfxSourcePool.Count}");
            newSoundObj.transform.parent = sfxSource.gameObject.transform;
            source = newSoundObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            
            return source;
        }

        private void ReturnToPool(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
                sfxSourcePool.Enqueue(source);
            }
        }
    }
}