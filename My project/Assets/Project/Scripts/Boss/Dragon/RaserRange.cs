using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaserRange : MiddleBossBullet {
    Vector3 playerPos;
    SpriteRenderer sprite;
    //Player player;
    //MiddleBoss boss;
    BoxCollider2D boxCollider;
    Animator anim;
    bool isDone;
    bool isAttack;
    //void OnEnable() {
        
    //    Init();
    //    transform.localPosition = Vector3.zero;
    //    sprite.color = new Color(1, 0, 0, 0.5f);
    //    boxCollider.enabled = false;
    //    StartCoroutine(OnRaser());
    //    anim.speed = 0f;
    //    isDone = false;
    //    isAttack = false;
    //}
    protected override void Init() {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
        player = GameManager.Instance.player;
        //boss = FindAnyObjectByType<MiddleBoss>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        transform.localPosition = Vector3.zero;
        sprite.color = new Color(1, 0, 0, 0.5f);
        boxCollider.enabled = false;
        StartCoroutine(OnRaser());
        anim.speed = 0f;
        isDone = false;
        isAttack = false;

    }
    void Update() {
        transform.position = mBoss.transform.position;
        playerPos = player.transform.position;
        if (isDone) return;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, (playerPos - transform.position).normalized);

    }
   
    IEnumerator OnRaser() {
        float timer = 0f;
        float duration = 2f;
        float startColor = 0.5f;
        float endColor = 1f;
        while (timer < duration) {
            timer += Time.deltaTime;
            sprite.color = new Color(1, 0, 0, Mathf.Lerp(startColor, endColor, timer / duration));
            yield return null;
        }
        isDone = true;
        yield return new WaitForSeconds(1f);
        CallRaser();
    }
    void CallRaser() {
        sprite.color = Color.white;
        anim.speed = 1f;
    }
    public void BoxOn() {
        isAttack = true;
        boxCollider.enabled = true;

    }
    public void End() {
        isAttack = false;
        gameObject.SetActive(false);
        mBoss.EndDoing();
        
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (!isAttack) return;
        base.OnTriggerEnter2D(collision);
    }
}
