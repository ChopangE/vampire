using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burned : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        GameManager.Instance.health -= Time.deltaTime * 10f;
    }
}
