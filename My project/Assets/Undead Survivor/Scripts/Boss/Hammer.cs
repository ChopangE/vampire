using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public Boss boss;
    void Awake() {
        boss = GetComponentInParent<Boss>();
    }
    public void AnimOff() {
        transform.GetChild(0).gameObject.SetActive(false);
        boss.anim.SetBool("Hamming", false);
        gameObject.SetActive(false);
    }

    public void EarthQuakeOn() {
        transform.GetChild(0).gameObject.SetActive(true);
        Collider2D hit = Physics2D.OverlapBox(boss.transform.position - new Vector3(0, 7.5f, 0), new Vector2(13, 4), 0, LayerMask.GetMask("Player"));
        if(hit != null) {
            hit.GetComponent<Player>().rigid.AddForce(new Vector2(0, -60), ForceMode2D.Impulse);
            Debug.Log("hi");
        }
    }
}
