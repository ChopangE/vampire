using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using Data.WeaponData;
using Cysharp.Threading.Tasks;
using Manager.InGame;
using SO;
using Unity.VisualScripting;
using Manager;

public class Weapon : MonoBehaviour
{
    #region Properties & Fields
    public WeaponType weaponType;
    public WeaponId id;
    public int prefabId;
    protected float baseDamage;
    public float damage;
    public float criticalDamagePercent;
    public float criticalChancePercent;
    public float duration;
    public int pierce;
    public int count;
    public float size;
    private int _level;
    public int level
    {
        get => _level;
        set
        {
            if (value > _level)
            {
                OnSkillLevelUp?.Invoke(this, value);
            }
            _level = value;
        }
    }
    public ItemData _data;

    public float remainingCooldown { get; protected set; }
    public float maxCooldown;

    protected float timer;
    protected Player player;

    public event EventHandler<float> OnSkillCooldownUpdate;
    public event EventHandler<int> OnSkillLevelUp;
    #endregion

    protected virtual void Awake()
    {
        player = GameManager.Instance.player;
    }
    void OnEnable()
    {
        Init().Forget();
    }

    public virtual async UniTask Init()
    {
        // DataManager가 초기화될 때까지 대기
        while (Global.DataManager == null)
        {
            await UniTask.Yield();
        }

        var dataInfo = await Global.DataManager.GetItemDataInfo(id);
        if (dataInfo == null)
        {
            Debug.Log($"WeaponId {id}에 해당하는 ItemDataInfo를 찾을 수 없습니다.");
        }
        else
        {
            level = dataInfo.curLevel;
            baseDamage = dataInfo.curDamage == 0 ? dataInfo.baseDamage : dataInfo.curDamage;
            count = dataInfo.curCount == 0 ? dataInfo.baseCount : dataInfo.curCount;
            maxCooldown = dataInfo.curCoolDown == 0 ? dataInfo.baseCooldown : dataInfo.curCoolDown;
            size = dataInfo.curRange == 0 ? dataInfo.baseRange : dataInfo.curRange;
            duration = dataInfo.curDuration == 0 ? dataInfo.baseDuration : dataInfo.curDuration;
            pierce = dataInfo.curPierce == 0 ? dataInfo.basePierce : dataInfo.curPierce;
            criticalChancePercent = dataInfo.curCriticalChancePercent;
            criticalDamagePercent = dataInfo.curCriticalDamagePercent;

        }


        if (_data.isEvaluateWeapon)
        {
            var weapon = GameManager.Instance.weaponController.GetEvaluateWeaponValue(this);
            if (weapon != null)
            {
                baseDamage = weapon._data.itemDataInfo.curDamage;
                count = weapon._data.itemDataInfo.curCount;
                maxCooldown = weapon._data.itemDataInfo.curCoolDown;
                size = weapon._data.itemDataInfo.curRange;
                duration = weapon._data.itemDataInfo.curDuration;
                pierce = weapon._data.itemDataInfo.curPierce;
                criticalChancePercent = weapon._data.itemDataInfo.curCriticalChancePercent;
                criticalDamagePercent = weapon._data.itemDataInfo.curCriticalDamagePercent;
                GameManager.Instance.weaponController.RemoveWeapon(_data._prevItemData.itemDataInfo.itemId);
            }else
            {
                Debug.Log("진화무기의 이전 무기가 없습니다. 실제 출시할 때 이를 방지해야합니다.");
            }
        }

        damage = baseDamage;
        SetPrefabId(_data);
        remainingCooldown = maxCooldown;
        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        level++;

        switch (prevUpgradeName)
        {
            case UpgradeName.Damage:
                if (damageUpgradeValues != null)
                {
                    damage += baseDamage * (1 + damageUpgradeValues.damagePercent) - baseDamage;
                    criticalDamagePercent += damageUpgradeValues.critDamagePercent;
                    criticalChancePercent += damageUpgradeValues.critChancePercent;
                }
                break;
            case UpgradeName.Projectiles:
                count += _data.itemDataInfo.baseCount + (int)prevUpgradeValue - _data.itemDataInfo.baseCount;
                break;
            case UpgradeName.PierceLimit:
                pierce += (int)prevUpgradeValue;
                break;
            case UpgradeName.Cooldown:
                maxCooldown -= prevUpgradeValue;
                break;
            case UpgradeName.Range:
                size += prevUpgradeValue;
                break;
            case UpgradeName.Duration:
                duration += prevUpgradeValue;
                break;
        }
        _data.itemDataInfo.curDuration = duration;
        _data.itemDataInfo.curCoolDown = maxCooldown;
        _data.itemDataInfo.curRange = size;
        _data.itemDataInfo.curDamage = damage;
        _data.itemDataInfo.curPierce = pierce;
        _data.itemDataInfo.curCriticalDamagePercent = criticalDamagePercent;
        _data.itemDataInfo.curCriticalChancePercent = criticalChancePercent;
        Global.DataManager.SaveWeaponData(_data);
    }

    public virtual void Attack()
    {
        if (!GameManager.Instance.isLive || !gameObject.activeSelf || maxCooldown <= 0) return;

        remainingCooldown -= Time.deltaTime;
        if (remainingCooldown <= 0)
        {
            ExecuteAttack();
            remainingCooldown = maxCooldown;
        }

        OnSkillCooldownUpdate?.Invoke(this, remainingCooldown / maxCooldown);
    }

    public virtual void ExecuteAttack()
    {
    }

    #region Private Methods
    private void SetPrefabId(ItemData data)
    {
        for (int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.Instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }
    }

    public void UpdateDamage(float damageMultiplier)
    {
        damage = baseDamage * damageMultiplier;
    }
    #endregion

    #region Utility Methods
    protected Vector3 GetPlayerDirection()
    {
        Vector3 dir = player.inputVec;
        if (dir.magnitude < 0.1f)
        {
            dir = player.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;
        }
        return dir;
    }

    private IEnumerator BreathDown(Transform bullet, Vector3 dir)
    {
        yield return new WaitForSeconds(0.4f);
        bullet.localScale = new Vector3(5, 5, 0);
        bullet.position = bullet.position + dir * 4f;
        yield return new WaitForSeconds(0.5f);
        bullet.gameObject.SetActive(false);
    }

    private IEnumerator Raser(Transform bullet, Vector3 dir)
    {
        Vector3 originalScale = bullet.localScale;
        yield return new WaitForSeconds(0.7f);
        bullet.localScale = new Vector3(5f, 3f, 1);
        bullet.position = transform.position + dir * 8f;
        yield return new WaitForSeconds(0.7f);
        bullet.gameObject.SetActive(false);
        bullet.localScale = originalScale;
    }
    #endregion
}