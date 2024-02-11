using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;
    void Awake() {
        coll = GetComponent<Collider2D>();
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) return;
        
        Vector3 playerVec = GameManager.Instance.player.transform.position;
        Vector3 myVec = transform.position;

        float dirX = playerVec.x - myVec.x;
        float dirY = playerVec.y - myVec.y;

        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);
        
        dirX = dirX > 0 ? 1 : -1;
        dirY = dirY > 0 ? 1 : -1;

        Vector3 playerDir = GameManager.Instance.player.inputVec;

        switch (transform.tag) {
            case "Ground":
                if(diffX > diffY) {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else {
                    transform.Translate(Vector3.up * dirY * 38);
                }
                break;
            case "Enemy":
                if (coll.enabled) {
                   
                    transform.Translate(playerDir * 20 + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), 0));
                }
                break;
        }
    }

}
