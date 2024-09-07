using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public int price;
    Button btn;
    RectTransform rect;
    Vector3 defaultVec;
    void Awake() {
        btn = GetComponent<Button>();
        rect = GetComponent<RectTransform>();
    }
    void Start() {
        defaultVec = rect.localScale;
    }
    public void OnClicked() {
        btn.interactable = false;
    }
    public void OnPointerEnter(PointerEventData eventData) {
        rect.localScale = defaultVec * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData) {
        rect.localScale = defaultVec;
    }
}
