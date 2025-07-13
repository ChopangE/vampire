using System;
using System.Collections.Generic;
using Data;
using Manager;
using UnityEngine;
using UnityWeld;
using UnityWeld.Binding;
namespace UI.Page
{
    [Binding]
    public class OptionPage : PageViewModel
    {
        
        private List<ViewModel> optionPageViews = new List<ViewModel>();
        private void OnEnable()
        {
            var allChildrenPages = GetComponentsInChildren<ViewModel>();
            // 자기 자신의 경우엔 무시 
            foreach (var child in allChildrenPages)
            {
                if (child.transform.name != transform.name) optionPageViews.Add(child);
            }

            if(GameManager.HasInstance) {
                GameManager.Instance.Stop();
            }
            Global.SoundManager.StopBGM(true);

            OnClickGeneralView();
        }

        private void OnDisable() {
            if(GameManager.HasInstance) {
                GameManager.Instance.Resume();
            }
            Global.SoundManager.StopBGM(false);
        }
        [Binding]
        public void OnClickKeepPlaying()
        {
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
            Global.UIManager.ClosePage();
        }
        [Binding]
        public void OnClickGeneralView()
        {
            foreach (var view in optionPageViews)
            {
                if (view as GeneralOptionViewModel)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }

        [Binding]
        public void OnClickDisplayView()
        {
            foreach (var view in optionPageViews)
            {
                if (view as DisplayOptionViewModel)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }


        [Binding]
        public void OnClickSoundView()
        {
            foreach (var view in optionPageViews)
            {
                if (view as SoundOptionViewModel)
                {
                    view.gameObject.SetActive(true);
                    // 사운드 설정 새로고침
                    ((SoundOptionViewModel)view).RefreshSettings();
                }
                else view.gameObject.SetActive(false);
            }
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }
        [Binding]
        public void OnClickKeyView()
        {
            foreach (var view in optionPageViews)
            {
                if (view as KeyOptionViewModel)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
            Global.SoundManager.PlaySFX(SFXEnum.Shop_Button_1);
        }
    }
}