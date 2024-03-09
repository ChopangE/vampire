using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burned : MonoBehaviour
{

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag != "Player") return;
        GameManager.Instance.health -= Time.deltaTime * 10f;
        

    }
}
