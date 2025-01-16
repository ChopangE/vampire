using System.Collections;
using System.Collections.Generic;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityWeld;
using UnityWeld.Binding;

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

    }

    void Next(bool isEvaluation = false)
    {
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        
        var notMaxLevelItems = DataManager.Instance.GetNotMaxLevelItems();
        var maxLevelItems = isEvaluation ? DataManager.Instance.GetMaxLevelItems() : null;

        int[] ran = new int[3];
        int count = 0;
        while (true)
        {
            if (isEvaluation)
            {
                int maxLevelSlot = Random.Range(0, 3);  // 만렙 아이템이 들어갈 위치
                ran[maxLevelSlot] = Random.Range(0, maxLevelItems.Length);

                maxLevelScrollNum = maxLevelSlot;
                
                // 나머지 두 슬롯에 일반 아이템 배치
                int normalItemIndex = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i != maxLevelSlot)
                    {
                        ran[i] = Random.Range(0, notMaxLevelItems.Length);
                        if (normalItemIndex > 0 && ran[i] == ran[(maxLevelSlot + 1) % 3]) continue;
                        normalItemIndex++;
                    }
                }
                if (normalItemIndex == 2) break;
            }
            else
            {
                maxLevelScrollNum = -1;
                ran[0] = Random.Range(0, notMaxLevelItems.Length);
                ran[1] = Random.Range(0, notMaxLevelItems.Length);
                ran[2] = Random.Range(0, notMaxLevelItems.Length);

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
            ItemData itemData = isEvaluation && ran[i] >= notMaxLevelItems.Length 
                ? maxLevelItems[ran[i]] 
                : notMaxLevelItems[ran[i]];
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
