using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WhirlwindFloor : FloorWeapon
{
    [Header("# Utility")]
    public float curTime;
    public float addPower;

    public GameObject projectile;

    CircleCollider2D coll;
    Vector3 spawnPos;
    bool isTiming;
    //Animator anim;
    public override void Init(ItemData data) {
        // Basic Set
        base.Init(data);
        isTiming = false;
    }

    void Update() {
        if (!GameManager.Instance.isLive) return;
        //if(!isTiming)
        timer += Time.deltaTime;
        if (timer > 5f) {
            isTiming = true;
            timer = 0f;
            Vector3 playerPos = player.transform.position;
            spawnPos = playerPos + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f),0f);
            projectile = GameManager.Instance.pool.Get(8);
            projectile.transform.position = spawnPos;
            //coll = projectile.GetComponent<CircleCollider2D>();
            //OnPlay();
        }
    }

    public void OffPlay() {
        projectile.SetActive(false);
        isTiming = false;
    }
    
    public void OnPlay() {
        projectile.SetActive(true);
    }
    
    void PullEnemy() {
        Collider2D[] enemyColls = Physics2D.OverlapCircleAll(projectile.transform.position, coll.radius, LayerMask.NameToLayer("Enemy"));
        foreach (Collider2D enemyColl in enemyColls) {
            Debug.Log("check");
            enemyColl.GetComponent<Enemy>().GetAddForce(transform.position);
        }
    }
}
