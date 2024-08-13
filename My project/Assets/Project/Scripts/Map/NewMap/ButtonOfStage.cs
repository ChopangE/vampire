using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOfStage : MonoBehaviour
{

    public int buttonNum;
    public Sprite pushedImage;
    public Sprite image;

    bool isActive;
    Image title;
    Button button;
    void Start()
    {
        Init();
    }
    void Init() {
        title = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(StageManager.Instance.CallScene);
        isActive = (buttonNum == StageManager.Instance.stageCount);
        button.interactable = isActive;
        if (isActive) {
            title.sprite = image;
        }
        else {
            title.sprite = pushedImage;
        }
        if(buttonNum % 4 == 3 && !isActive) {
            gameObject.SetActive(false);
        }
    }
    
}
