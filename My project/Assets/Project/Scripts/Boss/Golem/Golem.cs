using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Data;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Golem : MiddleBoss
{
    CameraControl CC;
    float castingTimer;
    //PoolManager pool;
    public LayerMask targetLayer;
    public Vector2 attackSize;
    public float meleeDamage;
    public GameObject childAttack;
    public GameObject savePrefabs;
    protected override void Update() {
        base.Update();
        if (!GameManager.Instance.isLive) {
            rigid.velocity = Vector2.zero;
            return;
        }
        timer += Time.deltaTime;
        if (castingTimer < timer) {
            castingTimer = Random.Range(12f, 15.0f);
            Attack();
            timer = 0f;
        }
    }
    public override void Init() {
        base.Init();
        speed = 1.6f;
        castingTimer = 7f;
        timer = 0f;
        //pool = GetComponentInChildren<PoolManager>();
        savePrefabs = GameObject.Find("MiddleBossPrefabs");
        childAttack = transform.GetChild(0).gameObject;
        CC = FindObjectOfType<CameraControl>();

    }
    public override void Dead()
    {
        base.Dead();
        // First_Golem_Defeat 업적 클리어
        var achievementsManager = FindAnyObjectByType<AchievementsManager>();
        if (achievementsManager != null)
        {
            achievementsManager.AchivementTrueByID("First_Golem_Defeat");
        }

        // Golem 처치 카운트 증가 및 저장
        var userDataManager = Global.UserDataManager;
        if (userDataManager != null)
        {
            var killSaveData = userDataManager.storage.killSaveData;
            if (killSaveData.ContainsKey("Golem"))
            {
                killSaveData["Golem"]++;
            }
            else
            {
                killSaveData["Golem"] = 1;
            }
            userDataManager.Save();

            // 10번 처치 시 Golem_Slayer 업적 클리어
            if (killSaveData["Golem"] >= 10)
            {
                if (achievementsManager != null)
                {
                    achievementsManager.AchivementTrueByID("Golem_Slayer");
                }
            }
        }
    }
    public void StopCameraShaking() {
        CC.StopCameraShake();
    }
    public void StartCameraShaking() {
        CC.ShakeCamera();
    }
    IEnumerator Attacking() {
        yield return new WaitForSeconds(2f);
    }
    void Attack() {
        SetDoing();
        Collider2D coll = Physics2D.OverlapBox(transform.position, attackSize, 0, targetLayer);
        if (coll) {
            anim.SetTrigger("Attack");
        }
        else {
            anim.SetTrigger("GroundAttack");
        }

    }
    void OnDrawGizmos() {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }
    public void Melee_() {
        Global.SoundManager.PlaySFX(SFXEnum.Golem_Punch);
        Collider2D coll = Physics2D.OverlapBox(transform.position, attackSize, 0, targetLayer);
        if (coll) {
            coll.gameObject.GetComponent<Rigidbody2D>().AddForce((coll.transform.position - transform.position).normalized * 100f, ForceMode2D.Impulse);
            var damage = GameManager.Instance.maxHealth * 0.4f;
            GameManager.Instance.Health -= damage;
            Player player = coll.GetComponent<Player>();
            if (player) {
                player.Stopping();
            }
        }
    }

    public void SetGroundAttack() {
        childAttack.gameObject.SetActive(true);
        StartCameraShaking();
    }
}
