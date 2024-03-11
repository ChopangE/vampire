using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class StageButton : MonoBehaviour
{
    public StageType type;
    public Transform player;
    public GameObject List;
    ListType prevUI;
    public GameObject targetUI;
    //SpriteRenderer sprite;

    //void Start() {
    //    if (StageManager.Instance.isCheck[(int)type]) {
    //        sprite = GetComponent<SpriteRenderer>();
    //        cr.a = 0.5f;
    //        sprite.color = cr;
    //    }
    //}
    void Start() {
    //    prevUI = List.GetComponentInChildren<ListType>();
    }
    void OnMouseDown() {
        if (!StageManager.Instance.isCheck[(int)type])
        StartCoroutine(GoToBtn(transform.position, player));
    }

    IEnumerator GoToBtn(Vector3 dest, Transform player) {

        while (player.position != dest) {
            player.position = Vector3.MoveTowards(player.position, dest, 0.05f);
            yield return null;
        }
        StagePlay();
    }

    void StagePlay() {
       
        targetUI.SetActive(true);
        switch (type) {
            case StageType.Stage:
            case StageType.Boss:
                StageManager.Instance.isCheck[(int)type] = true;
                StageManager.Instance.curPoint = (int)type;
                //여기서 UI호출
                
                break;
            case StageType.Random:
                //
                break;
           
            case StageType.Rest:
                //
                break;
            case StageType.Shop:
                StageManager.Instance.isCheck[(int)type] = true;
                StageManager.Instance.curPoint = (int)type;

                break;
        }
    }
}
