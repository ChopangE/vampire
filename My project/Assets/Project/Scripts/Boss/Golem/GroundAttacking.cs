using System.Collections;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;

public class GroundAttacking : MonoBehaviour
{
    Collider2D coll;
    Golem golem;
    void Awake() {
        coll = GetComponent<Collider2D>();
        golem = GetComponentInParent<Golem>();
    }

    void OnEnable()
    {
        Global.SoundManager.PlaySFX(SFXEnum.Golem_HandDown);
        coll.enabled = false;
    }

    
    public void ColliderOn() {
        coll.enabled = true;
    }
    public void EndAttack() {
        golem.EndDoing();
        gameObject.SetActive(false);
    }
}
