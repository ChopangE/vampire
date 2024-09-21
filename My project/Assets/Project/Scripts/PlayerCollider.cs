using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    void OnCollisionStay2D(Collision2D collision) {
        if (!GameManager.Instance.isLive) return;
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Bound") return;
        GameManager.Instance.health -= Time.deltaTime * 10;
        if (GameManager.Instance.health < 0) {
            for (int i = 2; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            GetComponent<Animator>().SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }
}
