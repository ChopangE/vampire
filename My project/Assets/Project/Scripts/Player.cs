using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Rigidbody2D rigid;
    public SpriteRenderer sprite;
    Animator anim;
    public Scanner scan;
    public float speed;
    public bool isBonusDamage;
    public float bonusDamage = 0.2f;
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
        
        Vector2 movePosition = rigid.position + inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(movePosition);
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
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
