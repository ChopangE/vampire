using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    //* 다양한 스탯들 모음
    public enum PlayerStat //* 플레이어 기본 스탯
    {
        Health, //* 체력
        AttackDamage, //* 공격력
        MovementSpeed, //* 이동속도
        
    }
    public enum PlayerPassiveStat //* 플레이어 영구 패시브 
    {
        Passive_MaxHealth, //* 체력
        Passive_AttackDamage, //* 공격력
        Passive_Defense,
        Passive_GoldBonus,
        Passive_RevivePossibility,
        Passive_MoveSpeed, //* 이동속도
        Passive_ProjectileSpeed, //* 발사 속도
        Passive_EXPMagnetRange,
        Passive_HealthRegeneration,
        Passive_EXPBonus,
        Passive_CriticalDamage,
        Passive_CriticalRate, //* 치확
    }
}