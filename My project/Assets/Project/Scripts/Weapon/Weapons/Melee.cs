using Data.WeaponData;
using UnityEngine;

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
    
    #endregion

    #region Unity Lifecycle
    protected virtual void Start()
    {
        InitializeComponents();
        SetupBoxCollider();
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
        return Instantiate(sword, pos.position, Quaternion.identity);
    }

    public void ConfigureSword(GameObject swordInstance, Transform parent = null, bool followPlayer = false, Vector3 offset = default)
    {
        if(parent == null)
            swordInstance.transform.parent = pos.transform;
        else
            swordInstance.transform.parent = parent;
        
        if(followPlayer)    
        {
            swordInstance.transform.localScale = !playerSprite.flipX 
                ? new Vector3(-1, 1, 1) 
                : new Vector3(1, 1, 1);
        }

        if(offset != default)
            swordInstance.transform.localPosition = offset;
        else
            swordInstance.transform.localPosition = dir;

        Destroy(swordInstance, duration);
    }
    #endregion
}
