using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class StageMap : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
    // Start is called before the first frame update
    void Start()
    {
        Init().Forget();
    }
    private async UniTaskVoid Init() {
        await UniTask.WaitUntil(() => Manager.Global.Instance != null);

        if(stages.Length == 0) return;
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
