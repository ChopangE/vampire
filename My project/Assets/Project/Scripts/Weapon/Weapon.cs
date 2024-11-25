using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using Data.WeaponData;

public class Weapon : MonoBehaviour
{
    #region Properties & Fields
    public WeaponType weaponType;
    public WeaponId id;
    public int prefabId;
    protected float baseDamage;
    public float damage;
    public int count;
    public int level;
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
        Init();
    }

    public virtual void Init(ItemData data = null)
    {
        if (data != null)
        {
            _data = data;
            id = data.itemDataInfo.itemId;
            baseDamage = data.itemDataInfo.baseDamage;
            count = data.itemDataInfo.baseCount;
        }else
        {
            var dataInfo = DataManager.Instance.GetItemDataInfo(id);
            baseDamage = dataInfo.baseDamage;
            count = dataInfo.baseCount;
            maxCooldown = dataInfo.baseCooldown;
        }
        damage = baseDamage;

        SetPrefabId(_data);
        remainingCooldown = maxCooldown;
        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }

    public void LevelUp(float damage, int count)
    {
        level++;
        OnSkillLevelUp?.Invoke(this, level);
        this.damage = damage;
        this.count += count;
    }

    public void Attack()
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