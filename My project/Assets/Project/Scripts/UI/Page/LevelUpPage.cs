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
        Debug.Log("Next() 호출" + isEvaluation);
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        
        var activeWeapons = GameManager.Instance.weaponController.ActiveWeapons;
        bool isWeaponsFull = activeWeapons.Count >= GameManager.Instance.weaponController.maxActiveWeaponCount;
        
        // 일반 아이템 목록 (패시브 아이템 제외)
        var notMaxLevelItems = Global.DataManager.GetNotMaxLevelItems()
            .Where(item => !item.isEvaluateWeapon && item.itemType != ItemType.Passive)
            .Where(item => !(item.itemDataInfo.curLevel >= item.itemDataInfo.maxLevel && item._nextItemData == null))
            .ToArray();

        // 현재 보유 중인 무기 중 최대 레벨이 아닌 것들
        var currentWeapons = notMaxLevelItems
            .Where(item => activeWeapons.Any(w => w.id == item.itemDataInfo.itemId))
            .ToArray();

        // 패시브 아이템 목록
        var passiveItems = Global.DataManager.items
            .Where(item => item.itemType == ItemType.Passive)
            .Where(item => !(item.itemDataInfo.curLevel >= item.itemDataInfo.maxLevel && item._nextItemData == null))
            .ToArray();


        // 모든 일반 스킬이 최대 레벨인지 확인
        bool allNormalSkillsMaxed = Global.DataManager.GetNotMaxLevelItems()
            .Where(item => !item.isEvaluateWeapon && item.itemType != ItemType.Passive)
            .Count() == 0;

        // 모든 아이템이 최대 레벨이면 바로 Hide 처리
        if (allNormalSkillsMaxed && passiveItems.Length == 0 && (!isEvaluation || 
            (isEvaluation && (Global.DataManager.GetMaxLevelItems()
                .Where(item => item.isEvaluateWeapon)
                .Where(item => item.itemType != ItemType.Passive)
                .Where(item => item._nextItemData != null).Count() == 0))))
        {
            Debug.Log("모든 아이템이 최대 레벨이므로 레벨업 화면을 닫습니다.");
            Hide();
            return;
        }

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

        // 더 이상 레벨업 가능한 아이템이 없으면 Hide 처리
        if (notMaxLevelItems.Length == 0)
        {
            Debug.Log("레벨업 가능한 아이템이 없으므로 레벨업 화면을 닫습니다.");
            Hide();
            return;
        }

        // 최대 레벨 아이템 목록
        var maxLevelItems = isEvaluation ? 
            Global.DataManager.GetMaxLevelItems()
                .Where(item => !item.isEvaluateWeapon)
                .ToArray() : null;

        // 진화무기 목록 - isEvaluation이 true일 때만 계산
        var evaluateWeapons = isEvaluation ? 
            Global.DataManager.GetNotMaxLevelItems()
                .Where(item => item.isEvaluateWeapon)
                .Where(item => item.itemType != ItemType.Passive)
                .Where(item => item._prevItemData != null)
                .Where(item => Global.DataManager.GetMaxLevelItems().Any(maxLevelItem => maxLevelItem.itemDataInfo.itemId == item._prevItemData.itemDataInfo.itemId))
                .ToArray() : null;
        if(evaluateWeapons != null)
        {
            foreach(var item in evaluateWeapons)
            {
                Debug.Log("진화무기: " + item._prevItemData.itemDataInfo.curLevel + " " + item._prevItemData.itemDataInfo.maxLevel + " " + item.itemDataInfo.itemId.ToString());
            }
        }

        int[] ran = new int[3];
        int count = 0;
        
        // 사용 가능한 아이템이 3개 미만인 경우 대비
        int availableItemCount = isEvaluation && evaluateWeapons != null && evaluateWeapons.Length > 0 ?
            evaluateWeapons.Length + notMaxLevelItems.Length : notMaxLevelItems.Length;
            
        // 아이템 수가 3개 미만이면 중복을 허용
        bool allowDuplicates = availableItemCount < 3;
        
        // 진화 무기 관련 초기화
        int evolveSlot = -1;
        
        try
        {
            // 디버깅용 로그 추가
            Debug.Log($"사용 가능한 아이템 수: {availableItemCount}, 중복 허용: {allowDuplicates}, 패시브: {passiveItems.Length}");
            
            // 최대 시도 횟수를 제한하여 무한 루프 방지
            for (count = 0; count < 50; count++)
            {
                // isEvaluation이 false면 진화무기 관련 로직 스킵하고 일반 레벨업 로직만 실행
                if (isEvaluation && evaluateWeapons != null && evaluateWeapons.Length > 0)
                {
                    // 진화무기를 표시할 슬롯 선택
                    evolveSlot = Random.Range(0, 3);
                    ran[evolveSlot] = Random.Range(0, evaluateWeapons.Length);
                    maxLevelScrollNum = evolveSlot;
    
                    // 나머지 슬롯에 일반 아이템 배치
                    bool isValid = true;
                    int normalItemIndex = 0;
                    
                    for (int i = 0; i < 3; i++)
                    {
                        if (i != evolveSlot)
                        {
                            if (notMaxLevelItems.Length == 0) 
                            {
                                // 일반 아이템이 없는 경우 같은 진화 무기를 표시
                                ran[i] = ran[evolveSlot];
                                normalItemIndex++;
                                continue;
                            }
                            
                            ran[i] = Random.Range(0, notMaxLevelItems.Length);
                            
                            // 중복을 허용하는 경우 검사 스킵
                            if (!allowDuplicates && normalItemIndex > 0 && ran[i] == ran[(evolveSlot + 1) % 3]) 
                            {
                                isValid = false;
                                break;
                            }
                            normalItemIndex++;
                        }
                    }
                    
                    if (!isValid) continue;
                    
                    // 모든 아이템이 패시브가 아닌 경우에만 패시브 아이템 개수 제한
                    if (!allNormalSkillsMaxed && notMaxLevelItems.Length > 0)
                    {
                        // 패시브 아이템 개수 체크
                        int passiveCount = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (i != evolveSlot && ran[i] < notMaxLevelItems.Length && notMaxLevelItems[ran[i]].itemType == ItemType.Passive)
                                passiveCount++;
                        }
                        if (passiveCount > 1 && !allowDuplicates) continue;
                    }
                    
                    // 조건 만족
                    if (normalItemIndex == 2 || allowDuplicates) break;
                }
                else
                {
                    // 진화모드가 아니거나 진화무기가 없는 경우 - 일반 레벨업 로직
                    maxLevelScrollNum = -1;
                    
                    // 사용 가능한 아이템이 없는 경우 처리
                    if (notMaxLevelItems.Length == 0)
                    {
                        Debug.LogWarning("레벨업 가능한 아이템이 없습니다.");
                        Hide();
                        return;
                    }
                    
                    ran[0] = Random.Range(0, notMaxLevelItems.Length);
                    ran[1] = Random.Range(0, notMaxLevelItems.Length);
                    ran[2] = Random.Range(0, notMaxLevelItems.Length);
    
                    // 모든 아이템이 패시브가 아닌 경우에만 패시브 아이템 개수 제한
                    if (!allNormalSkillsMaxed)
                    {
                        // 패시브 아이템 개수 체크
                        int passiveCount = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (notMaxLevelItems[ran[i]].itemType == ItemType.Passive)
                                passiveCount++;
                        }
                        if (passiveCount > 1 && !allowDuplicates) continue;
                    }
    
                    // 중복을 허용하거나 모든 아이템이 서로 다른 경우
                    if (allowDuplicates || (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])) break;
                }
                
                // 25번 시도 후에는 중복 허용
                if (count >= 25)
                {
                    Debug.LogWarning("LevelUpPage: Next() 아이템 선택 반복 횟수 초과 - 중복 허용");
                    allowDuplicates = true;
                }
            }
            
            // 최대 시도 횟수를 초과한 경우 (무한 루프 방지)
            if (count >= 50)
            {
                Debug.LogError("LevelUpPage: Next() 50번 이상 반복" + notMaxLevelItems.Length);
                // 중복을 허용하여 강제로 선택
                allowDuplicates = true;
                
                if (isEvaluation && evaluateWeapons != null && evaluateWeapons.Length > 0)
                {
                    // 진화 무기 슬롯 처리
                    if (evolveSlot == -1) evolveSlot = 0;
                    maxLevelScrollNum = evolveSlot;
                    ran[evolveSlot] = evaluateWeapons.Length > 0 ? 0 : 0;
                    
                    // 나머지 슬롯 처리
                    for (int i = 0; i < 3; i++)
                    {
                        if (i != evolveSlot)
                        {
                            ran[i] = notMaxLevelItems.Length > 0 ? Random.Range(0, notMaxLevelItems.Length) : 0;
                        }
                    }
                }
                else
                {
                    // 진화 무기가 없는 경우 - 각 슬롯마다 다른 랜덤값 설정
                    if (notMaxLevelItems.Length > 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0 || allowDuplicates)
                            {
                                ran[i] = Random.Range(0, notMaxLevelItems.Length);
                            }
                            else
                            {
                                // 이전 슬롯과 다른 값 설정 시도
                                int attempts = 0;
                                int prevIndex = ran[i-1];
                                int randIndex;
                                
                                do {
                                    randIndex = Random.Range(0, notMaxLevelItems.Length);
                                    attempts++;
                                } while (randIndex == prevIndex && attempts < 10 && notMaxLevelItems.Length > 1);
                                
                                ran[i] = randIndex;
                            }
                        }
                    }
                    else
                    {
                        // 사용 가능한 아이템이 없는 경우 처리
                        Debug.LogWarning("레벨업 가능한 아이템이 없습니다. 화면을 닫습니다.");
                        Hide();
                        return;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LevelUpPage: Next() 예외 발생: " + e.Message);
            Hide();
            return;
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
