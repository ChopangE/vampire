using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public event Action<Weapon> OnWeaponActivated;

    [ShowInInspector]
    private List<Weapon> weapons = new List<Weapon>();
    
    public int maxActiveWeaponCount = 6;
    public IReadOnlyList<Weapon> ActiveWeapons => weapons.Where(w => w.gameObject.activeSelf).ToList();
    public IReadOnlyList<Weapon> Weapons => weapons;
    private float damageMultiplier = 1f;

    [LabelText("추가 무기 소스(펫 등 다른 친구들꺼)")]
    [SerializeField] private List<GameObject> additionalWeaponSources;

    [LabelText("보너스 데미지")]
    public bool isBonusDamage;
    public float bonusDamage = 0.2f;

    void Awake()
    {
        // 자신의 자식들의 Weapon 찾기
        var ownWeapons = GetComponentsInChildren<Weapon>(true);
        
        // 추가 소스들의 Weapon 찾기
        var additionalWeapons = additionalWeaponSources
            .SelectMany(source => source.GetComponentsInChildren<Weapon>(true));
        
        // 모든 무기 합치고 중복 제거
        weapons = ownWeapons.Concat(additionalWeapons)
            .Distinct()
            .ToList();
    }

    void Update()
    {
        foreach (Weapon weapon in ActiveWeapons)
        {
            weapon.Attack();
        }
    }

    public void ActivateWeapon(Weapon weapon)
    {
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);
        }
        
        weapon.gameObject.SetActive(true);
        OnWeaponActivated?.Invoke(weapon);
    }

    public void DamageBuffPercent(float percent) {
        damageMultiplier += percent;
        foreach(Weapon weapon in ActiveWeapons) {
            weapon.UpdateDamage(damageMultiplier);
        }
    }
}
