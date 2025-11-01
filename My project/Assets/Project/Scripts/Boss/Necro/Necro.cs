using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necro : MiddleBoss
{
    float castingTimer;
    PoolManager pool;

    public GameObject savePrefabs;
    protected override void Update() {
        base.Update();
        if (!GameManager.Instance.isLive) {
            rigid.velocity = Vector2.zero;
            return;
        }
        timer += Time.deltaTime;
        if(castingTimer < timer) {
            castingTimer = Random.Range(12.0f, 15.0f);
            timer = 0f;
            Casting();
        }
    }
    public override void Init() {
        base.Init();
        // Reduce maxHealth by 30% for Necro
        maxHealth *= 0.7f;
        health = maxHealth;
        speed = 1.6f;
        castingTimer = 3f;
        timer = 0f;
        pool = GetComponentInChildren<PoolManager>();
        savePrefabs = GameObject.Find("MiddleBossPrefabs");
    }
    public override void Dead()
    {
        base.Dead();
        // First_Magician_Defeat 업적 클리어
        var achievementsManager = FindAnyObjectByType<AchievementsManager>();
        if (achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("First_Magician_Defeat");
        }

        // Magician 처치 카운트 증가 및 저장
        var userDataManager = Manager.Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (killSaveData.ContainsKey("Magician"))
            {
                killSaveData["Magician"]++;
            }
            else
            {
                killSaveData["Magician"] = 1;
            }
            userDataManager.Save();

            // 10번 처치 시 Magician_Slayer 업적 클리어
            if (killSaveData["Magician"] >= 10)
            {
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Magician_Slayer");
                }
            }
        }
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
