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
                GameManager.Instance.baseMaxHealth += (10 * Math.Max(0, currentLevel));
                break;
            case PassiveId.Speed:
                GameManager.Instance.player.speed = GameManager.Instance.player.baseSpeed * (1 + (0.05f * Math.Max(0, currentLevel)));
                break;
            case PassiveId.Damage:
                GameManager.Instance.player.damageBonus += (0.05f * Math.Max(0, currentLevel));
                break;
            case PassiveId.ExpGainIncrease:
                GameManager.Instance.expBonus *= (1 + (0.05f * Math.Max(0, currentLevel)));
                break;
            case PassiveId.ExpGainRangeIncrease:
                GameManager.Instance.expRangeBonus *= (1 + (0.1f * Math.Max(0, currentLevel)));
                break;
            case PassiveId.Defense:
                GameManager.Instance.Defense += (1 * Math.Max(0, currentLevel)); // 방어력 1당 5% 데미지 감소
                break;
            case PassiveId.CriticalDamage:
                GameManager.Instance.criticalDamage += (0.1f * Math.Max(0, currentLevel));
                break;
            case PassiveId.CriticalChance:
                GameManager.Instance.criticalChance += (0.1f * Math.Max(0, currentLevel));
                break;
            default:
                break;
        }
    }

    public void Init()
    {
        // 기존 패시브 아이템 적용
        if (Global.UserDataManager.storage.passiveItemDataInfoList.Count == 0)
        {
            Global.DataManager.LoadData();
        }
        foreach (var item in Global.UserDataManager.storage.passiveItemDataInfoList)
        {
            SetPassiveItem(item);
        }

        // PlayerPassive 효과 적용
        ApplyPlayerPassives();
    }

    private void ApplyPlayerPassives()
    {
        var passives = Global.StatsUpgradeManager.GetAllPlayerPassives();
        foreach (var passive in passives)
        {
            var value = passive.GetUpgradeValueConvert() / 100f;
            switch (passive.name)
            {
                case "MaxHealth":
                    GameManager.Instance.baseMaxHealth *= (1 + value);
                    break;
                case "AttackDamage":
                    GameManager.Instance.player.damageBonus += value;
                    break;
                case "Defense":
                    GameManager.Instance.Defense += value * 100f; // 백분율 값을 방어력 포인트로 변환
                    break;
                case "GoldBonus":
                    GameManager.Instance.goldBonus *= (1 + value);
                    break;
                case "MovementSpeed":
                    GameManager.Instance.player.speed *= (1 + value);
                    break;
                case "ProjectileSpeed":
                    GameManager.Instance.projectileSpeedBonus *= (1 + value);
                    break;
                case "EXPMagnetRange":
                    GameManager.Instance.expRangeBonus *= (1 + value);
                    break;
                case "HealthRegeneration":
                    GameManager.Instance.healthRegeneration += value;
                    break;
                case "EXPBonus":
                    GameManager.Instance.expBonus *= (1 + value);
                    break;
                case "CritDamage":
                    GameManager.Instance.criticalDamage *= (1 + value);
                    break;
                case "CritRate":
                    GameManager.Instance.criticalChance *= (1 + value);
                    break;
                case "RevivePossibility":
                    GameManager.Instance.revivePossibility += value * 100f;
                    break;
            }
        }
    }
}
