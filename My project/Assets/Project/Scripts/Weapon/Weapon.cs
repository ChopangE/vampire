using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    #region Properties & Fields
    public enum WeaponType { Rotate, Range, Bomb, Raser, Breath }
    public WeaponType weaponType;
    
    public int id;
    public int prefabId;
    public float damage;
    public int count; 
    public float speed;
    public int level;
    public ItemData data;
    
    public float RemainingCooldown { get; private set; }
    public float MaxCooldown => speed;

    private float timer;
    private Player player;
    
    public event EventHandler<float> OnSkillCooldownUpdate;
    public event EventHandler<int> OnSkillLevelUp;
    #endregion

    #region Unity Lifecycle
    private void Awake() 
    {
        player = GameManager.Instance.player;
    }
    #endregion

    #region Public Methods
    public void Init(ItemData data) 
    {
        id = data.itemId;
        damage = data.baseDamage;   
        count = data.baseCount;
        
        SetPrefabId(data);
        InitSetting();
        RemainingCooldown = speed;
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
        if (!GameManager.Instance.isLive) return;
        if (!gameObject.activeSelf) return;

        RemainingCooldown -= Time.deltaTime;
        if (RemainingCooldown <= 0)
        {
            ExecuteAttack();
            RemainingCooldown = speed;
        }

        float progress = RemainingCooldown / speed;
        OnSkillCooldownUpdate?.Invoke(this, progress);
    }
    #endregion

    #region Private Methods
    private void SetPrefabId(ItemData data)
    {
        for(int i = 0; i < GameManager.Instance.pool.prefabs.Length; i++) 
        {
            if(data.projectile == GameManager.Instance.pool.prefabs[i]) 
            {
                prefabId = i;
                break;
            }
        }
    }

    public void InitSetting() 
    {
        speed = id switch
        {
            0 => 5f,  // Wind
            1 => 0.3f,// Fire
            2 => 3f,  // Bomb
            3 => 5f,  // Raser
            4 => 5f,  // Breath
            5 => 7f,  // Clone
            6 => 4f,  // Stick
            _ => speed
        };

        player.BroadcastMessage("ApplayGear", SendMessageOptions.DontRequireReceiver);
    }

    private void ExecuteAttack()
    {
        switch (id)
        {
            case 0: Wind(); break;
            case 1: Fire(); break;
            case 2: Shut(); break;
            case 3: Amulet(); break;
            case 4: Breath(); break;
            case 5: Clone(); break;
            case 6: Stick(); break;
        }
    }
    #endregion

    #region Weapon Specific Attacks
    private void Wind()
    {
        if (!player.scan.nearestTarget) return;
        
        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(damage, 50, dir);
    }

    private void Fire() 
    {
        if (!player.scan.nearestTarget) return;

        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

    private void Shut()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        bullet.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
    }

    private void Amulet() 
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetChild(0).GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
    }

    private void Breath() 
    {
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position + dir * 3f;
    }

    private void Clone() 
    {
        Vector3 dir = GetPlayerDirection();
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
        bullet.GetComponent<Bullet>().Init(damage, 100, dir);
    }

    private void Stick()
    {
        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        float dir = player.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        
        bullet.position = transform.position;
        bullet.localScale = new Vector3(dir, 1, 1);
        DOVirtual.DelayedCall(0.5f, () => bullet.gameObject.SetActive(false));
    }
    #endregion

    #region Utility Methods
    private Vector3 GetPlayerDirection()
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
