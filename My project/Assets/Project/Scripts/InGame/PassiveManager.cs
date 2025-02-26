using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public void SetPassiveItem(PassiveItemDataInfo data)
    {
        int currentLevel = data.curLevel;
        switch (data.passiveId)
        {
            case PassiveId.Health:
                // 레벨 0은 기본 체력, 1레벨부터 10씩 증가
                GameManager.Instance.maxHealth = GameManager.Instance.baseMaxHealth + (10 * Math.Max(0, currentLevel));
                break;
            case PassiveId.Speed:
                // 레벨 0은 기본 속도, 1레벨부터 5%씩 증가
                GameManager.Instance.player.speed = GameManager.Instance.player.baseSpeed * (1 + (0.05f * Math.Max(0, currentLevel)));
                break;
            case PassiveId.Damage:
                GameManager.Instance.player.damageBonus = GameManager.Instance.player.damageBonus + (0.1f * Math.Max(0, currentLevel));
                break;
            case PassiveId.ExpGainIncrease:
                // 레벨 0은 보너스 없음(1배수), 1레벨부터 5%씩 증가
                GameManager.Instance.expBonus = 1 + (0.05f * Math.Max(0, currentLevel));
                break;
            case PassiveId.ExpGainRangeIncrease:
                // 레벨 0은 보너스 없음(1배수), 1레벨부터 10%씩 증가
                GameManager.Instance.expRangeBonus = 1 + (0.1f * Math.Max(0, currentLevel));
                break;
            case PassiveId.Defense:
                GameManager.Instance.Defense = GameManager.Instance.baseDefense + (10 * Math.Max(0, currentLevel));
                break;
            case PassiveId.CriticalDamage:
                GameManager.Instance.criticalDamage = GameManager.Instance.baseCriticalDamage + (0.1f * Math.Max(0, currentLevel));
                break;
            case PassiveId.CriticalChance:
                GameManager.Instance.criticalChance = GameManager.Instance.baseCriticalChance + (0.1f * Math.Max(0, currentLevel));
                break;
            default:
                break;
        }
    }

    public void Init()
    {
        if(Global.UserDataManager.storage.passiveItemDataInfoList.Count == 0)
        {
            Global.DataManager.LoadData();
        }
        foreach (var item in Global.UserDataManager.storage.passiveItemDataInfoList)
        {
            SetPassiveItem(item);
        }
    }

}
