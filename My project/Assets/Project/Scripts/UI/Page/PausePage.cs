using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityWeld;
//using static UnityEditor.Progress;

public class PausePage : ViewModel
{
    RectTransform rect;
    protected override void Awake() {
        base.Awake();
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
