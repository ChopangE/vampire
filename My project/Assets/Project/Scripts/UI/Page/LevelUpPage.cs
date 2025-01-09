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
    protected override void Awake()
    {
        base.Awake();
        rect = GetComponent<RectTransform>();
        items = itemParent.GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
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

    void Next()
    {

        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        var notMaxLevelItems = DataManager.Instance.GetNotMaxLevelItems();

        int[] ran = new int[3];
        int count = 0;
        while (true)
        {
            ran[0] = Random.Range(0, notMaxLevelItems.Length);
            ran[1] = Random.Range(0, notMaxLevelItems.Length);
            ran[2] = Random.Range(0, notMaxLevelItems.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2]) break;
            count++;
            if(count > 100) {
                Debug.LogError("LevelUpPage: Next() 100번 이상 반복" + notMaxLevelItems.Length);
                break;
            }
        }

        for (int i = 0; i < ran.Length; i++)
        {
            ItemData itemData = notMaxLevelItems[ran[i]];
            items[i].data = itemData;
            items[i].gameObject.SetActive(true);
        }

    }
}
