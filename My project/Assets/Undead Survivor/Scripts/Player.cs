using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    public Scanner scan;
    public float speed;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scan = GetComponent<Scanner>();
    }

    // Update is called once per frame
    

    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }

    void OnMove(InputValue value) {
    
        inputVec = value.Get<Vector2>();            //�̹� normalized ����
    
    }

    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;

        anim.SetFloat("Speed", inputVec.magnitude);
        
        if(inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ? true : false; 
        }
        
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (!GameManager.Instance.isLive) return;

        GameManager.Instance.health -= Time.deltaTime * 10;
        if (GameManager.Instance.health < 0) {
            for (int i = 2; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.Instance.GameOver();
        }
    }
}
