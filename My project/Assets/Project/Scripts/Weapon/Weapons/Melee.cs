using Data.WeaponData;
using UnityEngine;

public class Melee : Weapon
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

    private Vector3 dir;
    private GameObject weaponInstance;
    private bool isAttacking;
    private SpriteRenderer playerSprite;
    #endregion

    #region Unity Lifecycle
    private void Start()
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
    private void InitializeComponents()
    {
        Player player = GetComponentInParent<Player>();
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    private void SetupBoxCollider()
    {
        boxSize = stick.GetComponent<BoxCollider2D>().size;
    }
    #endregion

    #region Attack Handlers
    public override void ExecuteAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
            StartStickAttack();
        
        if (isAttacking)
            UpdateStickAttack();
    }
    #endregion

    #region Attack Execution
    private void StartStickAttack()
    {
        isAttacking = true;
        dir.x = playerSprite.flipX ? 2f : -2f;
        weaponInstance.transform.localPosition = dir;
        weaponInstance.SetActive(true);
    }

    private void UpdateStickAttack()
    {
        remainingCooldown -= Time.deltaTime;
        transform.Rotate(Vector3.back * 360 * Time.deltaTime);

        if (remainingCooldown <= 0)
            EndStickAttack();
    }

    private void EndStickAttack()
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
        return Instantiate(sword, pos.position + dir, Quaternion.identity);
    }

    public void ConfigureSword(GameObject swordInstance)
    {
        swordInstance.transform.parent = pos.transform;
        swordInstance.transform.localScale = !playerSprite.flipX 
            ? new Vector3(-1, 1, 1) 
            : new Vector3(1, 1, 1);
        Destroy(swordInstance, 0.5f);
    }
    #endregion
}
