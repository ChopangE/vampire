using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : MonoBehaviour
{
    Collider2D coll;
    public GameObject child;
    void Awake() {
        coll = GetComponent<Collider2D>();
        //child = GetComponentInChildren<GameObject>();
    }
    void Start() {
        coll.enabled = false;
        child.SetActive(false);
    }
    public void OnSmash() {
        child.SetActive(true);
    }

    public void OnFinish() {
        child.SetActive(false);
        coll.enabled = true;
    }

    public void StartGoGO() {
        coll.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag != "Player") return;

        Vector2 force = collision.contacts[0].normal;
        GameManager.Instance.player.rigid.AddForce(force * 20, ForceMode2D.Impulse);
    }
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector2 force = new Vector2(-1,-1);
        //GameManager.Instance.player.rigid.AddForce(force * 15, ForceMode2D.Impulse);
    }
}
