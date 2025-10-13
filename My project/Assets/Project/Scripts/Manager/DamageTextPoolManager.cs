using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InGame;
using ObjectPooling;
using Sirenix.OdinInspector;
using UI.InGame;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace Manager
{
    public class DamageTextPoolManager : ObjectPoolManager
    {
        public event EventHandler<DamageText> OnDamageTextSpawned;

        [BoxGroup("세부 설정")] [LabelText("소환할 데미지 텍스트")] [SerializeField]
        private List<GameObject> damageTextList;
        
        [BoxGroup("세부 설정")] [LabelText("데미지 텍스트 부모 객체")]
        public Transform DamageTextGroup;
        
        [BoxGroup("세부 설정")] [LabelText("최대 데미지 텍스트 수")] [SerializeField]
        private int maxDamageTextCount = 50;

        public bool showDamageText = true;
        private ObjectPool<PoolObject> damageTextPool;
        private List<DamageText> spawnedDamageTextList = new List<DamageText>();
    // throttle spikes: limit how many damage texts can spawn per frame
    [SerializeField] private int maxSpawnPerFrame = 60;
    private int spawnThisFrame = 0;
        private void Awake()
        {
            DOTween.SetTweensCapacity(500, 200);
        }

        private void OnEnable()
        {
            ResetAllPools();
        }

        void Update()
        {
            // reset per-frame spawn counter
            spawnThisFrame = 0;
        }

        public void SpawnDamageText(Vector3 position, float damage, bool isCritical = false, bool isBonus = false)
        {
            if (!showDamageText) return;
            if (activeObjCount >= maxDamageTextCount) return;
            if (spawnThisFrame >= maxSpawnPerFrame) return; // throttle bursts
            spawnThisFrame++;

            DamageText damageText = damageTextPool.PullGameObject(position, Quaternion.identity, DamageTextGroup)
                .GetComponent<DamageText>();
            
            damageText.SetDamage(damage, isCritical, isBonus);
            spawnedDamageTextList.Add(damageText);
        }

        protected override void ResetOnPull(PoolObject poolObject)
        {
            DamageText damageText = poolObject as DamageText;
            activeObjCount++;
            OnDamageTextSpawned?.Invoke(this, damageText);
        }

        protected override void ResetOnPush(PoolObject pushObject)
        {
            activeObjCount--;
            spawnedDamageTextList.Remove(pushObject.GetComponent<DamageText>());
        }

        protected override void ResetAllPools()
        {
            // Pre-warm pool to avoid runtime instantiation and list resizes
            int prewarm = Mathf.Max(1, maxDamageTextCount);
            damageTextPool = new ObjectPool<PoolObject>(damageTextList[0], ResetOnPull, ResetOnPush, prewarm);
            spawnedDamageTextList = new List<DamageText>(prewarm);
        }
    }
}
