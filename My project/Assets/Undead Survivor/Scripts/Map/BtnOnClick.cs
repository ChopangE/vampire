using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnOnClick : MonoBehaviour
{
    Button btn;
    void Awake() {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(StageManager.Instance.CallScene);
    }

    
}
