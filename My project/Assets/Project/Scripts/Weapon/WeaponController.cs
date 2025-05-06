using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.WeaponData;
using Manager;
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
        
        ActivateOwnedWeapons().Forget();
    }


    void Update()
    {
        foreach (Weapon weapon in ActiveWeapons)
        {
            weapon.Attack();
        }
    }
    private async UniTask ActivateOwnedWeapons()
    {
        await UniTask.WaitUntil(() => Global.DataManager.isLoaded);
        var weaponSavedDatas = Global.DataManager.GetItemDataInfos();
        // 일단 모든 무기를 활성화
        foreach(var weaponSavedData in weaponSavedDatas)
        {
            var weapon = weapons.FirstOrDefault(w => w.id == weaponSavedData.itemId);
            if(weapon != null)
            {
                if(weaponSavedData.curLevel > 0)
                {
                    ActivateWeapon(weapon);
                }
            }
        }
        
        // 진화무기가 활성화된 경우 이전 무기 비활성화
        foreach(var weapon in ActiveWeapons.ToList())
        {
            if(weapon._data.isEvaluateWeapon && weapon._data._prevItemData != null)
            {
                var prevWeaponId = weapon._data._prevItemData.itemDataInfo.itemId;
                var prevWeapon = ActiveWeapons.FirstOrDefault(w => w.id == prevWeaponId);
                if(prevWeapon != null)
                {
                    RemoveWeapon(prevWeapon);
                }
            }
            else if(!weapon._data.isEvaluateWeapon && weapon._data._nextItemData != null)
            {
                if(ActiveWeapons.FirstOrDefault(w => w.id == weapon._data._nextItemData.itemDataInfo.itemId) != null)
                {
                    RemoveWeapon(weapon);
                }
            }
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

    public void RemoveWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        
        if(weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
        }
        OnWeaponActivated?.Invoke(weapon);
    }
    public void RemoveWeapon(WeaponId id)
    {
        var weapon = weapons.FirstOrDefault(w => w.id == id);
        if(weapon != null)
        {
            RemoveWeapon(weapon);
        }
    }

    public void DamageBuffPercent(float percent) {
        damageMultiplier += percent;
        foreach(Weapon weapon in ActiveWeapons) {
            weapon.UpdateDamage(damageMultiplier);
        }
    }

    public Weapon GetEvaluateWeaponValue(Weapon weapon)
    {
        Weapon evaluatePrevWeapon = null;
        if(weapon._data._prevItemData != null)
        {
            evaluatePrevWeapon = ActiveWeapons.FirstOrDefault(w => w.id == weapon._data._prevItemData.itemDataInfo.itemId);
        }
        return evaluatePrevWeapon;
    }
}
