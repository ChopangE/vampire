using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        stages[lastActiveStageIndex].SetActive(true);
    }

    public void ShowStage(int index) {
        lastActiveStageIndex = index;
        stages[index].SetActive(true);
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
