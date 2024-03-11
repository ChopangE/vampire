using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashRange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector2 power2 = new Vector2(-10, -10);
        Rigidbody2D rigid = collision.gameObject.GetComponent<Rigidbody2D>();
        rigid.AddForce(power2 * 3, ForceMode2D.Impulse);
        GameManager.Instance.health -= 10;
        Debug.Log(collision.gameObject.tag);
    }

}
