using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
public class FadeScript : MonoBehaviour
{
    public Image panel;
    float time = 0f;
    float F_time = 2f;
    public bool isTitle = false;
    void Start() {
        if(!isTitle) StartCoroutine(FadeFlow());
    }
    public void Fade() {
        StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow() {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        while (alpha.a > 0f) {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            panel.color = alpha;
            yield return null;
        }
        panel.gameObject.SetActive(false);
        yield return null;
    }
}
