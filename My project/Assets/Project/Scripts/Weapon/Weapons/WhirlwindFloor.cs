using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

public class WhirlwindFloor : FloorWeapon
{
    [Header("# Utility")]
    public float addPower;

    public GameObject projectile;

    CircleCollider2D coll;
    Vector3 spawnPos;
    private Coroutine pullEnemyCoroutine;


    public override async UniTask Init() {
        await base.Init();
        coll = projectile.GetComponent<CircleCollider2D>();
        
    }

    public override void SpawnBullet() {
        base.SpawnBullet();
        SpawnWhirlwind();
        PullEnemy();
    }

    public void SpawnWhirlwind() {
        Global.SoundManager.PlaySFX(SFXEnum.WindFloor);
        Vector3 playerPos = player.transform.position;
        spawnPos = playerPos + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f),0f);
        
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        projectile = bullet.gameObject;
        var projectileBullet = bullet.GetComponent<Bullet>();
        projectileBullet.Init(damage, 
        -1, 
        Vector2.zero, 
        duration: duration, 
        size: size,
        criticalChancePercent: criticalChancePercent, 
        criticalDamagePercent: criticalDamagePercent);
        
        projectile.transform.position = spawnPos;
        OnPlay();
    }

    public void OffPlay() {
        projectile.SetActive(false);
        if (pullEnemyCoroutine != null) {
            StopCoroutine(pullEnemyCoroutine);
            pullEnemyCoroutine = null;
        }
    }
    
    public void OnPlay() {
        projectile.SetActive(true);
    }

    void PullEnemy() {
        if (!projectile || !projectile.activeInHierarchy) return;
        Collider2D[] enemyColls = Physics2D.OverlapCircleAll(projectile.transform.position, coll.radius, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider2D enemyColl in enemyColls) {
            Enemy enemy = enemyColl.GetComponent<Enemy>();
            if (enemy != null) {
                Vector3 dir = (projectile.transform.position - enemyColl.transform.position).normalized;
                enemy.GetAddForce(dir * addPower);
            }
        }
    }
}
