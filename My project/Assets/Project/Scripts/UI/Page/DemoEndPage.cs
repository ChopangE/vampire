using System;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityWeld.Binding;

namespace UI.Page
{
    [Binding]
    public class DemoEndPage : PageViewModel
    {
        [Binding]
        public string DemoEndTitle { get; } = "데모 종료";
        
        [Binding]
        public string DemoEndMessage { get; } = "데모 버전은 여기까지입니다.\n정식 버전을 기대해 주세요!";
        
        [Binding]
        public string ReturnToTitleText { get; } = "타이틀로 돌아가기";
        
        [Binding]
        public void ReturnToTitle()
        {
            // 타이틀 씬으로 돌아가기 (씬 인덱스 0 또는 적절한 씬 이름 사용)
            Global.UIManager.CloseAllPages();
            SceneManager.LoadScene(0); // 또는 SceneManager.LoadScene("TitleScene");
        }
        
        [Binding]
        public void RestartDemo()
        {
            // 데모를 처음부터 다시 시작
            Global.UserDataManager.storage.curStage = 0;
            Global.DataManager.SaveData();
            Global.UIManager.CloseAllPages();
            
            // 현재 씬 다시 로드하거나 적절한 씬으로 이동
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}