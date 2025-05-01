using System;
using Data;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace UI.Page
{
    [Binding]
    public class CreditPage : PageViewModel
    {
        private FadeScript fade;
        private CreditViewModel creditViewModel;
        private void Start() {
            fade = GetComponentInChildren<FadeScript>(true);
            creditViewModel = GetComponentInChildren<CreditViewModel>(true);
        }
        private void OnEnable() {
            StartCoroutine(CreditRoutine());
        }
        private IEnumerator CreditRoutine() {
            yield return new WaitForSeconds(2f);
            creditViewModel.gameObject.SetActive(true);
        }
        [Binding]
        public void OnClickOptionButton()
        {
            Global.UIManager.OpenPage<OptionPage>();
        }
        [Binding]
        public void OnClickExitButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}