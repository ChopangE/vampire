using System.Collections;
using System.Collections.Generic;
using InGame.Data;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float criticalDamagePercent;
    public float criticalChancePercent;
    public int per;
    public Rigidbody2D rb;
    public float duration;
    public bool canStun;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(duration == 0)
        {
            duration = Random.Range(3f, 5f);
        }
    }

    public virtual void Init(float damage, int per, Vector3 dir, bool canStun = false,
    float duration = 0f, float criticalDamagePercent = 0f, float criticalChancePercent = 0f)

    {
        this.damage = damage;
        this.criticalDamagePercent = criticalDamagePercent;
        this.criticalChancePercent = criticalChancePercent;
        this.per = per;
        this.canStun = canStun;
        this.duration = duration;
        
        if (duration == 0)
        {
            if(per <= -100)
            {
                this.duration = Random.Range(3f, 5f);
            }
        }
        
        if (per <= -1 && duration != 0)
        {
            StopAsync().Forget();
        }

        if (per > -1)
        {
            rb.velocity = dir * 7f;
        }
    }

    private async UniTaskVoid StopAsync()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
        if (this != null && gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bound"))
        {
            gameObject.SetActive(false);
            return;
        }
        if ((!collision.CompareTag("Enemy") && !collision.CompareTag("BossEnemy")) || per == -1)
        {
            return;
        }

        if (canStun)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyStun(enemy).Forget();
            }
        }

        per--;

        if (per == -1)
        {
            rb.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    protected virtual async UniTask ApplyStun(Enemy enemy, float duration = 1f)
    {
        enemy.isStunned = true;
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
        if (enemy != null)
        {
            enemy.isStunned = false;
        }
    }

    public virtual DamageData CalculateDamage()
    {
        DamageData damageData = new DamageData(damage, false, false);
        
        if (GameManager.Instance.weaponController.isBonusDamage)
        {
            damageData.isBonus = true;
        }

        if(criticalChancePercent > 0)
        {
            float random = Random.Range(0f, 1f);
            if(random < criticalChancePercent + GameManager.Instance.criticalChance)
            {
                damageData.isCritical = true;
                float weaponCriticalMultiplier = 1f + criticalDamagePercent;
                float passiveCriticalMultiplier = GameManager.Instance.criticalDamage;
                damageData.damage = damage * weaponCriticalMultiplier * passiveCriticalMultiplier;
            }
        }
        
        return damageData;
    }
}

