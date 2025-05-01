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
    private Color fadeColor = Color.black; // 기본 페이드 색상 (검정)
    
    void Start() {
        if(!isTitle) StartCoroutine(FadeFlow());
    }
    
    // 색상을 지정하는 메서드 추가
    public void SetFadeColor(Color color) {
        fadeColor = color;
        // alpha 값은 유지하고 RGB만 변경
        Color panelColor = panel.color;
        panelColor.r = color.r;
        panelColor.g = color.g;
        panelColor.b = color.b;
        panel.color = panelColor;
    }
    public void Fade() {
        StartCoroutine(FadeFlow());
    }
    
    // 지정된 색상으로 페이드
    public void Fade(Color color, System.Action onComplete = null) {
        SetFadeColor(color);
        StartCoroutine(FadeFlow(onComplete));
    }

    public void InGameFade(bool isGameOver = false, bool isGameWin = false, System.Action onComplete = null) {
        StartCoroutine(InGameFadeFlow(isGameOver, isGameWin, onComplete));
    }
    
    // 지정된 색상으로 페이드 아웃
    public void InGameFade(Color color, bool isGameOver = false, bool isGameWin = false, System.Action onComplete = null) {
        SetFadeColor(color);
        StartCoroutine(InGameFadeFlow(isGameOver, isGameWin, onComplete));
    }

    IEnumerator FadeFlow(System.Action onComplete = null) {
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
        
        // 완료 콜백 호출
        onComplete?.Invoke();
        
        yield return null;
    }

    IEnumerator InGameFadeFlow(bool isGameOver = false, bool isGameWin = false, System.Action onComplete = null) {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        time = 0f;
        float maxAlpha = 1f;
        if(isGameOver || isGameWin) maxAlpha = 0.5f;
        while (alpha.a < maxAlpha) {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, maxAlpha, time);
            panel.color = alpha;
            yield return null;
        }
        if(isGameOver) Global.UIManager.OpenPage<GameOverPage>();
        else if(isGameWin) Global.UIManager.OpenPage<GameWinPage>();
        yield return new WaitForSeconds(2f);
        if(isGameOver || isGameWin) {
            GameManager.Instance.isStageClear = false;
            if(Global.UserDataManager.curStage != 13)
                SceneManager.LoadScene("Map");
            else
            {
                Global.UIManager.OpenPage<CreditPage>();
            }
        }
        // 완료 콜백 호출
        onComplete?.Invoke();
        yield return null;
    }

    // 화면을 완전히 페이드 인하는 메서드 (화면이 어두워지는 효과)
    public void FadeIn(System.Action onComplete = null) {
        StartCoroutine(FadeInFlow(onComplete));
    }

    // 지정된 색상으로 페이드 인
    public void FadeIn(Color color, System.Action onComplete = null) {
        SetFadeColor(color);
        StartCoroutine(FadeInFlow(onComplete));
    }

    // 페이드 아웃 메서드 (화면이 밝아지는 효과)
    public void FadeOut(System.Action onComplete = null) {
        StartCoroutine(FadeFlow(onComplete));
    }

    // 지정된 색상으로 페이드 아웃
    public void FadeOut(Color color, System.Action onComplete = null) {
        SetFadeColor(color);
        StartCoroutine(FadeFlow(onComplete));
    }

    // 게임 상태를 고려한 페이드 아웃
    public void FadeOut(Color color, bool isGameOver = false, bool isGameWin = false, System.Action onComplete = null) {
        SetFadeColor(color);
        StartCoroutine(InGameFadeFlow(isGameOver, isGameWin, onComplete));
    }

    // 페이드 인 코루틴 (화면이 어두워지는 효과)
    IEnumerator FadeInFlow(System.Action onComplete = null) {
        panel.gameObject.SetActive(true);
        Color alpha = panel.color;
        alpha.a = 0f; // 시작 알파값 설정
        panel.color = alpha;
        
        time = 0f;
        float maxAlpha = 1f;
        
        while (alpha.a < maxAlpha) {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, maxAlpha, time);
            panel.color = alpha;
            yield return null;
        }
        
        // 완료 콜백 호출
        onComplete?.Invoke();
        
        yield return null;
    }
}
