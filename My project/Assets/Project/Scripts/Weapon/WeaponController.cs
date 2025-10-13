using System;
using System.Collections.Generic;
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
    // Reused cache of active weapons to avoid allocating lists every access.
    private readonly List<Weapon> activeWeaponsCache = new List<Weapon>(8);
    public IReadOnlyList<Weapon> ActiveWeapons => activeWeaponsCache;
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

        // Use a HashSet to avoid duplicates without LINQ allocations
        var set = new HashSet<Weapon>(ownWeapons.Length + (additionalWeaponSources?.Count ?? 0) * 4);

        foreach (var w in ownWeapons)
            if (w != null) set.Add(w);

        if (additionalWeaponSources != null)
        {
            for (int i = 0; i < additionalWeaponSources.Count; i++)
            {
                var src = additionalWeaponSources[i];
                if (src == null) continue;
                var arr = src.GetComponentsInChildren<Weapon>(true);
                for (int j = 0; j < arr.Length; j++)
                {
                    var w = arr[j];
                    if (w != null) set.Add(w);
                }
            }
        }

        weapons = new List<Weapon>(set.Count);
        weapons.AddRange(set);

        RefreshActiveWeaponsCache();

        ActivateOwnedWeapons().Forget();
    }


    void Update()
    {
        // iterate existing weapons and call Attack on active ones to avoid allocating ActiveWeapons each frame
        for (int i = 0; i < weapons.Count; i++)
        {
            var weapon = weapons[i];
            if (weapon != null && weapon.gameObject.activeSelf)
                weapon.Attack();
        }
    }
    private async UniTask ActivateOwnedWeapons()
    {
        await UniTask.WaitUntil(() => Global.DataManager.isLoaded);
        var weaponSavedDatas = Global.DataManager.GetItemDataInfos();
        // 일단 모든 무기를 활성화
        for (int i = 0; i < weaponSavedDatas.Length; i++)
        {
            var weaponSavedData = weaponSavedDatas[i];
            Weapon found = null;
            for (int j = 0; j < weapons.Count; j++)
            {
                var w = weapons[j];
                if (w != null && w.id == weaponSavedData.itemId)
                {
                    found = w;
                    break;
                }
            }

            if (found != null)
            {
                // Apply saved data synchronously to avoid each Weapon doing an async fetch in Init
                found.ApplySavedData(weaponSavedData);

                if (weaponSavedData.curLevel > 0)
                    ActivateWeapon(found);
            }
        }

        // Refresh cache before doing evolution checks
        RefreshActiveWeaponsCache();

        // 진화무기가 활성화된 경우 이전 무기 비활성화
        for (int i = 0; i < weapons.Count; i++)
        {
            var weapon = weapons[i];
            if (weapon == null || !weapon.gameObject.activeSelf) continue;

            if (weapon._data.isEvaluateWeapon && weapon._data._prevItemData != null)
            {
                var prevWeaponId = weapon._data._prevItemData.itemDataInfo.itemId;
                var prevWeapon = FindActiveWeaponById(prevWeaponId);
                if (prevWeapon != null)
                    RemoveWeapon(prevWeapon);
            }
            else if (!weapon._data.isEvaluateWeapon && weapon._data._nextItemData != null)
            {
                var nextId = weapon._data._nextItemData.itemDataInfo.itemId;
                if (FindActiveWeaponById(nextId) != null)
                    RemoveWeapon(weapon);
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
        RefreshActiveWeaponsCache();
        OnWeaponActivated?.Invoke(weapon);
    }

    public void RemoveWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
        }

        RefreshActiveWeaponsCache();
        OnWeaponActivated?.Invoke(weapon);
    }
    public void RemoveWeapon(WeaponId id)
    {
        Weapon found = null;
        for (int i = 0; i < weapons.Count; i++)
        {
            var w = weapons[i];
            if (w != null && w.id == id)
            {
                found = w;
                break;
            }
        }

        if (found != null)
            RemoveWeapon(found);
    }

    public void DamageBuffPercent(float percent) {
        damageMultiplier += percent;
        // Update active weapons without allocations
        for (int i = 0; i < weapons.Count; i++)
        {
            var w = weapons[i];
            if (w != null && w.gameObject.activeSelf)
                w.UpdateDamage(damageMultiplier);
        }
    }

    public Weapon GetEvaluateWeaponValue(Weapon weapon)
    {
        Weapon evaluatePrevWeapon = null;
        if (weapon._data._prevItemData != null)
        {
            var prevId = weapon._data._prevItemData.itemDataInfo.itemId;
            evaluatePrevWeapon = FindActiveWeaponById(prevId);
        }
        return evaluatePrevWeapon;
    }

    // Helper: refresh reusable active weapons cache
    private void RefreshActiveWeaponsCache()
    {
        activeWeaponsCache.Clear();
        for (int i = 0; i < weapons.Count; i++)
        {
            var w = weapons[i];
            if (w != null && w.gameObject.activeSelf)
                activeWeaponsCache.Add(w);
        }
    }

    private Weapon FindActiveWeaponById(WeaponId id)
    {
        for (int i = 0; i < activeWeaponsCache.Count; i++)
        {
            var w = activeWeaponsCache[i];
            if (w != null && w.id == id) return w;
        }
        return null;
    }
}
