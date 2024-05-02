using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public Rigidbody2D target;
    bool isLive;
    public Animator anim;
    public Smash smash;
    public GameObject[] bossLevel;
    public float smashTime;
    CameraControl CC;
    Collider2D coll;
    SpriteRenderer sprite;
    BossWeapon[] weapons;
    float Timer;
    float Timer2;
    int levelIndex;
    void Awake() {
        coll = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weapons = GetComponentsInChildren<BossWeapon>();
        smash = GetComponentInChildren<Smash>(true);
        CC = FindObjectOfType<CameraControl>();
    }
    
    public void Bottle() {
        weapons[0].Shut(0, 5);
    }
    public void Crows() {
        weapons[0].Range(2, 10);
    }

    void Update() {
        if (!GameManager.Instance.isLive) return;
        if (!isLive) {
            transform.Translate(0, -5 * Time.deltaTime, 0);
        }

        else {
            if (BossManager.Instance.phase >= 2) {
                Timer += Time.deltaTime;
                if (Timer > 5f) {
                    Timer = 0f;
                    bossLevel[levelIndex++].SetActive(true);
                    levelIndex = Mathf.Min(bossLevel.Length - 1, levelIndex);
                }
            }


            Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4), 0, LayerMask.GetMask("Player"));

            if (BossManager.Instance.phase >= 1) {

                if (Timer2 > smashTime) {

                    Timer2 = 0f;
                    if (hit != null) {
                        anim.SetBool("Hammer", true);
                        //GameManager.Instance.player.rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
                    }
                    else {
                        anim.SetBool("Smash", true);
                        //smash.gameObject.SetActive(true); Deleted
                    }

                }
                Timer2 += Time.deltaTime;
            }
        }
    }
    
    
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - new Vector3(0,7.5f,0), new Vector2(13,4));
    }
    void OnEnable() {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        sprite.sortingOrder = 1;
        health = maxHealth;
    }
   
    public void Hammer() {
        anim.SetBool("Hammer", false);
        anim.SetBool("Hamming", true);
        Invoke("StartShake", 0.2f);
    }
    
   public void StartShake() {
        CC.ShakeCamera();
   }
    public void AnimOff() {
        transform.GetChild(3).gameObject.SetActive(false);
        anim.SetBool("Hamming", false);
    }

    public void EarthQuakeOn() {
        transform.GetChild(3).gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4), 0, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            Player player = hit.GetComponent<Player>();
            player.rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
            player.Stopping();
        }
    }

    public void TracePlayerOn() {
        transform.GetChild(2).GetChild(0).GetComponent<SmashUptoDown>().OnColor();

    }

    public void StopToTracing() {
        transform.GetChild(2).GetComponent<SmashFinish>().StopPositionToTarget();
    }

    public void SmashUptoDown() {
        anim.SetBool("Smash", false);
        anim.SetBool("Smashing", true);
        Transform child = transform.GetChild(2);
        child.GetChild(0).GetComponent<SmashUptoDown>().OffColor();
        child.gameObject.GetComponent<Animator>().SetTrigger("isSmash");

    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Bullet")) return;
        if (!isLive) return;

        health -= collision.GetComponent<Bullet>().damage;

        
        if(health < 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = -2;
            anim.SetBool("Dead", true);
            CC.ShakeCamera();
        }

    }
    void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Floor")) return;
        if (!isLive) return;

        health -= collision.GetComponent<FloorWeapon>().damage;

        if (health <= 0) {
            isLive = false;
            coll.enabled = false;
            sprite.sortingOrder = -2;
            anim.SetBool("Dead", true);
            CC.ShakeCamera();
        }
    }
}
