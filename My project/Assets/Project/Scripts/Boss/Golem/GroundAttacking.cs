using System.Collections;
using System.Collections.Generic;
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
