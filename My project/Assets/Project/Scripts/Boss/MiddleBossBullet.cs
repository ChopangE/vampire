using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleBossBullet : MonoBehaviour
{
    public float damage;
    protected Player player;
    protected MiddleBoss mBoss;
    // Start is called before the first frame update
    void OnEnable()
    {
        Init();
    }

    protected virtual void Init() {
        player = GameManager.Instance.player;
        mBoss = FindAnyObjectByType<MiddleBoss>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        Damaging();
    }

    protected virtual void Damaging() {
        GameManager.Instance.health -= damage;
    }
}
