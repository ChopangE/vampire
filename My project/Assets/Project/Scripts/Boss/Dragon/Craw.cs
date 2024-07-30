using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craw : MiddleBossBullet
{
    Vector3 playerPos;
    SpriteRenderer sprite;
    BoxCollider2D boxCollider;
    Animator anim;
    bool isDone;
    bool isAttack;
    public Sprite init;
    protected override void Init()
    {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
        player = GameManager.Instance.player;
        //boss = FindAnyObjectByType<MiddleBoss>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        transform.localPosition = Vector3.zero;
        sprite.color = new Color(1, 0, 0, 0.5f);
        boxCollider.enabled = false;
        anim.speed = 0f;
        sprite.sprite = init;
        isDone = false;
        isAttack = false;
        StartCoroutine(OnCraw());

    }
    IEnumerator OnCraw() {
        float timer = 0f;
        float duration = 1f;
        float startColor = 0.5f;
        float endColor = 1f;
        while (timer < duration) {
            timer += Time.deltaTime;
            sprite.color = new Color(1, 0, 0, Mathf.Lerp(startColor, endColor, timer / duration));
            yield return null;
        }
        isDone = true;
        yield return new WaitForSeconds(1f);
        CallCraw();
    }
    void CallCraw() {
        sprite.color = Color.white;
        anim.speed = 1f;
    }
    public void BoxOn() {
        isAttack = true;
        boxCollider.enabled = true;
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (!isAttack) return;
        base.OnTriggerEnter2D(collision);
    }
    void Update() {
        if (isDone) return;
        playerPos = player.transform.position;
        transform.position = playerPos;

    }
    public void End() {
        gameObject.SetActive(false);
        mBoss.EndDoing();
    }
}
