using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilDragon : MiddleBoss
{
    float castingTimer;
    PoolManager pool;
    public GameObject savingPrefabs;
    public override void Init() {
        base.Init();
        speed = 1.6f;
        castingTimer = 5f;
        timer = 0f;
        pool = GetComponentInChildren<PoolManager>();
        savingPrefabs = GameObject.Find("MiddleBossPrefabs");

    }
    protected override void Update() {
        base.Update();
        
        if (!GameManager.Instance.isLive) {
            rigid.velocity = Vector2.zero;
            return;
        }
        timer += Time.deltaTime;
        if (castingTimer < timer) {
            castingTimer = Random.Range(12.0f, 15.0f);
            timer = 0f;
            Casting();
        }
    }
    void Casting() {
        //anim.SetTrigger("Casting");
        SetDoing();
        BulletInit();
    }
    void BulletInit() {
        //pool.Get(Random.Range(0,pool.prefabs.Length));
        pool.Get(Random.Range(0, pool.prefabs.Length)).transform.parent = savingPrefabs.transform;

    }
}
