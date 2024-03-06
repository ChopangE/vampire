using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListType : MonoBehaviour
{
    public StageType StageType;

    void Start() {
        if( (int)StageType == StageManager.Instance.curPoint) {
            gameObject.SetActive(true);
        }
    }
}
