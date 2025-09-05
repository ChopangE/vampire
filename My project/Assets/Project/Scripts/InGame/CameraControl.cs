using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    CinemachineVirtualCamera vc;
    CinemachineBasicMultiChannelPerlin noise;
    private float originalCameraSize;
    
    void Awake() {
        vc = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        noise = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        originalCameraSize = vc.m_Lens.OrthographicSize;
    }

    public void ShakeCamera() {
        noise.m_AmplitudeGain = 5f; 
        noise.m_FrequencyGain = 1f;

    }

    public void StopCameraShake() {
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    public void SetBossCameraSize(float size = 11f)
    {
        if(vc == null) vc = GetComponent<CinemachineVirtualCamera>();
        vc.m_Lens.OrthographicSize = size;
    }

   
}
