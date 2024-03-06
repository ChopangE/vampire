using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterStage : MonoBehaviour
{
    public enum StageType {
        Stage,Random,Boss,Rest,Shop
    }
    public Transform player;
    public Transform point;
    public StageType type;
    public void OnGo() {
        //Vector3 dest = Camera.main.ScreenToWorldPoint(transform.position);
        Vector3 dest = point.position;
        StartCoroutine(GoToBtn(dest, player));
    }

    IEnumerator GoToBtn(Vector3 dest, Transform player) {

        while (player.position != dest) {
            player.position = Vector3.MoveTowards(player.position, dest, 0.1f);
            yield return null;

        }
        if (type == StageType.Stage) {
            StageManager.Instance.curPoint = 0;
            SceneManager.LoadScene(2);
        }
        gameObject.SetActive(false);


    }

}
