using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smash : MonoBehaviour
{
    Collider2D coll;
    public GameObject child;
    public Transform target;
    public GameObject[] dusts;
    public Animator anim;
    bool isAuto = true;
    float timer = 0;
    void Awake() {
        coll = GetComponent<Collider2D>();
        target = GameManager.Instance.player.GetComponent<Transform>();
    }
    void Start() {
        /*
        if (transform.position.x > target.position.x) {
            transform.eulerAngles = new Vector3(0, 0 ,-45);
        }
        else {
            transform.eulerAngles = new Vector3(0, 0 , 45);

        }
        */
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
        if (isAuto) {
            Vector3 dir = target.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
            transform.eulerAngles += new Vector3(0, 0, 50f);
        }
    }
    public void OnSmash() {
        SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
        child.GetComponent<Collider2D>().enabled = true;

    }

    public void OnFinish() {
        child.SetActive(false);
        coll.enabled = true;
        OnDusts();
    }

    public void StartGoGO() {
        coll.enabled = false;
        OnAutoTarget();
        OffDusts();

    }

    public void ViewSmashLine() {
        child.SetActive(true);
        child.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 0, 0, 0.3f);
    }
    public void LastFinish() {
        
        anim.SetBool("Smash", false);
        gameObject.SetActive(false);
    }
    void OnAutoTarget() {
        isAuto = true;
    }
    void OnDusts() {
        for(int i = 0; i < dusts.Length; i++) {
            dusts[i].SetActive(true);
        }
    }
    void OffDusts() {
        for (int i = 0; i < dusts.Length; i++) {
            dusts[i].SetActive(false);
        }
    }
    void OnEnable() {
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
        transform.eulerAngles += new Vector3(0, 0, 50f);
    }

    public void OffAutoTarget() {
        isAuto = false;
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector3 power = collision.transform.position - transform.position;
        power = power.normalized;
        Rigidbody2D rigid = collision.gameObject.GetComponent<Rigidbody2D>();
        rigid.AddForce(power * 10, ForceMode2D.Impulse);

        GameManager.Instance.health -= 1;

    }

    
    
    /*
    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag != "Player") return;
        Vector2 force = new Vector2(-1,-1);
        //GameManager.Instance.player.rigid.AddForce(force * 15, ForceMode2D.Impulse);
    }
    */
}
