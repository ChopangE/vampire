using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public event Action<Weapon> OnWeaponActivated;

    private List<Weapon> weapons = new List<Weapon>();
    
    public int maxActiveWeaponCount = 6;
    public IReadOnlyList<Weapon> ActiveWeapons => weapons.Where(w => w.gameObject.activeSelf).ToList();
    public IReadOnlyList<Weapon> Weapons => weapons;
    private float damageMultiplier = 1f;

    void Start()
    {
        weapons = GetComponentsInChildren<Weapon>(true).ToList();
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
