using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMap : MonoBehaviour
{
    public GameObject[] stages;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init() {
        for(int i = 0; i < stages.Length; i++) {
            if(i == Manager.Global.StageManager.stageLevel) {
                stages[i].SetActive(true);
            }
            else {
                stages[i].SetActive(false);
            }
        }

    }
    
}
