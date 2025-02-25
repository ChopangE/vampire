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
    void OnEnable()
    {
        Global.StageManager.OnStageChanged += OnStageChanged;
        OnStageChanged();
    }
    void OnDisable()
    {
        Global.StageManager.OnStageChanged -= OnStageChanged;
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
