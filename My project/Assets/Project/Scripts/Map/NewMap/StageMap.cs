using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager;
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
    /// 0-3: Stage1, 4-7: Stage2, 8-11: Stage3, 12: BossStage
    /// </summary>
    public void ShowStageByCurrentStage()
    {
        int curStage = Global.UserDataManager.storage.curStage;
        
        if (curStage == 12)
        {
            BossStage();
        }
        else if (curStage >= 0 && curStage <= 3)
        {
            Stage1();
        }
        else if (curStage >= 4 && curStage <= 7)
        {
            Stage2();
        }
        else if (curStage >= 8 && curStage <= 11)
        {
            Stage3();
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
        for(int i = 0; i < stages.Length; i++) {
            if(i == 1) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
        Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
    }
    [Binding]
    public void Stage3() {
        for(int i = 0; i < stages.Length; i++) {
            if(i == 2) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
        Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
    }
    [Binding]
    public void BossStage() {
        for(int i = 0; i < stages.Length; i++) {
            if(i == 3) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
        Global.SoundManager.PlaySFX(SFXEnum.Shop_PageTurn);
    }
    private void OnStageChanged()
    {
        for(int i = 0; i < stages.Length; i++) {
            if(i == Global.StageManager.stageLevel) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }
    }
}
