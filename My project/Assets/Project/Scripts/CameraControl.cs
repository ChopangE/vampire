using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    CinemachineVirtualCamera vc;
    CinemachineBasicMultiChannelPerlin noise;
    void Awake() {
        vc = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        noise = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera() {
        noise.m_AmplitudeGain = 5f; 
        noise.m_FrequencyGain = 1f;

    }

    public void StopCameraShake() {
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
   
}
