using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;


public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Rigidbody2D rigid;
    public SpriteRenderer sprite;
    Animator anim;
    public Scanner scan;
    public float speed;
    public float damageBonus = 0f;
    public float baseSpeed = 3f;
    bool isKnockBack { get; set; }
    public bool IsFacingRight => sprite.flipX;

    public SpriteRenderer currentStatus;
    public Sprite stunSprite;
    public Sprite slowSprite;
    public Sprite burnSprite;
    public Sprite poisonSprite;
    public Sprite bleedSprite;
    
    // 상태이상 관련
    public bool isStunned { get; private set; }
    public bool isSlowed { get; private set; }
    public bool isBurning { get; private set; }
    public bool isPoisoned { get; private set; }
    public bool isBleeding { get; private set; }
    private float currentSpeedMultiplier = 1f;


    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scan = GetComponent<Scanner>();
        isKnockBack = false;
        
        // 게임 시작 시 상태 이상 스프라이트 초기화
        UpdateStatusSprite();
    }

    

    void FixedUpdate() {
        if (!GameManager.Instance.isLive) return;
        if (isKnockBack || isStunned) return;
        
        float actualSpeed = speed * currentSpeedMultiplier;
        Vector2 movePosition = rigid.position + inputVec * actualSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(movePosition);
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate() {
        if (!GameManager.Instance.isLive) return;

        anim.SetFloat("Speed", inputVec.magnitude);
        
        if(inputVec.x != 0) {
            sprite.flipX = inputVec.x < 0 ? true : false; 
        }
        
    }
    
    
    
    public void Stopping()
    {
        isKnockBack = true;
        StoppingAsync().Forget();
    }

    async UniTaskVoid StoppingAsync()
    {
        await UniTask.Delay(800); // 0.8초를 밀리초로 변환
        rigid.velocity = Vector2.zero;
        isKnockBack = false;
    }
    
    // 상태이상 적용 메서드들
    public void ApplyStun(float duration)
    {
        if (!isStunned)
        {
            StunAsync(duration).Forget();
        }
    }
    
    public void ApplySlow(float duration, float slowPercent)
    {
        if (!isSlowed)
        {
            SlowAsync(duration, slowPercent).Forget();
        }
    }
    
    public void ApplyBurn(float duration, float damagePercent)
    {
        if (!isBurning)
        {
            BurnAsync(duration, damagePercent).Forget();
        }
    }
    
    public void ApplyPoison(float tickInterval, float damagePercent)
    {
        if (!isPoisoned)
        {
            PoisonAsync(tickInterval, damagePercent).Forget();
        }
    }
    
    public void ApplyBleed(float statusDuration, float damageDuration, float damagePercent)
    {
        if (!isBleeding)
        {
            BleedAsync(statusDuration, damageDuration, damagePercent).Forget();
        }
    }
    
    async UniTaskVoid StunAsync(float duration)
    {
        isStunned = true;
        UpdateStatusSprite();
        rigid.velocity = Vector2.zero;
        await UniTask.Delay((int)(duration * 1000)); // 초를 밀리초로 변환
        isStunned = false;
        UpdateStatusSprite();
    }
    
    async UniTaskVoid SlowAsync(float duration, float slowPercent)
    {
        isSlowed = true;
        currentSpeedMultiplier = 1f - slowPercent;
        UpdateStatusSprite();
        await UniTask.Delay((int)(duration * 1000)); // 초를 밀리초로 변환
        isSlowed = false;
        currentSpeedMultiplier = 1f;
        UpdateStatusSprite();
    }
    
    async UniTaskVoid BurnAsync(float duration, float damagePercent)
    {
        isBurning = true;
        UpdateStatusSprite();
        float tickInterval = 1f; // 1초마다 데미지
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            await UniTask.Delay((int)(tickInterval * 1000)); // 초를 밀리초로 변환
            elapsed += tickInterval;
            
            var damage = GameManager.Instance.maxHealth * damagePercent;
            GameManager.Instance.Health -= damage;
        }
        
        isBurning = false;
        UpdateStatusSprite();
    }
    
    async UniTaskVoid PoisonAsync(float tickInterval, float damagePercent)
    {
        isPoisoned = true;
        UpdateStatusSprite();
        
        await UniTask.Delay((int)(tickInterval * 1000)); // 첫 번째 틱까지 대기
        
        if (isPoisoned) // 여전히 중독 상태인지 확인
        {
            var damage = GameManager.Instance.maxHealth * damagePercent;
            GameManager.Instance.Health -= damage;
        }
        
        isPoisoned = false;
        UpdateStatusSprite();
    }
    
    public void StopPoison()
    {
        isPoisoned = false;
        UpdateStatusSprite();
    }
    
    async UniTaskVoid BleedAsync(float statusDuration, float damageDuration, float damagePercent)
    {
        isBleeding = true;
        UpdateStatusSprite();
        
        float elapsed = 0f;
        float tickInterval = 1f; // 1초마다 데미지
        
        while (elapsed < damageDuration && isBleeding)
        {
            await UniTask.Delay((int)(tickInterval * 1000)); // 1초 대기
            elapsed += tickInterval;
            
            if (isBleeding) // 여전히 출혈 상태인지 확인
            {
                var damage = GameManager.Instance.maxHealth * damagePercent;
                GameManager.Instance.Health -= damage;
            }
        }
        
        // 데미지는 끝났지만 출혈 상태는 더 지속 (statusDuration - damageDuration)
        if (isBleeding && statusDuration > damageDuration)
        {
            float remainingDuration = statusDuration - damageDuration;
            await UniTask.Delay((int)(remainingDuration * 1000));
        }
        
        isBleeding = false;
        UpdateStatusSprite();
    }
    
    void UpdateStatusSprite()
    {
        if (currentStatus == null) return;
        
        // 상태 이상이 하나라도 있는지 확인
        bool hasStatusEffect = isStunned || isBleeding || isPoisoned || isBurning || isSlowed;
        
        if (!hasStatusEffect)
        {
            currentStatus.gameObject.SetActive(false);
            return;
        }
        
        // 우선순위: 스턴 > 출혈 > 중독 > 화상 > 슬로우
        if (isStunned)
        {
            currentStatus.sprite = stunSprite;
            currentStatus.gameObject.SetActive(true);
        }
        else if (isBleeding)
        {
            currentStatus.sprite = bleedSprite;
            currentStatus.gameObject.SetActive(true);
        }
        else if (isPoisoned)
        {
            currentStatus.sprite = poisonSprite;
            currentStatus.gameObject.SetActive(true);
        }
        else if (isBurning)
        {
            currentStatus.sprite = burnSprite;
            currentStatus.gameObject.SetActive(true);
        }
        else if (isSlowed)
        {
            currentStatus.sprite = slowSprite;
            currentStatus.gameObject.SetActive(true);
        }
    }
    
}
