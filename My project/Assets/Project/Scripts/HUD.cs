using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Page;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health, Shield, BossHealth }
    public InfoType type;
    Text myText;
    Slider mySlider;
    
    // Cached previous values to avoid updating UI/text every frame (and allocating strings)
    int lastLevel = int.MinValue;
    int lastKill = int.MinValue;
    int lastMin = int.MinValue, lastSec = int.MinValue;
    float lastExpNormalized = float.NaN;
    float lastHealthNormalized = float.NaN;
    float lastShieldNormalized = float.NaN;
    float lastBossHealthNormalized = float.NaN;

    // small lookup to avoid formatting allocations for mm:ss (00..99)
    static readonly string[] TwoDigits = CreateTwoDigits();

    static string[] CreateTwoDigits()
    {
        var arr = new string[100];
        for (int i = 0; i < 100; i++) arr[i] = i.ToString("D2");
        return arr;
    }

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void Start()
    {
        if (type == InfoType.Time)
        {
            InGameMainPage inGameMainPage = GetComponentInParent<InGameMainPage>();
            gameObject.SetActive(inGameMainPage.ActiveTimer);
        }
        if (type == InfoType.BossHealth)
        {
            if(GameManager.Instance.BossHealth > 0)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    void LateUpdate()
    {
        var gm = GameManager.Instance;
        if (!gm.isLive) return;

        switch (type)
        {
            case InfoType.Exp:
            {
                float curExp = gm.curExp;
                float maxExp = gm.GetNextExpRequired();
                float normalized = (maxExp <= 0f) ? 0f : (curExp / maxExp);
                if (!Mathf.Approximately(normalized, lastExpNormalized))
                {
                    mySlider.value = normalized;
                    lastExpNormalized = normalized;
                }
                break;
            }

            case InfoType.Level:
            {
                int lvl = Mathf.FloorToInt(gm.level);
                if (lvl != lastLevel)
                {
                    myText.text = "Lv." + lvl;
                    lastLevel = lvl;
                }
                break;
            }

            case InfoType.Kill:
            {
                int k = Mathf.FloorToInt(gm.kill);
                if (k != lastKill)
                {
                    myText.text = k.ToString();
                    lastKill = k;
                }
                break;
            }
            case InfoType.Time:
            {
                float remainTime = gm.maxGameTime - gm.gameTime;
                if (remainTime <= 0)
                {
                    gm.StageClear();
                    remainTime = 0;
                }
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                if (min != lastMin || sec != lastSec)
                {
                    string minStr = (min >= 0 && min < TwoDigits.Length) ? TwoDigits[min] : min.ToString("D2");
                    string secStr = (sec >= 0 && sec < TwoDigits.Length) ? TwoDigits[sec] : sec.ToString("D2");
                    myText.text = minStr + ":" + secStr;
                    lastMin = min;
                    lastSec = sec;
                }
                break;
            }
            case InfoType.Health:
            {
                float curHealth = gm.Health;
                float maxHealth = gm.maxHealth;
                float normalized = (maxHealth <= 0f) ? 0f : (curHealth / maxHealth);
                if (!Mathf.Approximately(normalized, lastHealthNormalized))
                {
                    mySlider.value = normalized;
                    lastHealthNormalized = normalized;
                }
                break;
            }
            case InfoType.Shield:
            {
                float curShield = gm.Shield;
                float maxShield = gm.maxHealth;
                float normalized = (maxShield <= 0f) ? 0f : (curShield / maxShield);
                if (!Mathf.Approximately(normalized, lastShieldNormalized))
                {
                    mySlider.value = normalized;
                    lastShieldNormalized = normalized;
                }
                break;
            }
            case InfoType.BossHealth:
            {
                float curBossHealth = gm.BossHealth;
                float maxBossHealth = gm.maxBossHealth;
                float normalized = (maxBossHealth <= 0f) ? 0f : (curBossHealth / maxBossHealth);
                if (!Mathf.Approximately(normalized, lastBossHealthNormalized))
                {
                    mySlider.value = normalized;
                    lastBossHealthNormalized = normalized;
                }
                break;
            }

        }
    }


}
