using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class ListType : MonoBehaviour
{
    public StageType StageType;

    void Start() {
        if( (int)StageType == Global.StageManager.curPoint) {
            gameObject.SetActive(true);
        }
    }
}
