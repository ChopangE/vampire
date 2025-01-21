using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using Data.WeaponData;
using Cysharp.Threading.Tasks;
using Manager.InGame;
using SO;

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

    public virtual async UniTaskVoid Init(ItemData data = null)
    {
        if (data != null)
        {
            _data = data;
            id = data.itemDataInfo.itemId;
            baseDamage = data.itemDataInfo.baseDamage;
            count = data.itemDataInfo.baseCount;
        }else
        {
            var dataInfo = await DataManager.Instance.GetItemDataInfo(id);
            if(dataInfo == null) {
                Debug.Log($"WeaponId {id}에 해당하는 ItemDataInfo를 찾을 수 없습니다.");
            }else{
                level = dataInfo.curLevel;
                baseDamage = dataInfo.baseDamage;
                count = dataInfo.baseCount;
                maxCooldown = dataInfo.baseCooldown;
                size = dataInfo.baseRange;
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

        switch(prevUpgradeName)
        {
            case UpgradeName.Damage:
                if(damageUpgradeValues != null)
                {
                    damage = baseDamage * (1 + damageUpgradeValues.damagePercent);
                    criticalDamagePercent += damageUpgradeValues.critDamagePercent;
                    criticalChancePercent += damageUpgradeValues.critChancePercent;
                }
                break;
            case UpgradeName.Projectiles:
                count = _data.itemDataInfo.baseCount + (int)prevUpgradeValue;
                break;
            case UpgradeName.PierceLimit:
                break;
            case UpgradeName.Cooldown:
                maxCooldown = _data.itemDataInfo.baseCooldown - prevUpgradeValue;
                break;
            case UpgradeName.Range:
                size = _data.itemDataInfo.baseRange + prevUpgradeValue;
                break;
        }
    }

    public virtual void Attack()
    {
        if (!GameManager.Instance.isLive || !gameObject.activeSelf) return;

        remainingCooldown -= Time.deltaTime;
        if (remainingCooldown <= 0)
        {
            ExecuteAttack();
            remainingCooldown = maxCooldown;
        }

        OnSkillCooldownUpdate?.Invoke(this, remainingCooldown / maxCooldown);
    }

    public virtual void ExecuteAttack() { 
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