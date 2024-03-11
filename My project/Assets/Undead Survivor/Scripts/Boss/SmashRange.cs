using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashRange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector3 power = collision.transform.position - transform.position;
        power = power.normalized;
            //new Vector3(-10, -10, 0);
        Rigidbody2D rigid = collision.gameObject.GetComponent<Rigidbody2D>();
        rigid.AddForce(power * 50, ForceMode2D.Impulse);

        GameManager.Instance.health -= 1;
        Debug.Log(collision.gameObject.tag);
    }

    

}
