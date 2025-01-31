using UnityEngine;
using Cysharp.Threading.Tasks;

public class BowlingBullet : Bullet
{
    [SerializeField] private float stunRadius = 1.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHeight = 3f;
    [SerializeField] private float rockDuration = 2f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float journeyLength;
    
    public override void Init(float damage, int per, Vector3 dir, bool canStun = false,
    float duration = 0f, float criticalDamagePercent = 0f, float criticalChancePercent = 0f)
    {
        base.Init(damage, per, dir, canStun, duration, criticalDamagePercent, criticalChancePercent);
        startPos = transform.position;
        InitializeBullet().Forget();
    }

    private async UniTaskVoid InitializeBullet()
    {
        // 1프레임 대기하여 위치가 완전히 지정되도록 함
        await UniTask.Yield();
        
        startPos = transform.position;
        // 7x7 영역 밖의 랜덤 위치 지정 (각 방향으로 ±7 이상)
        float randomX = (Random.Range(0, 2) == 0 ? Random.Range(-7f, -7f) : Random.Range(7f, 7f)) + startPos.x;
        float randomY = (Random.Range(0, 2) == 0 ? Random.Range(-7f, -7f) : Random.Range(7f, 7f)) + startPos.y;
        targetPos = new Vector3(randomX, randomY, transform.position.z);
        journeyLength = Vector3.Distance(startPos, targetPos);
        
        MoveInParabola().Forget();
    }

    private async UniTaskVoid MoveInParabola()
    {
        float elapsedTime = 0f;
        float duration = journeyLength / moveSpeed;
        Vector3 originalScale = Vector3.one;
        Vector3 enlargedScale = Vector3.one * 1.5f;
        
        // 시작할 때는 원래 크기로 시작
        transform.localScale = originalScale;
        
        while (elapsedTime < duration)
        {
            float percentComplete = elapsedTime / duration;
            
            // 포물선 운동 계산
            float parabolicHeight = -4 * maxHeight * (percentComplete * percentComplete - percentComplete);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, percentComplete);
            currentPos.z = -parabolicHeight;
            
            // 처음 20% 구간에서 크기를 천천히 1.5배로 증가
            if (percentComplete < 0.2f)
            {
                float scalePercent = percentComplete / 0.2f; // 0~1 값으로 정규화
                transform.localScale = Vector3.Lerp(originalScale, enlargedScale, scalePercent);
            }
            // 도착 지점 가까이에서 크기를 원래대로 부드럽게 변경 (마지막 20% 구간에서)
            else if (percentComplete > 0.8f)
            {
                float scalePercent = (percentComplete - 0.8f) / 0.2f; // 0~1 값으로 정규화
                transform.localScale = Vector3.Lerp(enlargedScale, originalScale, scalePercent);
            }
            
            transform.position = currentPos;
            
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        
        // 도착 지점에서 정확한 위치와 크기 설정
        transform.position = targetPos;
        transform.localScale = originalScale;
        
        // radius 변수를 사용하여 범위 내의 적들에게 기절 효과
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, stunRadius);
        foreach (Collider2D collision in colliders)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    ApplyStun(enemy, 2f).Forget();
                }
            }
        }
        
        await UniTask.Delay(System.TimeSpan.FromSeconds(rockDuration));
        gameObject.SetActive(false);
    }
}

