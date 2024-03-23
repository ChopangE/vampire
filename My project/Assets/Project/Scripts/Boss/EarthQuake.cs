using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthQuake : MonoBehaviour
{
    CameraControl CC;

    void Awake() {
        CC = FindObjectOfType<CameraControl>();
       

    }
    
    public void StopCameraShaking() {
        CC.StopCameraShake();
    }
    public void StartCameraShaking() {
        CC.ShakeCamera();
    }
}
