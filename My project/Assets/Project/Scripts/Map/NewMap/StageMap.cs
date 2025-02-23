using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class StageMap : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
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
}
