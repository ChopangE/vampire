using System.Collections;
using System.Collections.Generic;
using Manager;
using UI.Page;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeScript : MonoBehaviour
{
    public Image panel;
    float time = 0f;
    float F_time = 1.5f;
    public bool isTitle = false;
    void Start() {
        if(!isTitle) StartCoroutine(FadeFlow());
    }
    public void Fade() {
        StartCoroutine(FadeFlow());
    }

    public void FadeOut(bool isGameOver = false) {
        StartCoroutine(FadeOutFlow(isGameOver));
    }


    IEnumerator FadeFlow() {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        time = 0f;
        while (alpha.a > 0f) {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            panel.color = alpha;
            yield return null;
        }
        panel.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator FadeOutFlow(bool isGameOver = false) {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        time = 0f;
        float maxAlpha = 1f;
        if(isGameOver) maxAlpha = 0.5f;
        while (alpha.a < maxAlpha) {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, maxAlpha, time);
            panel.color = alpha;
            yield return null;
        }
        Global.UIManager.OpenPage<GameOverPage>();
        yield return new WaitForSeconds(2f);
        if(isGameOver) {
            SceneManager.LoadScene("Map");
        }
        yield return null;
    }
}
