using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using OutGame;
using UI.Page;
using Manager;

public class LoadingScene : SceneBase {
    private LoadingPage loadingPage;
    public static void LoadScene() {
        SceneManager.LoadScene(0);
    }

    private void OnEnable() {
        loadingPage = Global.UIManager.OpenPage<LoadingPage>();
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess() {
        AsyncOperation op = SceneManager.LoadSceneAsync("InGameScene");
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone) {
            yield return null;

            if(op.progress < 0.9f) {
                loadingPage.Progress = op.progress;
            }
            else {
                timer += Time.unscaledDeltaTime;
                loadingPage.Progress = Mathf.Lerp(0.9f, 1f, timer);
                if(loadingPage.Progress >= 1f) {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

        }
        
        
    }
    
}
