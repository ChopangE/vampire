using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Golem : MiddleBoss
{
    CameraControl CC;
    float castingTimer;
    float timer;
    //PoolManager pool;
    public LayerMask targetLayer;
    public Vector2 attackSize;
    public float meleeDamage;
    public GameObject childAttack;
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
        childAttack = transform.GetChild(0).gameObject;
        CC = FindObjectOfType<CameraControl>();

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
        Collider2D coll = Physics2D.OverlapBox(transform.position, attackSize, 0, targetLayer);
        if (coll) {
            coll.gameObject.GetComponent<Rigidbody2D>().AddForce((coll.transform.position - transform.position).normalized * 100f, ForceMode2D.Impulse);
            GameManager.Instance.health -= meleeDamage;
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
