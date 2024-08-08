using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Golem : MiddleBoss
{
    float castingTimer;
    float timer;
    //PoolManager pool;
    public LayerMask targetLayer;
    public Vector2 attackSize;


    public GameObject savePrefabs;
    protected override void Update() {
        base.Update();
        timer += Time.deltaTime;
        if (castingTimer < timer) {
            castingTimer = Random.Range(6.0f, 10.0f);
            //StartCoroutine(Attacking());
            Attack();
            timer = 0f;
        }
    }
    public override void Init() {
        base.Init();
        speed = 1.7f;
        castingTimer = 7f;
        timer = 0f;
        //pool = GetComponentInChildren<PoolManager>();
        savePrefabs = GameObject.Find("MiddleBossPrefabs");
    }
    IEnumerator Attacking() {
        yield return new WaitForSeconds(2f);
    }
    void Attack() {
        SetDoing();
        anim.SetTrigger("Attack");
        Vector2 center = transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, attackSize, 0,targetLayer);

    }
    void OnDrawGizmos() {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
}
