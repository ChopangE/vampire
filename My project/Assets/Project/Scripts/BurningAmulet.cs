using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningAmulet : MonoBehaviour {


    public GameObject fireBall;

    int prefabId;

    void Start() {
        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++) {
            if (fireBall == GameManager.Instance.pool.prefabs[i]) {
                prefabId = i;
                break;
            }
        }
    }
    public void Burning() {
        Vector3 dir = GameManager.Instance.player.inputVec;
        if (dir.magnitude < 0.1f) {
            dir = GameManager.Instance.player.GetComponent<SpriteRenderer>().flipX ? new Vector2(-1f, 0) : new Vector2(1f, 0);
        }
        Vector3 dir2 = dir;
        if (GameManager.Instance.player.scan.nearestTarget) {
            Vector3 targetPos = GameManager.Instance.player.scan.nearestTarget.position;
            dir2 = (targetPos - transform.position).normalized;
        }
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir2);
        bullet.GetComponent<Bullet>().Init(5, 0, dir2);
        gameObject.SetActive(false);
        //transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.right, dir2);
        //transform.GetChild(0).GetComponent<Bullet>().Init(5, 1, dir2);
        //StartCoroutine(BreathDown(bullet, dir));
    }
}
