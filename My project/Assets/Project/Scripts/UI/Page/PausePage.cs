using System.Collections;
using System.Collections.Generic;
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
        AudioManager.instance.EffectBgm(true);
    }
    public void Hide() {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        AudioManager.instance.EffectBgm(false);

    }
}
