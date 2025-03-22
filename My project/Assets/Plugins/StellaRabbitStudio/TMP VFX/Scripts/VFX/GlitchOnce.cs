using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace StellaRabbitStudio
{
    public class GlitchOnce : MonoBehaviour
    {
        [Header("Material Option")]
        [SerializeField] private bool useInstanceMaterial = false;
        [Header("Play Option")]
        [SerializeField]
        private bool playOnEnable = true;

        [Description("Set -1 for infinite replay")]
        [SerializeField]
        private int rePlay = -1;

        [SerializeField] private float rePlayCoolTime = 1;

        private int remainingPlays;
        private float cooldownTimer;

        [Header("Glitch Effect Properties")]
        [SerializeField] private bool verticalGlitchOnOff = false;
        [SerializeField] private float glitchIntensity = 0.5f;
        [SerializeField] private float glitchFrequency = 1f;
        [SerializeField][Range(0, 1)] private float lagAmount = 0.1f;
        [SerializeField] private float glitchExtent = 0.1f;
        [SerializeField] private float glitchSpeed = 1f;

        [Space(10)][Range(0, 10)] public float playTime = 0.5f;

        private Material instanceTextMat;
        private TextMeshProUGUI selfText;

        private Coroutine curGlitch;
        private bool isGlitching = false;

        [Space(10)]
        public UnityEvent<float, float> OnGlitchStarted;
        public UnityEvent OnGlitchCompleted;
        public UnityEvent OnGlitchStopped;

        void Awake()
        {
            selfText = GetComponent<TextMeshProUGUI>();
            if (selfText == null)
            {
                Debug.LogError($"TextMeshProUGUI component not found on {gameObject.name}");
                enabled = false;
                return;
            }
            InitializeMaterial();
        }

        private void InitializeMaterial()
        {
            instanceTextMat = useInstanceMaterial 
                ? new Material(selfText.fontSharedMaterial) 
                : selfText.fontSharedMaterial;
            
            if (useInstanceMaterial)
            {
                selfText.fontMaterial = instanceTextMat;
            }
        }

        private void OnEnable()
        {
            remainingPlays = rePlay;
            cooldownTimer = 0;

            if (playOnEnable)
                Glitch();
        }

        private void Update()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                return;
            }
            
            if (isGlitching)
            {
                ApplyGlitchProperties();
            }
        }

        private void OnDisable()
        {
            StopGlitch();
        }

        public void Glitch()
        {
            if (isGlitching)
                return;

            isGlitching = true;
            ApplyGlitchProperties();
            curGlitch = StartCoroutine(GlitchCoroutine());
        }

        public void StopGlitch()
        {
            if (curGlitch != null)
            {
                StopCoroutine(curGlitch);
                curGlitch = null;
                OnGlitchStopped?.Invoke();
            }
            isGlitching = false;
            ResetGlitchProperties();
        }

        private IEnumerator GlitchCoroutine()
        {
            float elapsedTime = 0;
            OnGlitchStarted?.Invoke(glitchIntensity, glitchFrequency);

            while (elapsedTime < playTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isGlitching = false;
            ResetGlitchProperties();
            OnGlitchCompleted?.Invoke();

            cooldownTimer = rePlayCoolTime;
            curGlitch = null;

            if (rePlay != 0)
            {
                remainingPlays--;
                if (remainingPlays != 0)
                {
                    yield return new WaitForSeconds(rePlayCoolTime);
                    Glitch();
                }
            }
        }

        private void ApplyGlitchProperties()
        {
            if (instanceTextMat == null) return;
            instanceTextMat.SetFloat("_VerticalGlitchOnOff", verticalGlitchOnOff ? 1 : 0);
            instanceTextMat.SetFloat("_GlitchIntensity", glitchIntensity);
            instanceTextMat.SetFloat("_GlitchFrequency", glitchFrequency);
            instanceTextMat.SetFloat("_LagAmount", lagAmount);
            instanceTextMat.SetFloat("_GlitchExtent", glitchExtent);
            instanceTextMat.SetFloat("_GlitchSpeed", glitchSpeed);
        }

        private void ResetGlitchProperties()
        {
            if (instanceTextMat == null) return;
            instanceTextMat.SetFloat("_VerticalGlitchOnOff", 0);
            instanceTextMat.SetFloat("_GlitchIntensity", 0);
            instanceTextMat.SetFloat("_GlitchFrequency", 0);
            instanceTextMat.SetFloat("_LagAmount", 0);
            instanceTextMat.SetFloat("_GlitchExtent", 0);
            instanceTextMat.SetFloat("_GlitchSpeed", 1); // 기본 속도로 리셋
        }

        private void OnDestroy()
        {
            if (useInstanceMaterial && instanceTextMat != null)
            {
                Destroy(instanceTextMat);
            }
        }

        // 에디터에서 useUniqueMaterial 값이 변경될 때 호출되는 메서드
        private void OnValidate()
        {
            if (Application.isPlaying && selfText != null)
            {
                InitializeMaterial();
            }
        }
    }
}
