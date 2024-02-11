using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator anim;
    public float speed;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    
    }

    // Update is called once per frame
    

    void FixedUpdate() {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }

    void OnMove(InputValue value) {
    
        inputVec = value.Get<Vector2>();            //이미 normalized 적용
    
    }

    void LateUpdate() {

        anim.SetFloat("Speed", inputVec.magnitude);
        
        if(inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ? true : false; 
        }
        
    }

}
