using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public event Action<Weapon> OnWeaponActivated;

    private List<Weapon> weapons = new List<Weapon>();
    
    public IReadOnlyList<Weapon> ActiveWeapons => weapons.Where(w => w.gameObject.activeSelf).ToList();
    public IReadOnlyList<Weapon> Weapons => ActiveWeapons;
    void Start()
    {
        weapons = GetComponentsInChildren<Weapon>().ToList();
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
}
