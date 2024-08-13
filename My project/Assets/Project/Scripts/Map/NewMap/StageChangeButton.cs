using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeButton : MonoBehaviour
{
    public GameObject curStage;
    public GameObject goalStage;
    
    public void StageChange() {
        curStage.SetActive(false);
        goalStage.SetActive(true);
    }
}
