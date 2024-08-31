using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityWeld;

public class LevelUpPage : ViewModel
{
    RectTransform rect;
    Item[] items; 
    protected override void Awake() {
        base.Awake();
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show() {
        Next();
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide() {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        AudioManager.instance.EffectBgm(false);

    }

    void Next() {

        foreach(Item item in items) {
            item.gameObject.SetActive(false);
        }

        int[] ran = new int[3];
        while (true) {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2]) break;
        }

        for(int i = 0; i < ran.Length; i++) {
            Item ranItem = items[ran[i]];
            if(ranItem.level == ranItem.data.damages.Length) {
                items[8].gameObject.SetActive(true);
            }
            else {
                ranItem.gameObject.SetActive(true);

            }


        }

    }
}
