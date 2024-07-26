using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilDragon : MiddleBoss
{
    float castingTimer;
    float timer;
    PoolManager pool;
    public GameObject savingPrefabs;
    public override void Init() {
        base.Init();
        speed = 2.0f;
        castingTimer = 5f;
        timer = 0f;
        pool = GetComponentInChildren<PoolManager>();
    }
    protected override void Update() {
        base.Update();
        timer += Time.deltaTime;
        if (castingTimer < timer) {
            castingTimer = Random.Range(7.0f, 8.0f);
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
