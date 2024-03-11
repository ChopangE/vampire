using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : MonoBehaviour
{
    Collider2D coll;
    public GameObject child;
    public Transform target;
    float timer = 0;
    void Awake() {
        coll = GetComponent<Collider2D>();
        target = GameManager.Instance.player.GetComponent<Transform>();
        //child = GetComponentInChildren<GameObject>();
    }
    void Start() {
        if (transform.position.x > target.position.x) {
            transform.eulerAngles = new Vector3(0, 0 ,-45);
        }
        else {
            transform.eulerAngles = new Vector3(0, 0 , 45);

        }
        coll.enabled = false;
        child.SetActive(false);
    }

    void Update() {
        timer += Time.deltaTime;
        /*
        if (transform.position.x > target.position.x && target.position.y > 98.5f) {
            transform.eulerAngles = new Vector3(0, 0, -45);
        }
        else if(transform.position.x > target.position.x && target.position.y < 98.5f) {
            transform.eulerAngles = new Vector3(0, 0, 0);

        }
        else if (transform.position.x < target.position.x && target.position.y < 98.5f) {
            transform.eulerAngles = new Vector3(0, 0, 30);

        }
        else {
            transform.eulerAngles = new Vector3(0, 0, 60);
        }
        */
        if(timer > 3f) {
            timer = 0f;
        }
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
        transform.eulerAngles += new Vector3(0, 0, 30f);
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

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector3 power = collision.transform.position - transform.position;
        power = power.normalized;
        //new Vector3(-10, -10, 0);
        Rigidbody2D rigid = collision.gameObject.GetComponent<Rigidbody2D>();
        rigid.AddForce(power * 10, ForceMode2D.Impulse);

        GameManager.Instance.health -= 10;
        Debug.Log(collision.gameObject.tag);
       // rigid.velocity = Vector2.zero;

    }

    
    /*
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector2 force = new Vector2(-1,-1);
        //GameManager.Instance.player.rigid.AddForce(force * 15, ForceMode2D.Impulse);
    }
    */
}
