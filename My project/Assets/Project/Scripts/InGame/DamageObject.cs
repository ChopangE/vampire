using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class DamageObject : MonoBehaviour, IDamageHitable
    {
        public event Action<float> OnHit;
        [Range(0,1000)] [BoxGroup("체력"), HideLabel] [LabelText("최대 체력")]
        public float maxHealth;
        [ProgressBar(0, 10, 0, 1, 0, Segmented = true)] [Range(0,1000)] [BoxGroup("체력"), HideLabel] [LabelText("현재 체력")] 
        public float health;

        [LabelText("넉백 옵션")] [SerializeField] 
        private bool canKnockBack = false;
        private Rigidbody2D _rigid;
        private Collider2D _coll;
        private bool _isLive;
        protected virtual void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collider2D>();
        }
        protected virtual void OnEnable()
        {
            _isLive = true;
            _coll.enabled = true;
            _rigid.simulated = true;
            health = maxHealth;
        }
        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Bullet")) return;
            
            var damage = collision.GetComponent<Bullet>().damage;
            OnHit?.Invoke(damage);
            health -= damage;

            if (collision.CompareTag("Melee") && canKnockBack) StartCoroutine(KnockBack());

            if (health < 0) 
            {
                Dead();
            }
        }
        private IEnumerator KnockBack()
        {
            yield return null;
            Vector3 playerPos = GameManager.Instance.player.transform.position;
            Vector3 dir = transform.position - playerPos;
            _rigid.AddForce(dir.normalized * 5, ForceMode2D.Impulse);
        }
        //* 상속해서 수정할 수 있게 추상화
        public virtual void Dead()
        {
            _isLive = false;
            _coll.enabled = false;
            _rigid.simulated = false;
            gameObject.SetActive(false);
        }
    }
}