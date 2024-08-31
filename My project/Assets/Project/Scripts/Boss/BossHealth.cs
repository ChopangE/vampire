using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    Slider mySlider;
    private Boss _boss;
    void Awake() {
        mySlider = GetComponent<Slider>();
    }
    
    void OnEnable()
    {
        var objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var b in objs) {
            b.TryGetComponent(out _boss);
        }
    }
    void LateUpdate() {
        if (!GameManager.Instance.isLive && _boss == null) return;
        float curHealth = _boss.health;
        float maxHealth = _boss.maxHealth;
        mySlider.value = curHealth / maxHealth;
    }


}
