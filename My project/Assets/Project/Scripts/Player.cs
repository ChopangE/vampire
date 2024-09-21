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
    bool isKnockBack { get; set; }
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scan = GetComponent<Scanner>();
        isKnockBack = false;
    }

    

    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;
        if (isKnockBack) return;
        transform.Translate(inputVec * speed * Time.fixedDeltaTime);
       
    }

    void OnMove(InputValue value) {
    
        inputVec = value.Get<Vector2>();            //�̹� normalized ����
        if (!isKnockBack) {
            rigid.velocity = Vector2.zero;
        }

    }

    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;

        anim.SetFloat("Speed", inputVec.magnitude);
        
        if(inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ? true : false; 
        }
        
    }
    
    
    
    public void Stopping()
    {
        isKnockBack = true;
        StartCoroutine(StoppingCor());
    }

    IEnumerator StoppingCor()
    {
        yield return new WaitForSeconds(0.8f);
        rigid.velocity = Vector2.zero;
        isKnockBack = false;
    }

    
    
}
