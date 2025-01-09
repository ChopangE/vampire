using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
//using static UnityEditor.Progress;

public class Pause : MonoBehaviour
{
    RectTransform rect;
    void Awake() {
        rect = GetComponent<RectTransform>();
    }

    public void Show() {
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();
        Global.SoundManager.StopBGM(true);
    }
    public void Hide() {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        Global.SoundManager.StopBGM(false);

    }
}
