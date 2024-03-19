using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {
    public Slider progerssBar;
    public static void LoadScene() {
        SceneManager.LoadScene(0);
    }

    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess() {
        AsyncOperation op = SceneManager.LoadSceneAsync(0);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone) {
            yield return null;

            if(op.progress < 0.9f) {
                progerssBar.value = op.progress;
            }
            else {
                timer += Time.unscaledDeltaTime;
                progerssBar.value = Mathf.Lerp(0.9f, 1f, timer);
                if(progerssBar.value >= 1f) {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

        }
        
        
    }
    
}
