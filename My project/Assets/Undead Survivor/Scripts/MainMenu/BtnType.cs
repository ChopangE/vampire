using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ButtonType btnType;
    public CanvasGroup main;
    public CanvasGroup option;
    public CanvasGroup characterSel;
    Vector3 defaultVec;
    void Awake() {
        defaultVec = transform.localScale;
    }
    public void OnBtnClick() {
        switch (btnType) {
            case ButtonType.Start:
                Debug.Log("Start");
                CanvasOn(characterSel);
                CanvasOff(main);
                break;
            case ButtonType.Option:
                Debug.Log("Option");
                CanvasOn(option);
                CanvasOff(main);
                break;
            case ButtonType.Quit:
                Debug.Log("Quit");
                break;
            case ButtonType.Back:
                Debug.Log("Back");
                CanvasOn(main);
                CanvasOff(option);
                break;
            case ButtonType.Sound:
                Debug.Log("Sound");
                break;
            case ButtonType.Continue:
                Debug.Log("Continue");
                break;
            default:
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

    public void OnPointerEnter(PointerEventData eventData) {
        transform.localScale = defaultVec * 1.2f; 
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.localScale = defaultVec;
    }
}
