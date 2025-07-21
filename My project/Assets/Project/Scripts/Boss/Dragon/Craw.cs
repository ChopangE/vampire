using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;
using Cysharp.Threading.Tasks;

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
        OnCrawAsync().Forget();

    }
    async UniTaskVoid OnCrawAsync() {
        float timer = 0f;
        float duration = 1f;
        float startColor = 0.5f;
        float endColor = 1f;
        while (timer < duration) {
            timer += Time.deltaTime;
            sprite.color = new Color(1, 0, 0, Mathf.Lerp(startColor, endColor, timer / duration));
            await UniTask.Yield();
        }
        isDone = true;
        await UniTask.Delay(1000); // 1초를 밀리초로 변환
        CallCraw();
    }
    void CallCraw() {
        sprite.color = Color.white;
        anim.speed = 1f;
        Global.SoundManager.PlaySFX(SFXEnum.Dragon_Craw);
    }
    public void BoxOn() {
        isAttack = true;
        boxCollider.enabled = true;
    }
    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (!isAttack) return;
        base.OnTriggerEnter2D(collision);
    }

    protected override void Damaging() {
        var damage = GameManager.Instance.maxHealth * 0.4f;
        GameManager.Instance.Health -= damage;
        
        // 출혈 상태 적용: 10초 지속, 5초 동안 1초마다 최대체력의 1% 데미지
        GameManager.Instance.player.ApplyBleed(10f, 5f, 0.01f);
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
