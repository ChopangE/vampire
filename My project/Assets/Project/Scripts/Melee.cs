using UnityEngine;

public class Melee : MonoBehaviour
{
    #region Fields
    [Header("Combat Settings")]
    public int id;
    public float damage = 3f;
    public float coolTime = 1f;
    
    [Header("Attack Properties")]
    public Vector2 boxSize;
    public Transform pos;
    
    [Header("Weapon Objects")]
    public GameObject sword;
    public GameObject stick;

    private float curTime;
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

    private void Update()
    {
        if (!GameManager.Instance.isLive) return;

        if (id == 1)
            HandleSwordAttack();
        else
            HandleStickAttack();
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
    private void HandleSwordAttack()
    {
        if (curTime <= 0)
        {
            ExecuteSwordAttack();
            curTime = coolTime;
        }
        curTime -= Time.deltaTime;
    }

    private void HandleStickAttack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
            StartStickAttack();
        
        if (isAttacking)
            UpdateStickAttack();
    }
    #endregion

    #region Attack Execution
    private void ExecuteSwordAttack()
    {
        SetAttackDirection();
        GameObject sword = SpawnSword();
        ConfigureSword(sword);
    }

    private void StartStickAttack()
    {
        isAttacking = true;
        dir.x = playerSprite.flipX ? 2f : -2f;
        weaponInstance.transform.localPosition = dir;
        weaponInstance.SetActive(true);
    }

    private void UpdateStickAttack()
    {
        curTime += Time.deltaTime;
        transform.Rotate(Vector3.back * 360 * Time.deltaTime);

        if (curTime >= 1f)
            EndStickAttack();
    }

    private void EndStickAttack()
    {
        transform.rotation = Quaternion.identity;
        isAttacking = false;
        curTime = 0f;
        weaponInstance.SetActive(false);
    }
    #endregion

    #region Utility Methods
    private void SetAttackDirection()
    {
        dir = new Vector3(1.0f, 0, 0);
        if (pos.gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            dir *= -1;
        }
    }

    private GameObject SpawnSword()
    {
        return Instantiate(sword, pos.position + dir, Quaternion.identity);
    }

    private void ConfigureSword(GameObject swordInstance)
    {
        swordInstance.transform.parent = pos.transform;
        swordInstance.transform.localScale = !playerSprite.flipX 
            ? new Vector3(-1, 1, 1) 
            : new Vector3(1, 1, 1);
        Destroy(swordInstance, 0.5f);
    }
    #endregion
}
