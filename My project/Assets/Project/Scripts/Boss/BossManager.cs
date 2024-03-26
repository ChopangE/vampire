using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager instance;

    Boss boss;
    bool isDead;
    int bossLevel = 0;
    float[] phaseLevel = { 0.7f, 0.3f };

    public int phase = 0;
    void Awake() {
        boss = FindObjectOfType<Boss>();
    }
    
    public static BossManager Instance {
        get {
            if (instance == null) {
                var obj = FindObjectOfType<BossManager>();
                if (obj != null) {
                    instance = obj;
                }
                else {
                    var newObj = new GameObject().AddComponent<BossManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    void Update() {
        float nowHealth = boss.health / boss.maxHealth;
        if (nowHealth < phaseLevel[bossLevel]) {
            phase++;
            bossLevel = Mathf.Min(bossLevel + 1, phaseLevel.Length - 1);
        }
    }

   
}
