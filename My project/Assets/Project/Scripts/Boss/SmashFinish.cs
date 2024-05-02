using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmashFinish : MonoBehaviour
{
    Boss boss;
    Transform target;
    bool isTargeting = true;
    Transform dusts;
    void Awake() {
        boss = GetComponentInParent<Boss>();
        target = GameManager.Instance.player.transform;
        
    }
    void Start() {
        dusts = transform.GetChild(1);
    }
    void Update() {
        if (isTargeting) {
            Vector3 dir = target.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
            transform.eulerAngles += new Vector3(0, 0, 30f);
            
        }

    }
    

    public void SmashFinishing() {
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;

        boss.anim.SetBool("Smashing",false);
        //boss.isPatterning = false;
        Invoke("StartPositionToTarget", 0.3f);
        //StartCoroutine(FinshPattern());
    }

    
    void StartPositionToTarget() {
        isTargeting = true;
    }

    public void StopPositionToTarget() {
        isTargeting = false;
    }

    public void MakeDustAndDamaging() {
        dusts.gameObject.SetActive(true);
        transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
    }

    public void DoneDust() {
        dusts.gameObject.SetActive(false);

    }


}
