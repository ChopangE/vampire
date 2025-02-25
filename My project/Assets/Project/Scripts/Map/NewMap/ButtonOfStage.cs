using System.Collections;
using System.Collections.Generic;
using Manager;
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
        
        button.onClick.AddListener(() => {
            if(buttonNum == 0) {
                // Global.DataManager.ResetData();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
            }
            else {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
            }
        });

        // 현재 스테이지와 버튼 번호 비교
        int currentStage = Global.UserDataManager.curStage;
        isActive = buttonNum == currentStage;
        
        button.interactable = isActive;
        if (isActive) {
            title.sprite = image;
        }
        else {
            title.sprite = pushedImage;
        }

        // 각 레벨의 마지막 스테이지(3,7,11...)이고 아직 활성화되지 않은 경우 숨김
        if(buttonNum % Global.StageManager.MAX_STAGE_COUNT == 3 && !isActive) {
            gameObject.SetActive(false);
        }
    }
    
}
