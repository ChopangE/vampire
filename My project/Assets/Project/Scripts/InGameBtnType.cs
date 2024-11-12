using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameBtnType : MonoBehaviour {
    public enum IngameButtonType { Continue, Option, Exit, Back   };
    
    public IngameButtonType btnType;
    public CanvasGroup main;
    public CanvasGroup option;
    public PausePage pausePage;
    Vector3 defaultVec;
    void Awake() {
        defaultVec = transform.localScale;
    }

    public void OnBtnClick() {
        switch (btnType) {
            case IngameButtonType.Continue:
                pausePage.Hide();
                break;
            case IngameButtonType.Option:
                CanvasOn(option);
                CanvasOff(main);
                break;
            case IngameButtonType.Exit:
                Debug.Log("Exit");
                break;
            case IngameButtonType.Back:
                CanvasOn(main);
                CanvasOff(option);
                break;
        }

    }
    public void CanvasOn(CanvasGroup group) {
        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void CanvasOff(CanvasGroup group) {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
   
}
