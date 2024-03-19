using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Boss boss;

    Slider mySlider;
    
    void Awake() {
        mySlider = GetComponent<Slider>();
    }
    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;
        float curHealth = boss.health;
        float maxHealth = boss.maxHealth;
        mySlider.value = curHealth / maxHealth;
    }


}
