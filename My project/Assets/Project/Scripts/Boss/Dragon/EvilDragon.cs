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
    public override void Dead()
    {
        base.Dead();
        // First_Dragon_Defeat 업적 클리어
        var achievementsManager = FindAnyObjectByType<AchievementsManager>();
        if (achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("First_Dragon_Defeat");
        }

        // Dragon 처치 카운트 증가 및 저장
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (killSaveData.ContainsKey("Dragon"))
            {
                killSaveData["Dragon"]++;
            }
            else
            {
                killSaveData["Dragon"] = 1;
            }
            userDataManager.Save();

            // 10번 처치 시 Dragon_Slayer 업적 클리어
            if (killSaveData["Dragon"] >= 10)
            {
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Dragon_Slayer");
                }
            }
        }
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
