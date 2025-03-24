using Data;
using Manager;
using UI.Page;
using UnityEngine;
using UnityEngine.Playables;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class IntroViewModel : ViewModel
    {
        private bool _isIntroCutScenePlaying = false;
        [Binding]
        public bool IsIntroCutScenePlaying
        {
            get => _isIntroCutScenePlaying;
            set
            {
                _isIntroCutScenePlaying = value;
                OnPropertyChanged(nameof(IsIntroCutScenePlaying));
            }
        }
        [SerializeField] private PlayableDirector _introCutScene;
        [SerializeField] private FadeScript _fadeScript;
        [Binding]
        public void PlayIntroCutScene()
        {
            _fadeScript.FadeIn(Color.white, () =>
            {
                _fadeScript.panel.gameObject.SetActive(false);
                IsIntroCutScenePlaying = true;
                _introCutScene.Play();
                Global.SoundManager.PlayMusic(BGMEnum.TitleFireBurning);
            });
        }
        public void OnIntroCutSceneEnd()
        {
            _fadeScript.panel.gameObject.SetActive(true);
            _fadeScript.FadeIn(Color.black, () =>
            {
                IsIntroCutScenePlaying = false;
                Global.SoundManager.StopBGM();
                Global.UIManager.ClosePage();
                Global.UIManager.OpenPage<StartScenePage>();
            });
        }
    }
}