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
        if (!GameManager.Instance.isLive) return;

        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.Instance.curExp;
                float maxExp = GameManager.Instance.GetNextExpRequired();
                mySlider.value = curExp / maxExp;
                break;

            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.Instance.level);
                break;

            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.Instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager.Instance.maxGameTime - GameManager.Instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                if (remainTime <= 0)
                {
                    GameManager.Instance.StageClear();
                }
                break;
            case InfoType.Health:
                float curHealth = GameManager.Instance.Health;
                float maxHealth = GameManager.Instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.Shield:
                float curShield = GameManager.Instance.Shield;
                float maxShield = GameManager.Instance.maxHealth;
                mySlider.value = curShield / maxShield;
                break;
            case InfoType.BossHealth:
                float curBossHealth = GameManager.Instance.BossHealth;
                float maxBossHealth = GameManager.Instance.maxBossHealth;
                mySlider.value = curBossHealth / maxBossHealth;
                break;


        }
    }


}
