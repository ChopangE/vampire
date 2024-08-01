using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necro : MiddleBoss
{
    float castingTimer;
    float timer;
    PoolManager pool;

    public GameObject savePrefabs;
    protected override void Update() {
        base.Update();
        timer += Time.deltaTime;
        if(castingTimer < timer) {
            castingTimer = Random.Range(4.0f, 7.0f);
            timer = 0f;
            Casting();
        }
    }
    public override void Init() {
        base.Init();
        speed = 2.0f;
        castingTimer = 3f;
        timer = 0f;
        pool = GetComponentInChildren<PoolManager>();
        savePrefabs = GameObject.Find("MiddleBossPrefabs");
    }
    void Casting() {
        anim.SetTrigger("Casting");
        SetDoing();
    }
    public void BulletInit() {
        //pool.Get(Random.Range(0,pool.prefabs.Length));
        pool.Get(Random.Range(0, pool.prefabs.Length)).transform.parent = savePrefabs.transform;

    }
}
