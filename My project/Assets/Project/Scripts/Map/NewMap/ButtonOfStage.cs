using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UI.Page;
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
    bool isPlayingSound = false; // 사운드 재생 중 여부

    void Start()
    {
        Init();
    }
    void Init()
    {
        title = GetComponent<Image>();
        // 현재 스테이지와 버튼 번호 비교
        int currentStage = Global.UserDataManager.curStage;
        isActive = buttonNum == currentStage;
        
        if (TryGetComponent(out button))
        {

            button.onClick.AddListener(() =>
            {
                if (isPlayingSound) return; // 사운드 재생 중이면 클릭 무시

                SFXEnum sfx = SFXEnum.Shop_StageClose;
                isPlayingSound = true; // 사운드 재생 시작
                if (buttonNum == 12)
                {
                    sfx = SFXEnum.Shop_FinalBossStage;
                    Global.SoundManager.PlaySFX(sfx);
                    Global.SoundManager.PlaySFX(SFXEnum.Shop_BossStageClose);

                }
                else if (buttonNum % Global.StageManager.MAX_STAGE_COUNT == 3)
                {
                    sfx = SFXEnum.Shop_BossStageClose;
                    Global.SoundManager.PlaySFX(sfx);
                }
                else
                {
                    Global.SoundManager.PlaySFX(sfx);
                }

                // 사운드 재생 후 씬 전환 지연
                StartCoroutine(LoadSceneAfterDelay("LoadingScene", Global.SoundManager.GetSFXClipLength(sfx)));
            });
            button.interactable = isActive;
        }

        if (isActive)
        {
            title.sprite = image;
        }
        else
        {
            title.sprite = pushedImage;
        }

        // 각 레벨의 마지막 스테이지(3,7,11...)이고 아직 활성화되지 않은 경우 숨김
        // if(buttonNum % Global.StageManager.MAX_STAGE_COUNT == 3 && !isActive) {
        //     gameObject.SetActive(false);
        // }
    }

    // 새로운 코루틴 메서드 추가
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        Global.UIManager.OpenPage<MapPageBlock>();
        yield return new WaitForSeconds(delay);
        if (buttonNum == 0)
        {
            Global.DataManager.ResetData();
        }
        isPlayingSound = false; // 사운드 재생 완료
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
