using System.Collections;
using System.Collections.Generic;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityWeld;
using UnityWeld.Binding;
using System.Linq;

[Binding]
public class LevelUpPage : ViewModel
{
    RectTransform rect;
    [SerializeField] private Transform itemParent;
    private Item[] items;
    private int maxLevelScrollNum = -1;

    protected override void Awake()
    {
        base.Awake();
        rect = GetComponent<RectTransform>();
        items = itemParent.GetComponentsInChildren<Item>(true);
    }

    public void Show(bool isEvaluation = false)
    {
        Next(isEvaluation);
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();
        Global.SoundManager.StopBGM(true);
    }

    [Binding]
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        Global.SoundManager.StopBGM(false);
        GameManager.Instance.OnLevelUpComplete();
    }

    void Next(bool isEvaluation = false)
    {
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        
        var activeWeapons = GameManager.Instance.weaponController.ActiveWeapons;
        bool isWeaponsFull = activeWeapons.Count >= GameManager.Instance.weaponController.maxActiveWeaponCount;
        
        // 일반 아이템 목록 (패시브 아이템 제외)
        var notMaxLevelItems = Global.DataManager.GetNotMaxLevelItems()
            .Where(item => !item.isEvaluateWeapon && item.itemType != ItemType.Passive)
            .ToArray();

        // 현재 보유 중인 무기 중 최대 레벨이 아닌 것들
        var currentWeapons = notMaxLevelItems
            .Where(item => activeWeapons.Any(w => w.id == item.itemDataInfo.itemId))
            .ToArray();

        // 패시브 아이템 목록
        var passiveItems = Global.DataManager.items
            .Where(item => item.itemType == ItemType.Passive)
            .ToArray();

        // 모든 일반 스킬이 최대 레벨인지 확인
        bool allNormalSkillsMaxed = Global.DataManager.GetNotMaxLevelItems()
            .Where(item => !item.isEvaluateWeapon && item.itemType != ItemType.Passive)
            .Count() == 0;

        // 무기가 가득 찼거나 모든 일반 스킬이 최대 레벨일 때의 아이템 풀 설정
        if (isWeaponsFull || allNormalSkillsMaxed)
        {
            if (allNormalSkillsMaxed)
            {
                notMaxLevelItems = passiveItems;
            }
            else
            {
                notMaxLevelItems = currentWeapons.Concat(passiveItems).ToArray();
            }
        }
        else
        {
            notMaxLevelItems = notMaxLevelItems.Concat(passiveItems).ToArray();
        }

        // 최대 레벨 아이템 목록
        var maxLevelItems = isEvaluation ? 
            Global.DataManager.GetMaxLevelItems()
                .Where(item => !item.isEvaluateWeapon)
                .ToArray() : null;

        // 진화무기 목록
        var evaluateWeapons = isEvaluation ? 
            Global.DataManager.GetMaxLevelItems()
                .Where(item => item.isEvaluateWeapon)
                .Where(item => item.itemType != ItemType.Passive)
                .ToArray() : null;

        int[] ran = new int[3];
        int count = 0;
        while (true)
        {
            if (isEvaluation)
            {
                if (evaluateWeapons != null && evaluateWeapons.Length > 0)
                {
                    // 진화무기를 표시할 슬롯 선택
                    int evolveSlot = Random.Range(0, 3);
                    ran[evolveSlot] = Random.Range(0, evaluateWeapons.Length);
                    maxLevelScrollNum = evolveSlot;

                    // 나머지 슬롯에 일반 아이템 배치
                    int normalItemIndex = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i != evolveSlot)
                        {
                            ran[i] = Random.Range(0, notMaxLevelItems.Length);
                            if (normalItemIndex > 0 && ran[i] == ran[(evolveSlot + 1) % 3]) continue;
                            normalItemIndex++;
                        }
                    }
                    
                    // 패시브 아이템 개수 체크
                    int passiveCount = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i != evolveSlot && notMaxLevelItems[ran[i]].itemType == ItemType.Passive)
                            passiveCount++;
                    }
                    if (passiveCount > 1) continue;
                    
                    if (normalItemIndex == 2) break;
                }
                else
                {
                    // 진화무기가 없는 경우 기존 로직대로 처리
                    ran[0] = Random.Range(0, notMaxLevelItems.Length);
                    ran[1] = Random.Range(0, notMaxLevelItems.Length);
                    ran[2] = Random.Range(0, notMaxLevelItems.Length);
                    
                    // 패시브 아이템 개수 체크
                    int passiveCount = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (notMaxLevelItems[ran[i]].itemType == ItemType.Passive)
                            passiveCount++;
                    }
                    if (passiveCount > 1) continue;
                    
                    if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2]) break;
                }
            }
            else
            {
                maxLevelScrollNum = -1;
                ran[0] = Random.Range(0, notMaxLevelItems.Length);
                ran[1] = Random.Range(0, notMaxLevelItems.Length);
                ran[2] = Random.Range(0, notMaxLevelItems.Length);

                // 패시브 아이템 개수 체크
                int passiveCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (notMaxLevelItems[ran[i]].itemType == ItemType.Passive)
                        passiveCount++;
                }
                if (passiveCount > 1) continue;

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2]) break;
            }
            
            count++;
            if(count > 200) {
                Debug.LogError("LevelUpPage: Next() 200번 이상 반복" + notMaxLevelItems.Length);
                break;
            }
        }
        CheckMaxLevelScroll();

        for (int i = 0; i < ran.Length; i++)
        {
            ItemData itemData;
            if (isEvaluation && i == maxLevelScrollNum && evaluateWeapons != null && evaluateWeapons.Length > 0)
            {
                // 진화무기 슬롯인 경우
                itemData = evaluateWeapons[ran[i]];
            }
            else
            {
                // 일반 아이템 슬롯
                itemData = notMaxLevelItems[ran[i]];
            }
            items[i].data = itemData;
            items[i].gameObject.SetActive(true);
        }
    }

    private void CheckMaxLevelScroll()
    {
        if(maxLevelScrollNum == -1)
        {
            foreach(var item in items)
            {
                item.IsMaxLevel = false;
            }
        }
        else
        {
            for(int i = 0; i < items.Length; i++)
            {
                items[i].IsMaxLevel = i == maxLevelScrollNum;
            }
        }
    }
}
