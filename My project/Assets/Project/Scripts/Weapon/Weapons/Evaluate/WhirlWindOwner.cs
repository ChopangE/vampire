using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class WhirlWindOwner : FloorWeapon
{
    [Header("# Utility")]
    public float addPower;

    public GameObject projectile;

    CircleCollider2D coll;
    Vector3 spawnPos;

    public override async UniTask Init() {
        await base.Init();
        coll = projectile.GetComponent<CircleCollider2D>();

        maxCooldown = 20f;
        remainingCooldown = maxCooldown;
        duration = 10f;
    }

    public override void SpawnBullet() {
        base.SpawnBullet();
        SpawnWhirlwind();
        PullEnemy();
    }

    public void SpawnWhirlwind() {
        projectile = GameManager.Instance.pool.Get(prefabId);
        WhirlBullet whirlBullet = projectile.GetComponent<WhirlBullet>();
        whirlBullet.Init(damage, -1, Vector2.zero, duration: duration, criticalChancePercent: criticalChancePercent, criticalDamagePercent: criticalDamagePercent);
        projectile.transform.SetParent(player.transform, worldPositionStays: false);
        projectile.transform.localPosition = new Vector3(0, -0.75f, 0);
        OnPlay();
    }

    public void OffPlay() {
        projectile.SetActive(false);
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
