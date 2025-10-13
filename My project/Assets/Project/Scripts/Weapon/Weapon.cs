using System;
using System.Collections;
using System.Collections.Generic;
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
    public float size = 1f;
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
    private SpriteRenderer cachedPlayerSpriteRenderer;
    private bool dataApplied = false;

    public event EventHandler<float> OnSkillCooldownUpdate;
    public event EventHandler<int> OnSkillLevelUp;
    #endregion

    // Cache WaitForSeconds to avoid allocating on each coroutine yield
    private static readonly WaitForSeconds s_waitBreathFirst = new WaitForSeconds(0.4f);
    private static readonly WaitForSeconds s_waitBreathSecond = new WaitForSeconds(0.5f);
    private static readonly WaitForSeconds s_waitRaser = new WaitForSeconds(0.7f);

    protected virtual void Awake()
    {
        player = GameManager.Instance.player;
        if (player != null)
            cachedPlayerSpriteRenderer = player.GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        Init().Forget();
    }

    public virtual async UniTask Init()
    {
        // If data was already applied synchronously (from controller), skip expensive async fetch
        if (dataApplied)
        {
            // Ensure runtime state that Init normally sets is present
            damage = baseDamage;
            remainingCooldown = maxCooldown;
            SetPrefabId(_data);
            player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
            return;
        }

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
                // 이전 무기의 제외할 업그레이드 리스트를 확인하여 해당 속성들은 상속하지 않음
                // Avoid allocating HashSet: scan small lists into booleans (cheap since lists are small)
                bool prevExclDamage = false, prevExclProjectiles = false, prevExclCooldown = false,
                    prevExclRange = false, prevExclDuration = false, prevExclPierce = false;

                if (weapon._data.excludeUpgradeList != null)
                {
                    foreach (var upgrade in weapon._data.excludeUpgradeList)
                    {
                        if (upgrade == null) continue;
                        var uname = upgrade.upgradeName;
                        if (uname == UpgradeName.Damage) prevExclDamage = true;
                        else if (uname == UpgradeName.Projectiles) prevExclProjectiles = true;
                        else if (uname == UpgradeName.Cooldown) prevExclCooldown = true;
                        else if (uname == UpgradeName.Range) prevExclRange = true;
                        else if (uname == UpgradeName.Duration) prevExclDuration = true;
                        else if (uname == UpgradeName.PierceLimit) prevExclPierce = true;
                    }
                }

                // 현재 무기의 상속 제외 리스트도 확인
                bool inhExclDamage = false, inhExclProjectiles = false, inhExclCooldown = false,
                    inhExclRange = false, inhExclDuration = false, inhExclPierce = false;

                if (_data.excludeInheritanceList != null)
                {
                    foreach (var upgrade in _data.excludeInheritanceList)
                    {
                        if (upgrade == null) continue;
                        var uname = upgrade.upgradeName;
                        if (uname == UpgradeName.Damage) inhExclDamage = true;
                        else if (uname == UpgradeName.Projectiles) inhExclProjectiles = true;
                        else if (uname == UpgradeName.Cooldown) inhExclCooldown = true;
                        else if (uname == UpgradeName.Range) inhExclRange = true;
                        else if (uname == UpgradeName.Duration) inhExclDuration = true;
                        else if (uname == UpgradeName.PierceLimit) inhExclPierce = true;
                    }
                }

                // 제외되지 않은 속성들만 상속 (업그레이드 제외 + 상속 제외 모두 고려)
                if (!prevExclDamage && !inhExclDamage)
                    baseDamage = weapon._data.itemDataInfo.curDamage;

                if (!prevExclProjectiles && !inhExclProjectiles)
                    count = weapon._data.itemDataInfo.curCount;

                if (!prevExclCooldown && !inhExclCooldown)
                    maxCooldown = weapon._data.itemDataInfo.curCoolDown;

                if (!prevExclRange && !inhExclRange)
                    size = weapon._data.itemDataInfo.curRange;

                if (!prevExclDuration && !inhExclDuration)
                    duration = weapon._data.itemDataInfo.curDuration;

                if (!prevExclPierce && !inhExclPierce)
                    pierce = weapon._data.itemDataInfo.curPierce;

                criticalChancePercent = weapon._data.itemDataInfo.curCriticalChancePercent;
                criticalDamagePercent = weapon._data.itemDataInfo.curCriticalDamagePercent;
                
                GameManager.Instance.weaponController.RemoveWeapon(_data._prevItemData.itemDataInfo.itemId);
            }else
            {
                Debug.Log("진화무기의 이전 무기가 없습니다. 실제 출시할 때 이를 방지해야합니다.");
            }
        }
        if(size == 0)
        {
            size = 1f;
        }

        damage = baseDamage;
        SetPrefabId(_data);
        remainingCooldown = maxCooldown;
        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }

    // Apply saved data synchronously (called by WeaponController during bulk init to avoid per-weapon async work)
    public void ApplySavedData(ItemDataInfo dataInfo)
    {
        if (dataInfo == null) return;
        dataApplied = true;
        level = dataInfo.curLevel;
        baseDamage = dataInfo.curDamage == 0 ? dataInfo.baseDamage : dataInfo.curDamage;
        count = dataInfo.curCount == 0 ? dataInfo.baseCount : dataInfo.curCount;
        maxCooldown = dataInfo.curCoolDown == 0 ? dataInfo.baseCooldown : dataInfo.curCoolDown;
        size = dataInfo.curRange == 0 ? dataInfo.baseRange : dataInfo.curRange;
        duration = dataInfo.curDuration == 0 ? dataInfo.baseDuration : dataInfo.curDuration;
        pierce = dataInfo.curPierce == 0 ? dataInfo.basePierce : dataInfo.curPierce;
        criticalChancePercent = dataInfo.curCriticalChancePercent;
        criticalDamagePercent = dataInfo.curCriticalDamagePercent;

        damage = baseDamage;
        remainingCooldown = maxCooldown;
        SetPrefabId(_data);
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
                count += (int)prevUpgradeValue;
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
        _data.itemDataInfo.curCount = count;
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
        yield return s_waitBreathFirst;
        bullet.localScale = new Vector3(5, 5, 0);
        bullet.position = bullet.position + dir * 4f;
        yield return s_waitBreathSecond;
        bullet.gameObject.SetActive(false);
    }

    private IEnumerator Raser(Transform bullet, Vector3 dir)
    {
        Vector3 originalScale = bullet.localScale;
        yield return s_waitRaser;
        bullet.localScale = new Vector3(5f, 3f, 1);
        bullet.position = transform.position + dir * 8f;
        yield return s_waitRaser;
        bullet.gameObject.SetActive(false);
        bullet.localScale = originalScale;
    }
    #endregion
}