using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager;
using UI.Page;
using Unity.VisualScripting;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class StageMap : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
    private int lastActiveStageIndex = 0;

    void OnEnable()
    {
        Global.StageManager.OnStageChanged += OnStageChanged;
        ShowStageByCurrentStage();
    }
    void OnDisable()
    {
        Global.StageManager.OnStageChanged -= OnStageChanged;
    }

    public void HideAllStages() {
        // 현재 활성화된 스테이지 찾기
        for(int i = 0; i < stages.Length; i++) {
            if(stages[i].activeSelf) {
                lastActiveStageIndex = i;
                break;
            }
        }
        // 모든 스테이지 비활성화
        foreach(var stage in stages) {
            stage.SetActive(false);
        }
    }

    public void ShowStage() {
        for(int i = 0; i < stages.Length; i++) {
            if(i == lastActiveStageIndex) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
    }

    public void ShowStage(int index) {
        lastActiveStageIndex = index;
        for(int i = 0; i < stages.Length; i++) {
            if(i == index) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 현재 스테이지에 따라 적절한 스테이지 책을 표시합니다.
    /// 데모 버전: 스테이지 1(0-3)만 가능, 이후 스테이지는 데모 끝 페이지 표시
    /// </summary>
    public void ShowStageByCurrentStage()
    {
        int curStage = Global.UserDataManager.storage.curStage;
        
        // 데모 제한: 스테이지 1(0-3)을 넘어가면 데모 끝 페이지 열기
        if (curStage > 3)
        {
            Global.UIManager.OpenPage<DemoEndPage>();
            return;
        }
        
        if (curStage >= 0 && curStage <= 3)
        {
            Stage1();
        }
        else
        {
            // 기본값으로 Stage1 표시
            Stage1();
        }
    }

    [Binding]
    public void Stage1() {
        for(int i = 0; i < stages.Length; i++) {
            if(i == 0) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
        Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
    }
    [Binding]
    public void Stage2() {
        // 데모 제한: Stage2 접근 시 데모 끝 페이지 열기
        Global.UIManager.OpenPage<DemoEndPage>();
    }
    [Binding]
    public void Stage3() {
        // 데모 제한: Stage3 접근 시 데모 끝 페이지 열기
        Global.UIManager.OpenPage<DemoEndPage>();
    }
    [Binding]
    public void BossStage() {
        // 데모 제한: BossStage 접근 시 데모 끝 페이지 열기
        Global.UIManager.OpenPage<DemoEndPage>();
    }
    private void OnStageChanged()
    {
        int currentStageLevel = Global.StageManager.stageLevel;
        
        // 데모 제한: stageLevel 1을 넘어가면 데모 끝 페이지 열기
        if (currentStageLevel > 0)
        {
            Global.UIManager.OpenPage<DemoEndPage>();
            return;
        }
        
        for(int i = 0; i < stages.Length; i++) {
            if(i == currentStageLevel) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
    }
}
