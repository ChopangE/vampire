using System.Collections;
using System.Collections.Generic;
using InGame; // Global 클래스가 정의된 네임스페이스를 추가해야 할 수 있습니다.
using Manager;
using UnityEngine;

public class MoveSpikeBullet : Bullet
{
    [SerializeField] private float moveSpeed = 2f;
    private Transform playerTransform;
    private bool isSoundPlaying = false; // 효과음 재생 상태 변수 추가

    protected override void Awake()
    {
        base.Awake();
        playerTransform = GameManager.Instance.player.transform;
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!isSoundPlaying) // 효과음이 재생 중이지 않을 때만 재생
        {
            float clipLength = Global.SoundManager.GetSFXClipLength(Data.SFXEnum.SpikeFloor);
            Global.SoundManager.PlaySFX(Data.SFXEnum.SpikeFloor, 0.05f, 0.1f);
            isSoundPlaying = true; // 효과음 재생 상태 설정
            StartCoroutine(WaitForSound(clipLength)); // 효과음 대기 코루틴 시작
        }
    }

    private IEnumerator WaitForSound(float clipLength)
    {
        yield return new WaitForSeconds(clipLength); // 효과음 길이만큼 대기
        isSoundPlaying = false; // 대기 후 효과음 재생 상태 해제
    }
}
