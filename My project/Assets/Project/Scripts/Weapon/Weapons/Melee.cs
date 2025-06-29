using Data.WeaponData;
using Data;
using Manager;
using Manager.InGame;
using UnityEngine;
using SO;
using Cysharp.Threading.Tasks;

public class Melee : BulletWeapon
{
    #region Fields
    [Header("Combat Settings")]
    public float coolTime = 1f;
    
    [Header("Attack Properties")]
    public Vector2 boxSize;
    public Transform pos;
    
    [Header("Weapon Objects")]
    public GameObject sword;
    public GameObject stick;

    protected Vector3 dir;
    private GameObject weaponInstance;
    private bool isAttacking;
    protected SpriteRenderer playerSprite;
    private Vector3 originalSwordScale;
    
    #endregion

    #region Unity Lifecycle
    protected virtual void Start()
    {
        InitializeComponents();
        SetupBoxCollider();
        StoreOriginalSwordScale();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos.position + dir, boxSize);
    }
    #endregion

    #region Initialization
    protected void InitializeComponents()
    {
        Player player = GetComponentInParent<Player>();
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    private void SetupBoxCollider()
    {
        if(stick != null)
        {
            boxSize = stick.GetComponent<BoxCollider2D>().size;
        }else if(sword != null)
        {
            boxSize = sword.GetComponent<BoxCollider2D>().size;
        }
    }

    private void StoreOriginalSwordScale()
    {
        if (sword != null)
        {
            originalSwordScale = Vector3.one;
        }
    }

    public override async UniTask Init()
    {
        await base.Init();
        
        // 초기화 완료 후 sword 크기 설정
        UpdateSwordSize();
    }
    #endregion

    #region Level Up Override
    public override void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        base.LevelUp(prevUpgradeName, prevUpgradeValue, damageUpgradeValues);
        
        // Range 업그레이드 시 sword 크기 업데이트
        if (prevUpgradeName == UpgradeName.Range)
        {
            UpdateSwordSize();
        }
    }

    private void UpdateSwordSize()
    {
        if (sword != null)
        {
            // size 값에 비례하여 sword 크기 조정
            float sizeMultiplier = size / _data.itemDataInfo.baseRange;
            sword.transform.localScale = originalSwordScale * sizeMultiplier;
        }
    }
    #endregion

    #region Attack Handlers
    public override void SpawnBullet()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            StartStickAttack();
        
        if (isAttacking)
            UpdateStickAttack();
    }
    #endregion

    #region Attack Execution
    protected virtual void StartStickAttack()
    {
        isAttacking = true;
        dir.x = playerSprite.flipX ? 2f : -2f;
        weaponInstance.transform.localPosition = dir;
        weaponInstance.SetActive(true);
    }

    protected virtual void UpdateStickAttack()
    {
        remainingCooldown -= Time.deltaTime;
        transform.Rotate(Vector3.back * 360 * Time.deltaTime);

        if (remainingCooldown <= 0)
            EndStickAttack();
    }

    protected virtual void EndStickAttack()
    {
        transform.rotation = Quaternion.identity;
        isAttacking = false;
        remainingCooldown = maxCooldown;
        weaponInstance.SetActive(false);
    }
    #endregion

    #region Utility Methods
    public void SetAttackDirection()
    {
        dir = new Vector3(1.0f, 0, 0);
        if (pos.gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            dir *= -1;
        }
    }

    public GameObject SpawnSword()
    {
        GameObject swordInstance = Instantiate(sword, pos.position, Quaternion.identity);
        
        // size 값에 따라 sword 크기 조정
        float sizeMultiplier = size / _data.itemDataInfo.baseRange;
        swordInstance.transform.localScale = originalSwordScale * sizeMultiplier;
        
        return swordInstance;
    }

    public void ConfigureSword(GameObject swordInstance, Transform parent = null, bool followPlayer = false, Vector3 offset = default)
    {
        if(parent == null)
            swordInstance.transform.parent = pos.transform;
        else
            swordInstance.transform.parent = parent;
        
        if(followPlayer)    
        {
            // 플레이어 방향에 따라 크기 조정 (size 값 포함)
            float sizeMultiplier = size / _data.itemDataInfo.baseRange;
            Vector3 adjustedScale = originalSwordScale * sizeMultiplier;
            
            swordInstance.transform.localScale = !playerSprite.flipX 
                ? new Vector3(-adjustedScale.x, adjustedScale.y, adjustedScale.z) 
                : new Vector3(adjustedScale.x, adjustedScale.y, adjustedScale.z);
        }
        else
        {
            // followPlayer가 false일 때도 size 값 적용
            float sizeMultiplier = size / _data.itemDataInfo.baseRange;
            swordInstance.transform.localScale = originalSwordScale * sizeMultiplier;
        }

        if(offset != default)
            swordInstance.transform.localPosition = offset;
        else
            swordInstance.transform.localPosition = dir;

        Destroy(swordInstance, duration);
    }
    #endregion
}
