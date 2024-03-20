using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashFinish : MonoBehaviour
{
    Boss boss;
    void Awake() {
        boss = GetComponentInParent<Boss>();
    }
    public void SmashFinishing() {
        boss.anim.SetBool("Smashing",false);
    }
}
