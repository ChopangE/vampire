using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Manager.InGame;
using SO;

public enum BlessingType
{
    WindShield,      // 실드
    Lightning,       // 번개
    WindSpeed,       // 이속
    Rain            // 비
}

public class PetWeapon : Weapon
{
    [LabelText("외곽선")]
    [SerializeField] private OutlineSprite outlineSprite;
    
    [Header("기본 설정")]
    [LabelText("속도")] public float speed;
    
    [Header("축복 설정")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject rainPrefab;
    [SerializeField] private float blessingDuration = 30f;
    private BlessingType currentBlessing;
    private CancellationTokenSource _blessingCts = new CancellationTokenSource();
    
    private readonly Color32 shieldColor = new Color32(195, 212, 66, 255);    // #c3d442
    private readonly Color32 lightningColor = new Color32(243, 208, 64, 255); // #f3d040
    private readonly Color32 speedColor = new Color32(69, 161, 222, 255);     // #45a1de
    private readonly Color32 cloneColor = new Color32(242, 242, 240, 255);    // #f2f2f0
    private readonly Color32 rainColor = new Color32(185, 181, 195, 255);     // #b9b5c3

    private bool isBuffActive = false;

    void Start()
    {
        speed = 1f;
        if(level >= 3) {
            StartBlessingCycle().Forget();
        }
    }

    async UniTaskVoid StartBlessingCycle()
    {
        if (_blessingCts != null)
        {
            _blessingCts.Cancel();
            _blessingCts.Dispose();
        }
        _blessingCts = new CancellationTokenSource();

        try
        {
            while (!_blessingCts.Token.IsCancellationRequested)
            {
                currentBlessing = (BlessingType)UnityEngine.Random.Range(0, 5);
                await ApplyBlessing(_blessingCts.Token);
                
                await UniTask.Delay(TimeSpan.FromSeconds(blessingDuration), cancellationToken: _blessingCts.Token);
                RemoveCurrentBlessing();
            }
        }
        catch (OperationCanceledException)
        {
            // Blessing cycle was cancelled
        }
    }

    async UniTask ApplyBlessing(CancellationToken cancellationToken)
    {
        switch (currentBlessing)
        {
            case BlessingType.WindShield:
                ApplyShieldBlessing();
                break;
            case BlessingType.Lightning:
                await ApplyLightningBlessing(cancellationToken);
                break;
            case BlessingType.WindSpeed:
                ApplySpeedBlessing();
                break;
            case BlessingType.Rain:
                await ApplyRainBlessing(cancellationToken);
                break;
        }
        UpdateVisualEffect();
    }

    void ApplyShieldBlessing()
    {
        float shieldAmount = GameManager.Instance.maxHealth * 0.5f;
        GameManager.Instance.Shield = shieldAmount;
    }

    async UniTask ApplyLightningBlessing(CancellationToken cancellationToken)
    {
        float endTime = Time.time + blessingDuration;
        while (Time.time < endTime && !cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < 5 && !cancellationToken.IsCancellationRequested; i++)
            {
                Vector3 randomPosition = GetRandomPositionInScreen();
                SpawnLightning(randomPosition);
                await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: cancellationToken);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
        }
    }

    void ApplySpeedBlessing()
    {
        player.speed *= 1.1f; // 10% 증가
    }

    void ApplyCloneBlessing()
    {
        // 이전 버프가 적용되어 있다면 먼저 제거
        if (isBuffActive)
        {
            GameManager.Instance.weaponController.DamageBuffPercent(-0.1f);
        }
        
        // 새로운 버프 적용
        GameManager.Instance.weaponController.DamageBuffPercent(0.1f);
        isBuffActive = true;
    }

    async UniTask ApplyRainBlessing(CancellationToken cancellationToken)
    {
        float endTime = Time.time + blessingDuration;
        while (Time.time < endTime && !cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < 5 && !cancellationToken.IsCancellationRequested; i++)
            {
                Vector3 randomPosition = GetRandomPositionInScreen();
                SpawnRain(randomPosition);
                await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: cancellationToken);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
        }
    }

    void RemoveCurrentBlessing()
    {
        switch (currentBlessing)
        {
            case BlessingType.WindSpeed:
                player.speed /= 1.1f;
                break;
        }
    }


    Vector3 GetRandomPositionInScreen()
    {
        Camera mainCamera = Camera.main;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        
        float randomX = UnityEngine.Random.Range(-width/2, width/2);
        float randomY = UnityEngine.Random.Range(-height/2, height/2);
        var mainPos = mainCamera.transform.position;
        mainPos = new Vector3(mainPos.x, mainPos.y, 0);
        return mainPos + new Vector3(randomX, randomY, 0);
    }

    void SpawnLightning(Vector3 position)
    {
        Bullet lightning = Instantiate(lightningPrefab, position, Quaternion.identity).GetComponent<Bullet>();
        lightning.Init(damage, -1, Vector3.zero, true, 5, criticalDamagePercent, criticalChancePercent);
        // 번개 효과 설정
        // 데미지 = 몹 체력의 50%, 1초 스턴
    }

    void SpawnRain(Vector3 position)
    {
        Bullet rain = Instantiate(rainPrefab, position, Quaternion.identity).GetComponent<Bullet>();
        rain.Init(damage, -1, Vector3.zero, true, 5, criticalDamagePercent, criticalChancePercent);
        // 비 효과 설정
        // 이속 30% 감소, 데미지 = 몹 체력의 10%
    }

    void UpdateVisualEffect()
    {
        Color32 newColor = currentBlessing switch
        {
            BlessingType.WindShield => shieldColor,
            BlessingType.Lightning => lightningColor,
            BlessingType.WindSpeed => speedColor,
            BlessingType.Rain => rainColor,
            _ => Color.white
        };
        
        outlineSprite.SetColor(newColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isLive) return;

        timer += Time.deltaTime;
        if (timer > speed)
        {
            timer = 0f;
            Fire();
        }
    }
    void Fire() {
        if (!player.scan.nearestTarget) {
            return;
        }

        Vector3 targetPos = player.scan.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        // Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        // bullet.position = transform.position;
        // bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // bullet.GetComponent<Bullet>().Init(damage, 0, dir);
    }

    private void OnDestroy()
    {
        if (_blessingCts != null)
        {
            _blessingCts.Cancel();
            _blessingCts.Dispose();
            _blessingCts = null;
        }
    }

    public override void LevelUp(UpgradeName prevUpgradeName, float prevUpgradeValue, DamageUpgradeValues damageUpgradeValues = null)
    {
        base.LevelUp(prevUpgradeName, prevUpgradeValue, damageUpgradeValues);
        
        // 레벨 3 이상일 때 스킬 활성화
        if (level == 3)
        {
            Debug.Log("펫무기 활성화");
            StartBlessingCycle().Forget();
        }
    }
}
