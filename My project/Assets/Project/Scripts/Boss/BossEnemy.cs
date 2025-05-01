using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using InGame;
using InGame.Data;
using Manager;
using UnityEngine;

public class BossEnemy : DamageObject
{
    public float speed;
    public RuntimeAnimatorController[] animCon;
    Rigidbody2D target;
    bool isLive;
    private float lastDamageTime = 0f;
    private const float DAMAGE_INTERVAL = 1f;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;

    protected override void Awake() {
        base.Awake();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    
    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;
        if (!isLive) return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(nextVec + rigid.position);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;
        if (isLive) {
            spriter.flipX = target.position.x < rigid.position.x;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        health = maxHealth;
    }

    private void CalculateDamage(DamageData damageData)
    {
        float finalDamage = damageData.damage;
        bool isCritical = damageData.isCritical;

        finalDamage = finalDamage * (1 + GameManager.Instance.player.damageBonus);

        health -= finalDamage;
        GameManager.DamageTextPoolManager.SpawnDamageText(transform.position, finalDamage, isCritical, false);

        if (GameManager.Instance.weaponController.isBonusDamage && damageData.isBonus)
        {
            float bonusDamage = finalDamage * GameManager.Instance.weaponController.bonusDamage;
            health -= bonusDamage;
            DelayedSpawnDamageText(transform.position, bonusDamage, 0.1f, true).Forget();
        }
    }

    private async UniTaskVoid DelayedSpawnDamageText(Vector3 position, float damage, float delay, bool isBonus = false)
    {
        await UniTask.Delay((int)(delay * 1000));
        GameManager.DamageTextPoolManager.SpawnDamageText(position, damage, false, isBonus);
    }

    public void Init(int index) {
        anim.runtimeAnimatorController = animCon[index];
    }

    public override void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;
        if (!isLive) return;

        if (collision.TryGetComponent(out Bullet bullet))
        {
            Global.SoundManager.PlayHitSFX(SFXEnum.Monster_Hit_1, isShootCooldown: false);
            CalculateDamage(bullet.CalculateDamage());

            if (collision.CompareTag("Melee")) {
                StartCoroutine(KnockBack());
            }
        }

        if (health <= 0) {
            Dead();
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        if (!isLive) return;
        if (!collision.TryGetComponent(out Bullet bullet)) return;

        if (Time.time >= lastDamageTime + DAMAGE_INTERVAL)
        {
            CalculateDamage(bullet.CalculateDamage());
            lastDamageTime = Time.time;

            if (health <= 0) {
                Dead();
            }
        }
    }

    public override void Dead() {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        gameObject.SetActive(false);
    }

    IEnumerator KnockBack() {
        yield return null;
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dir = transform.position - playerPos;
        rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);
    }
}
