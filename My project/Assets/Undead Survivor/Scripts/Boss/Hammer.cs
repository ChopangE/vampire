using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public Boss boss;
    void Awake() {
        boss = GetComponentInParent<Boss>();
    }
    void AnimOff() {
        transform.GetChild(0).gameObject.SetActive(false);
        boss.anim.SetBool("Hammer", false);
        gameObject.SetActive(false);
    }

    void EarthQuakeOn() {
        transform.GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.player.rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
    }
}
